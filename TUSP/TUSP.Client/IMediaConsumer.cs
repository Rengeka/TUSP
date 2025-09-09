namespace TUSP.Domain;

public interface IMediaConsumer
{
    public void StartConsuming();
    public void AddSegment(byte[] segment);
}