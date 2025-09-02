using TUSP.Client;
using TUSP.Server;

var tuspLCient = new TuspClient();
var tuspServer = new TuspListener();

var thread1 = new Thread(() => tuspServer.StartTestListening());
thread1.Start();

var sendInterval = TimeSpan.FromSeconds(1);

while (true)
{
    tuspLCient.SendTestMessage();
    Thread.Sleep(sendInterval); 
}