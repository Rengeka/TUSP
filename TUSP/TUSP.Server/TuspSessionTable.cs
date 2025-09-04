namespace TUSP.Server;

internal class TuspSessionTable
{
    private readonly List<TuspSession> _sessions = new();

    public void AddSession(TuspSession session)
    {
        _sessions.Add(session);
    }
}