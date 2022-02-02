using System.Net;
using System.Net.Sockets;
using Altair.BBS.Service.Data.Events;
using Altair.BBS.Service.Data.Tcp;
using Serilog;

namespace Altair.BBS.Service.Services;

public class TcpService
{
    private Socket _serverSocket;
    private readonly ILogger _logger;
    private readonly List<TcpClientInfo> _clients = new();
    private readonly int _maxClients;
    private readonly int _listeningPort;
    private readonly int _readBufferSize = 1024;

    public event TcpServerEvents.ClientConnectionEventHandler ClientConnected;
    public event TcpServerEvents.ClientConnectionEventHandler ClientDisconnected;
    public event TcpServerEvents.ClientDataEventHandler DataReceived;

    public TcpService(ILogger logger, int listeningPort, int maxClients = 1024)
    {
        _maxClients = maxClients;
        _listeningPort = listeningPort;
        _logger = logger;
    }

    public void Start()
    {
        _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _serverSocket.Bind(new IPEndPoint(IPAddress.Any, _listeningPort));
        _serverSocket.Listen(_maxClients);
        _serverSocket.BeginAccept(AcceptedCallback, _serverSocket);
    }

    private void AcceptedCallback(IAsyncResult ar)
    {
        try
        {
            var clientSocket = _serverSocket.EndAccept(ar);
            var state = new TcpClientInfo(Guid.NewGuid().ToString(), clientSocket, new byte[_readBufferSize]);
            _clients.Add(state);

            ClientConnected?.Invoke(this, state);
            clientSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, ReadCallback, state);

            _serverSocket.BeginAccept(AcceptedCallback, _serverSocket);
        }
        catch (Exception ex)
        {
            _logger.Error("Error during accept {Error}", ex.Message);
        }
    }

    private void ReadCallback(IAsyncResult ar)
    {
        var state = (TcpClientInfo) ar.AsyncState!;
        var socket = state.ClientSocket;

        var bytesRead = socket.EndReceive(ar);

        if (bytesRead > 0)
        {
            var data = new byte[bytesRead];
            Array.Copy(state.Buffer, 0, data, 0, bytesRead);

            DataReceived?.Invoke(this, state, data);

            socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, ReadCallback, state);
        }
        else
        {
            ClientDisconnected?.Invoke(this, state);
        }
    }
}