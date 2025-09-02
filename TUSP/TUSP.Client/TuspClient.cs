using System.Net.Sockets;
using System.Text;
using TUSP.Domain;

namespace TUSP.Client;

public class TuspClient
{
    public void SendTestMessage()
    {
        var updClient = new UdpClient();

        var package = new TuspPackage()
        {
            Id = 1,
            Payload = Encoding.UTF8.GetBytes("Hello!"),
            Headers =
            [
                
            ]
        };

        var byteMessage = SerializePackage(package);

        updClient.Send(byteMessage, byteMessage.Length, "localhost" /*"127.0.0.1"*/, 5000);
        Console.WriteLine("Message was sent");
    }

    private byte[] SerializePackage(TuspPackage package)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        writer.Write(package.Id);

        writer.Write(package.Headers.Count());

        foreach (var header in package.Headers)
        {
            writer.Write(header.Key);                  
            writer.Write((ushort)header.Value.Length);  
            writer.Write(header.Value);                
        }

        writer.Write(package.PayloadLength);

        writer.Write(package.Payload);

        return ms.ToArray();
    }
}