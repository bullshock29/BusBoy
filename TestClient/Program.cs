BusBoy.RabbitMQ.RPC.RpcClient client = new BusBoy.RabbitMQ.RPC.RpcClient("localhost");
string ret = await client.CallAsync("hello world");

Console.WriteLine("Press any key to exit.");
Console.ReadLine();
