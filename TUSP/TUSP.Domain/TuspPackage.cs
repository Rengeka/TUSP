namespace TUSP.Domain;

public class TuspPackage
{
    public int Id { get; set; } 
    public byte[] Payload { get; set; }
    public int PayloadLength => Payload.Length;
    public Dictionary<string, byte[]> Headers { get; set; }
}