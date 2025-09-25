using TUSP;
using TUSP.Client;
using TUSP.Server;

var tuspCient = new TuspClient();
var tuspServer = new TuspListener();
var mediaPlayer = new TestMediaConsumer();

var thread1 = new Thread(tuspServer.StartListening);
thread1.Start();

var sendInterval = TimeSpan.FromSeconds(1);

tuspCient.Ping("localhost", 5000);
tuspCient.Init("localhost", 5000);
//tuspCient.StartTestVideoStream("localhost", 5000);
tuspCient.StartVideoStream("localhost", 5000, mediaPlayer);
