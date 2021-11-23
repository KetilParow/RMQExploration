using RabbitMQ.Client;
using System;
using System.Text;

namespace Publish2RabbitTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            SendSomeRubbish();
        }
        public static void SendSomeRubbish()
        {
            Directmessages directmessages = new Directmessages();

            while (true)
            {
                Console.Write("Enter a message to send: ");
                var message = Console.ReadLine();
                var routingKey = "";
                var exchange = "";
                if (string.IsNullOrWhiteSpace(message))
                {
                    Console.Write("Press Q to quit");
                    char inp = Console.ReadKey().KeyChar;
                    if (inp == 'Q' || inp == 'q')
                    {
                        return;
                    }
                    Console.WriteLine();
                    continue;
                }
                if (message.Split(':').Length == 3)
                {
                    exchange = message.Split(':')[0];
                    message = message.Split(':')[1] + ":" + message.Split(':')[2];
                }
                if (message.Split(':').Length == 2)
                {
                    routingKey = message.Split(':')[0];
                    message = message.Split(':')[1];
                }
                directmessages.SendMessage(message, routingKey, exchange);
            }            
        }
    }

    public class Directmessages
    {

        private const string UName = "guest";
        private const string PWD = "guest";
        private const string HName = "localhost";
        private const string vHost = "shovel-entry";
        public void SendMessage(string message, string routingKey, string exchange="amqp.topic")
        {
            //Main entry point to the RabbitMQ .NET AMQP client
            var connectionFactory = new ConnectionFactory()
            {
                UserName = UName,
                Password = PWD,
                HostName = HName,
                VirtualHost = vHost
            };
            using (var connection = connectionFactory.CreateConnection())
            {
                using (var model = connection.CreateModel())
                {
                    var properties = model.CreateBasicProperties();
                    properties.Persistent = false;
                    byte[] messagebuffer = Encoding.Default.GetBytes(message);
                    model.BasicPublish(exchange, routingKey, properties, messagebuffer);
                    //model.BasicPublish("amq.topic", "mytopic", properties, messagebuffer);
                }
            }
            Console.WriteLine("Message Sent");
        }
    }
}
