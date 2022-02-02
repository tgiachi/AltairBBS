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

    public bool IsRunning { get; set; }

    private readonly ILogger _logger;
    public BlockingCollection<UserState> ConnectedClients { get; set; } = new();

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
        IsRunning = true;
    }

    private void OnDataReceived(TcpService server, TcpClientInfo client, byte[] data)
    {
        if (data.Length == 0)
            return;

        while (data.Length > 0 && data[0] == (byte) TelnetOpCodes.Commands.Iac)
        {
            var record = data.Take(3).ToArray();
            ParseOpCodes(record);
            data = data.Skip(3).ToArray();
        }

        data.ToList().ForEach(b => { _logger.Information("Incoming byte {B}", b.ToString("X2")); });
    }

    private void ParseOpCodes(byte[] record)
    {
    }

    private void OnClientConnected(TcpService server, TcpClientInfo client)
    {
        var userState = new UserState()
        {
            TcpClientInfo = client,
            TextEncoder = Encoding.Default
        };

        ConnectedClients.TryAdd(userState);

        userState.Send(TelnetOpCodes.Iac, TelnetOpCodes.Do, TelnetOpCodes.Echo);
        userState.SendText("Hello!");
    }
}