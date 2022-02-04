using System.Text;
using Altair.BBS.Service.Data.Tcp;

namespace Altair.BBS.Service.Data.Telnet;

public class UserState
{
    public TcpClientInfo TcpClientInfo { get; set; }

    public Encoding TextEncoder { get; set; }

    public UserStateOptions Options { get; set; } = new();

    public void SendText(string text, string crln = "\r\n")
    {
        TcpClientInfo.ClientSocket.Send(TextEncoder.GetBytes($"{text}{crln}"));
    }

    public void Send(params byte[] data )
    {
        TcpClientInfo.ClientSocket.Send(data);
    }
}

public class UserStateOptions
{
    public bool isEchoEnabled { get; set; }
}