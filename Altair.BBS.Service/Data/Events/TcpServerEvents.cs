using Altair.BBS.Service.Data.Tcp;
using Altair.BBS.Service.Services;

namespace Altair.BBS.Service.Data.Events;

public class TcpServerEvents
{
    public delegate void ClientConnectionEventHandler(TcpService server, TcpClientInfo client);
    public delegate void ClientDataEventHandler(TcpService server, TcpClientInfo client, byte[] data);
}