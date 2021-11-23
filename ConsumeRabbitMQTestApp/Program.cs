using RabbitMQ.Client;
using System;
using System.Text;

namespace ConsumeRabbitMQTestApp
{
    class Program
    {
        private const string UName  = "guest";
        private const string Pwd    = "guest";
        private const string HName  = "localhost";

        static void Main(string[] args)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                HostName = HName,
                UserName = UName,
                Password = Pwd,
            };

            using (var connection = connectionFactory.CreateConnection()) 
            {
                using (var channel = connection.CreateModel())
                {
                    channel.BasicQos(0, 1, false);
                    MessageReceiver messageReceiver = new MessageReceiver(channel);
                    channel.BasicConsume("messages.to.me", false, messageReceiver);
                    // accept only one unack-ed message at a time
                    // uint prefetchSize, ushort prefetchCount, bool global
                    PressAnyKey();
                    Console.ReadKey();
                }
            }
        }

        public static void PressAnyKey()
        {
            Console.Write("Press any key to exit");
        }
    }
    public class MessageReceiver : DefaultBasicConsumer
    {
        private readonly IModel _channel;
        public MessageReceiver(IModel channel) :base()
        {
            _channel = channel;
        }
        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            Console.WriteLine($"\nConsuming Message");
            Console.WriteLine(string.Concat("Message received from the exchange ", exchange));
            Console.WriteLine(string.Concat("Consumer tag: ", consumerTag));
            Console.WriteLine(string.Concat("Delivery tag: ", deliveryTag));
            Console.WriteLine(string.Concat("Routing tag: ", routingKey));
            Console.WriteLine(string.Concat("Message: ", Encoding.UTF8.GetString(body.ToArray())));
            _channel.BasicAck(deliveryTag, false);
            Program.PressAnyKey();
        }
    }
}
