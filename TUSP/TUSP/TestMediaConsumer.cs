using System.Collections.Concurrent;
using TUSP.Domain;

namespace TUSP;

internal class TestMediaConsumer : IMediaConsumer
{
    private readonly ConcurrentQueue<byte[]> _segmentQueue = new();
    private readonly CancellationTokenSource _cts = new();
    private Task? _playbackTask;

    public void AddSegment(byte[] segment)
    {
        _segmentQueue.Enqueue(segment);
    }

    public void StartConsuming()
    {
        if (_playbackTask != null) return; 

        _playbackTask = Task.Run(async () =>
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                if (_segmentQueue.TryDequeue(out var segment))
                {
                    Console.WriteLine($"[MediaPlayer] Playing segment, size={segment.Length} bytes");
                    await Task.Delay(100); 
                }
                else
                {
                    Console.WriteLine($"[MediaPlayer] Waiting for next segment");
                    await Task.Delay(2000);
                }
            }
        }, _cts.Token);
    }

    public void Stop()
    {
        _cts.Cancel();
        _playbackTask?.Wait();
    }
}