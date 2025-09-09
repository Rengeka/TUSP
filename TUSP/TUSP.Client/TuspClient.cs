using System.Net;
using System.Net.Sockets;
using System.Text;
using TUSP.Domain;

namespace TUSP.Client;

public class TuspClient
{
    private readonly UdpClient _udpClient;

    public TuspClient()
    {
        _udpClient = new UdpClient();

    }

    public void Ping(string remoteHost, int remotePort)
    {
        var package = new TuspPackage()
        {
            SessionId = 0,
            Payload = [],
            MessageType = Server.Enums.TuspMessageType.Ping,
            SequenceNumber = 0
        };

        _udpClient.Client.ReceiveTimeout = 2000;

        for (int i = 1; i <= 4; i++)
        {
            try
            {
                var byteMessage = package.Serialize();
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

                var timestamp = DateTime.Now;
                _udpClient.Send(byteMessage, byteMessage.Length, remoteHost, (int)remotePort);

                byte[] data = _udpClient.Receive(ref remoteEP);
                var elapsed = DateTime.Now - timestamp;

                var responsePackage = data.DeserializeTuspPackage();

                Console.WriteLine($"[Client] Reply {i} from {remoteHost}: time={elapsed.TotalMilliseconds} ms, message='{Encoding.UTF8.GetString(responsePackage.Payload)}'");
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
            {
                Console.WriteLine($"[Client] Reply {i} from {remoteHost}: Request timed out.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Client] Reply {i} from {remoteHost}: Error - {ex.Message}");
            }
        }
    }

    public void Init(string remoteHost, int remotePort)
    {
        var package = new TuspPackage()
        {
            SessionId = 0,
            Payload = [],
            MessageType = Server.Enums.TuspMessageType.Init,
            SequenceNumber = 0
        };

        try
        {
            var byteMessage = package.Serialize();
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            _udpClient.Send(byteMessage, byteMessage.Length, remoteHost, (int)remotePort);

            byte[] data = _udpClient.Receive(ref remoteEP);

            var responsePackage = data.DeserializeTuspPackage(); 

            Console.WriteLine($"[Client] Reply from {remoteHost}: message='{Encoding.UTF8.GetString(responsePackage.Payload)}'");
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
        {
            Console.WriteLine($"[Client] Reply from {remoteHost}: Request timed out.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Client] Reply from {remoteHost}: Error - {ex.Message}");
        }
    }

    public void StartVideoStream(string remoteHost, int remotePort)
    {
        var package = new TuspPackage()
        {
            SessionId = 0,
            Payload = [],
            MessageType = Server.Enums.TuspMessageType.Data,
            SequenceNumber = 0
        };

        try
        {
            var byteMessage = package.Serialize();
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            _udpClient.Send(byteMessage, byteMessage.Length, remoteHost, (int)remotePort);

            ConsumeVideoStream(remotePort);
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
        {
            Console.WriteLine($"[Client] Reply from {remoteHost}: Request timed out.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Client] Reply from {remoteHost}: Error - {ex.Message}");
        }
    }

    private List<byte> _segmentBuffer = new();
    private uint _expectedSequence = 0;

    private void ConsumeVideoStream(int localPort)
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

        while (true)
        {
            try
            {
                byte[] data = _udpClient.Receive(ref remoteEP);
                var package = data.DeserializeTuspPackage();

                if (package.SequenceNumber != _expectedSequence)
                {
                    Console.WriteLine($"[Client] Lost packet: expected={_expectedSequence}, got={package.SequenceNumber}");
                }
                else
                {
                    _expectedSequence++;
                }

                HandleVideoChunk(package);
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
            {
                Console.WriteLine("[Client] Timeout waiting for data");
            }
        }
    }

    private void HandleVideoChunk(TuspPackage package)
    {
        _segmentBuffer.AddRange(package.Payload);

        Console.WriteLine($"[Client] Received segment: {package.PayloadLength} bytes");

        if (package.Headers.TryGetValue("IsLast", out var isLastStr) && bool.Parse(isLastStr))
        {
            byte[] completeSegment = _segmentBuffer.ToArray();
            _segmentBuffer.Clear();

            DecodeSegment(completeSegment);
        }
    }

    private void DecodeSegment(byte[] segment)
    {
        // Use FFmpeg, LibVLC or other decoder
        Console.WriteLine($"[Client] Received complete segment: {segment.Length} bytes");
    }
}