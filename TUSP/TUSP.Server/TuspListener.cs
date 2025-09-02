using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TUSP.Server;

public class TuspListener
{
    public void StartTestListening()
    {
        int listenPort = 5000;
        using (UdpClient udpServer = new UdpClient(listenPort))
        {
            Console.WriteLine($"[Server] Listening on port {listenPort}...");

            while (true)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpServer.Receive(ref remoteEP);
                string message = Encoding.UTF8.GetString(data);

                Console.WriteLine($"[Server] Received from {remoteEP}: {message}");

                string response = "Hello from server";
                byte[] responseData = Encoding.UTF8.GetBytes(response);
                udpServer.Send(responseData, responseData.Length, remoteEP);
            }
        }
    }
}