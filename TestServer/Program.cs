using TestServer;

BusBoy.RabbitMQ.RPC.RpcServer server = new BusBoy.RabbitMQ.RPC.RpcServer();
Core core = new Core();
server.Start("localHost", core.DoWork);

Console.WriteLine("Press any key to exit.");
Console.ReadLine();
