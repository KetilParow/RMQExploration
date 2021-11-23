using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SubscribeToTopicTestApp
{
    class Program
    {
        private const string UName = "guest";
        private const string Pwd = "guest";
        private const string HName = "localhost";
        private const string VHost = "/";

        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { 
                HostName = HName,
                UserName = UName,
                Password = Pwd,
                VirtualHost = VHost
            };
            new MessageConsumer(factory, args).ConsumeMessages();
        }
    }

    internal class MessageConsumer
    {
        private ConnectionFactory factory;
        private string[] args;

        public MessageConsumer(ConnectionFactory factory, string[] args)
        {
            this.factory = factory;
            this.args = args;
        }
        public void ConsumeMessages()
        {
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //channel.ExchangeDeclare(exchange: "Parows.topics", type: "topic");
                //var queueName = channel.QueueDeclare().QueueName;
                var queueName = "broadcast.mytopic";

                //if (args.Length < 1)
                //{
                //    Console.Error.WriteLine("Usage: {0} [binding_key...]",
                //                            Environment.GetCommandLineArgs()[0]);
                //    Console.WriteLine(" Press [enter] to exit.");
                //    Console.ReadLine();
                //    Environment.ExitCode = 1;
                //    return;
                //}

                //foreach (var bindingKey in args)
                //{
                channel.QueueDeclare(queue: queueName, durable: true, exclusive:false, autoDelete: false, arguments: new Dictionary<string,object>{ { "x-queue-type","quorum"} });
                //channel.QueueBind(queue: queueName
                //                  , exchange: "amq.topic"
                //                  , routingKey: "mytopic" //bindingKey
                //                  );
                //}

            Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var routingKey = ea.RoutingKey;
                    Console.WriteLine(" [x] Received '{0}':'{1}'",
                                      routingKey,
                                      message);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
