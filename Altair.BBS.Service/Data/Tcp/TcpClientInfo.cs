using System.Drawing;
using System.Net.Sockets;

namespace Altair.BBS.Service.Data.Tcp;

public class TcpClientInfo
{
    private int _bufferLength;

    public byte[] CurrentBuffer { get; set; }
    public string CurrentText { get; set; }
    public string ClientID { get; set; }
    public Socket ClientSocket { get; set; }
    public byte[] Buffer { get; set; }

    public bool IsEcho { get; set; }

    public bool IsPassword { get; set; } = false;

    public Point ScreenSize { get; set; }

    public byte[,] Canvas { get; set; }

    public void CleanBuffer()
    {
        CurrentBuffer = new byte[_bufferLength];
    }

    public TcpClientInfo(string id, Socket clientSocket, byte[] buffer)
    {
        _bufferLength = buffer.Length;
        CurrentBuffer = new byte[_bufferLength];
        ClientID = id;
        ClientSocket = clientSocket;
        Buffer = buffer;
    }
}