using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BusBoy.RabbitMQ.RPC
{
    public class RpcServer
    {
        public delegate string CallbackDelegate(string message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostName">Host name for the rabbit queue</param>
        /// <param name="callback">string abc123(string message)</param>
        public void Start(string hostName, CallbackDelegate callback)
        {
            //setup connection
            var factory = new ConnectionFactory { HostName = hostName };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            //declare queue
            channel.QueueDeclare(queue: "rpc_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: "rpc_queue",
                                 autoAck: false,
                                 consumer: consumer);

            //setup consumer
            consumer.Received += (model, ea) =>
            {
                string response = string.Empty;

                //decode incoming message
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {
                    //do work on message
                    var message = Encoding.UTF8.GetString(body);
                    int n = int.Parse(message);

                    //Use call back to get message
                    response = callback(message);
                }
                catch (Exception e)
                {
                    response = e.ToString();
                }
                finally
                {
                    //respond to message
                    var responseBytes = Encoding.UTF8.GetBytes(response);

                    channel.BasicPublish(exchange: string.Empty,
                                         routingKey: props.ReplyTo,
                                         basicProperties: replyProps,
                                         body: responseBytes);

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };
        }
    }
}
