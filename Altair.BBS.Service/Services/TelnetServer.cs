using System.Collections.Concurrent;
using System.Text;
using Altair.BBS.Api.Ansi;
using Altair.BBS.Service.Data.Tcp;
using Altair.BBS.Service.Data.Telnet;
using Serilog;

namespace Altair.BBS.Service.Services;

public class TelnetServer
{
    private TcpService _tcpService;

    public char PasswordChar { get; set; } = '#';
    public List<byte[]> OnConnectOptions { get; set; } = new();

    public bool IsRunning { get; set; }

    private readonly ILogger _logger;
    public ConcurrentDictionary<string, UserState> ConnectedClients { get; set; } = new();

    public TelnetServer(ILogger logger)
    {
        _tcpService = new TcpService(logger, 23);
        _logger = logger;
    }

    public void Start()
    {
        _tcpService.ClientConnected += OnClientConnected;
        _tcpService.DataReceived += OnDataReceived;
        _tcpService.Start();
        OnConnectOptions.Add(new[]
            {(byte) TelnetOpCodes.Commands.Iac, (byte) TelnetOpCodes.Commands.Dont, (byte) TelnetOpCodes.Options.Echo});
        OnConnectOptions.Add(new[]
        {
            (byte) TelnetOpCodes.Commands.Iac, (byte) TelnetOpCodes.Commands.Do,
            (byte) TelnetOpCodes.Options.NegotiateAboutWindowSize
        });
        OnConnectOptions.Add(new[]
        {
            (byte) TelnetOpCodes.Commands.Iac, (byte) TelnetOpCodes.Commands.Do,
            (byte) TelnetOpCodes.Options.EndOrRecord
        });
        OnConnectOptions.Add(new[]
        {
            (byte) TelnetOpCodes.Commands.Iac, (byte) TelnetOpCodes.Commands.Will,
            (byte) TelnetOpCodes.Options.TerminalType
        });
        OnConnectOptions.Add(new[]
        {
            (byte) TelnetOpCodes.Commands.Iac, (byte) TelnetOpCodes.Commands.Will,
            (byte) TelnetOpCodes.Options.EndOrRecord
        });

        IsRunning = true;
    }

    private void OnDataReceived(TcpService server, TcpClientInfo client, byte[] data)
    {
        if (data.Length == 0)
            return;

        while (data.Length > 0 && data[0] == (byte)TelnetOpCodes.Commands.Iac)
        {
            data = ParseOpCodes(data, client);
        }

        if (data.Length > 0)
        {
            if (data[0] == '\n' || data[0] == '\r')
            {
                if (data[0] == '\r')
                {
                    data = data.Skip(2).ToArray();
                }
                else
                {
                    data = data.Skip(1).ToArray();
                }

                var userState = ConnectedClients[client.ClientID];
                client.CurrentText = Encoding.UTF8.GetString(client.CurrentBuffer);
                _logger.Information("Command: {Cmd}", client.CurrentText);
                client.CleanBuffer();


                var task = new Task(async () =>
                {

                    userState.Send(AnsiColors.ClearScreen());


                    await Task.Delay(1000);
                    var data = "░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░\r\n" +
                               "\u001b[31mPlease enter your password:\u001b[0m\r\n" +
                               "░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░\r\n";

                    client.IsPassword = true;

                    foreach (var ch in data)
                    {
                        await Task.Delay(10);
                        userState.Send((byte)ch);
                    }


                    // userState.Send(0x1b, 0x5b, 0x32, 0x4A, 0x1b, 0x5b, 0x31, 0x3B, 0x31, 0x48);
                });
                task.Start();
            }
            else
            {
                client.CurrentBuffer = client.CurrentBuffer.Concat(data).ToArray();
            }
        }

        if (client.IsEcho)
        {
            if (client.IsPassword)
            {
                client.ClientSocket.Send(Enumerable.Range(0, data.Length).Select(i => (byte)PasswordChar).ToArray());
            }
            else
            {
                client.ClientSocket.Send(data);
            }
        }

        data.ToList().ForEach(b => { _logger.Information("Incoming byte {B}", b.ToString("X2")); });
    }

    private byte[] ParseOpCodes(byte[] data, TcpClientInfo client)
    {
        var buffer = data;
        if (buffer[0] == (byte)TelnetOpCodes.Commands.Iac)
        {
            buffer = buffer.Skip(1).ToArray();

            if (buffer[0] == (byte)TelnetOpCodes.Commands.SubEnd)
            {
                buffer = buffer.Skip(1).ToArray();
            }

            if (buffer[0] == (byte)TelnetOpCodes.Commands.SubBegin)
            {
                if (buffer[1] == (byte)TelnetOpCodes.Options.NegotiateAboutWindowSize)
                {
                    var size = buffer.Skip(2).Take(4).ToArray();
                    int witdh = size[1];
                    int height = size[3];
                    _logger.Information("Client window size: {Width} col {Height} rows", witdh, height);
                    client.ScreenSize = new(witdh, height);
                    client.Canvas = new byte[witdh, height];
                    buffer = data.Skip(7).ToArray();
                }
            }

            if (buffer[0] == (byte)TelnetOpCodes.TerminalType)
            {
            }
            else
            {
                var commandRecord = buffer.Take(2).ToArray();
                buffer = buffer.Skip(2).ToArray();

                _logger.Information("{Command} {Option}", (TelnetOpCodes.Commands)commandRecord[0],
                    (TelnetOpCodes.Options)commandRecord[1]);

                if (commandRecord[1] == (byte)TelnetOpCodes.Options.Echo)
                {
                    client.IsEcho = commandRecord[0] == (byte)TelnetOpCodes.Commands.Wont;
                }
            }
        }

        return buffer;
    }

    private void OnClientConnected(TcpService server, TcpClientInfo client)
    {
        var userState = new UserState()
        {
            TcpClientInfo = client,
            TextEncoder = Encoding.ASCII
        };

        ConnectedClients.TryAdd(userState.TcpClientInfo.ClientID, userState);

        foreach (var onConnectOption in OnConnectOptions)
        {
            userState.Send(onConnectOption);
        }


        userState.SendText("hello!");
    }
}