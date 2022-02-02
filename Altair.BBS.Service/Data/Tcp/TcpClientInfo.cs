using System.Net.Sockets;

namespace Altair.BBS.Service.Data.Tcp;

public class TcpClientInfo
{
    public string ClientID { get; set; }
    public Socket ClientSocket { get; set; }
    public byte[] Buffer { get; set; }

    public TcpClientInfo(string id, Socket clientSocket, byte[] buffer)
    {
        ClientID = id;
        ClientSocket = clientSocket;
        Buffer = buffer;
    }
}