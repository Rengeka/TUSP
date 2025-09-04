using System.Net;
using System.Net.Sockets;
using TUSP.Domain;

namespace TUSP.Server;

public class TuspListener
{
    // TODO make async
    public void StartTestListening()
    {
        int listenPort = 5000;
        using (UdpClient udpClient = new UdpClient(listenPort))
        {
            Console.WriteLine($"[Server] Listening on port {listenPort}...");

            var dispatcher = new CommandDispatcher(udpClient);

            while (true)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpClient.Receive(ref remoteEP);

                TuspPackage package = data.DeserializeTuspPackage();

                dispatcher.HandleRequest(package, remoteEP);
            }
        }
    }
}