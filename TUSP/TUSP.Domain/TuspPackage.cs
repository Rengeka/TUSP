using System.Text;
using TUSP.Server.Enums;

namespace TUSP.Domain;

public class TuspPackage
{
    public uint SessionId { get; set; } 
    public TuspMessageType MessageType { get; set; }
    public uint SequenceNumber { get; set; }
    public byte[] Payload { get; set; } = [];
    public uint PayloadLength => (uint)Payload.Length;
    public Dictionary<string, string> Headers { get; set; } = new();
}

public static class TuspPackageExtensions
{
    public static byte[] Serialize(this TuspPackage package)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        writer.Write(package.SessionId);
        writer.Write((int)package.MessageType);
        writer.Write(package.SequenceNumber);
        writer.Write(package.Headers.Count);

        int headerSectionLength = 0;
        foreach (var header in package.Headers)
        {
            var keyBytes = Encoding.UTF8.GetBytes(header.Key);
            var valueBytes = Encoding.UTF8.GetBytes(header.Value);

            headerSectionLength += 2 + keyBytes.Length;  
            headerSectionLength += 2 + valueBytes.Length; 
        }

        writer.Write(headerSectionLength);

        foreach (var header in package.Headers)
        {
            var keyBytes = Encoding.UTF8.GetBytes(header.Key);
            var valueBytes = Encoding.UTF8.GetBytes(header.Value);

            writer.Write((ushort)keyBytes.Length);
            writer.Write(keyBytes);

            writer.Write((ushort)valueBytes.Length);
            writer.Write(valueBytes);
        }

        writer.Write(package.PayloadLength);
        writer.Write(package.Payload);

        return ms.ToArray();
    }

    public static TuspPackage DeserializeTuspPackage(this byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var reader = new BinaryReader(ms);

        var package = new TuspPackage();

        package.SessionId = reader.ReadUInt32();
        package.MessageType = (TuspMessageType)reader.ReadInt32();
        package.SequenceNumber = reader.ReadUInt32();

        int headerCount = reader.ReadInt32();
        int headerSectionLength = reader.ReadInt32();

        package.Headers = new Dictionary<string, string>(headerCount);
        for (int i = 0; i < headerCount; i++)
        {
            ushort keyLength = reader.ReadUInt16();
            string key = Encoding.UTF8.GetString(reader.ReadBytes(keyLength));

            ushort valueLength = reader.ReadUInt16();
            string value = Encoding.UTF8.GetString(reader.ReadBytes(valueLength));

            package.Headers[key] = value;
        }

        uint payloadLength = reader.ReadUInt32();
        package.Payload = reader.ReadBytes((int)payloadLength);

        return package;
    }
}