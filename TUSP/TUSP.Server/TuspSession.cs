namespace TUSP.Server;

// TOOD Secure and Insecure Sessions
// Use HMAC-signed ID
internal class TuspSession
{
    public uint SessionId { get; set; }
    public string ClientHost {  get; set; }
}