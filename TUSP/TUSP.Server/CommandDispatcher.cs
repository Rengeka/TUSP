using System.Net;
using System.Net.Sockets;
using System.Text;
using TUSP.Domain;
using TUSP.Server.Enums;

namespace TUSP.Server;

internal class CommandDispatcher
{
    private readonly UdpClient _udpClient;

    public CommandDispatcher(UdpClient udpClient)
    {
        _udpClient = udpClient;
    }

    public void HandleRequest(TuspPackage package, IPEndPoint ep)
    {
        switch (package.MessageType)
        {
            case TuspMessageType.Ping:
                {
                    HandlePing(package, ep);
                    break;
                }
            case TuspMessageType.Init:
                {
                    HandleInit(package, ep);
                    break;
                }
        }
    }

    private void HandlePing(TuspPackage package, IPEndPoint remoteEP)
    {
        Console.WriteLine($"[Server] Ping received from host={remoteEP.Address}, Port={remoteEP.Port}");

        string response = "Hello from server";
        byte[] responseData = Encoding.UTF8.GetBytes(response);

        var responsePackage = new TuspPackage()
        {
            MessageType = TuspMessageType.Pong,
            Payload = responseData,
            SessionId = 0,
            SequenceNumber = 0,
            Headers = []
        };

        var byteRsponse = responsePackage.Serialize();

        _udpClient.Send(byteRsponse, byteRsponse.Length, remoteEP);
    }  

    private void HandleInit(TuspPackage package, IPEndPoint remoteEP)
    {
        Console.WriteLine($"[Server] Init received from host={remoteEP.Address}, Port={remoteEP.Port}");

        string response = "SecretKeyUnDosTre!";
        byte[] responseData = Encoding.UTF8.GetBytes(response);

        var responsePackage = new TuspPackage()
        {
            MessageType = TuspMessageType.Ack,
            Payload = responseData,
            SessionId = 0,
            SequenceNumber = 0,
            Headers = []
        };

        var byteRsponse = responsePackage.Serialize();

        _udpClient.Send(byteRsponse, byteRsponse.Length, remoteEP);
    }
}