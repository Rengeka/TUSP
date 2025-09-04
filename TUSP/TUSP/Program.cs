using TUSP.Client;
using TUSP.Server;

var tuspCient = new TuspClient();
var tuspServer = new TuspListener();

var thread1 = new Thread(() => tuspServer.StartTestListening());
thread1.Start();

var sendInterval = TimeSpan.FromSeconds(1);


tuspCient.Ping("localhost", 5000);
tuspCient.Init("localhost", 5000);