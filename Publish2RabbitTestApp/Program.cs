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
                directmessages.SendMessage(message);                
            }            
        }
    }

    

    public class Directmessages
    {

        private const string UName = "guest";
        private const string PWD = "guest";
        private const string HName = "localhost";
        public void SendMessage(string message)

        {
            //Main entry point to the RabbitMQ .NET AMQP client
            var connectionFactory = new ConnectionFactory()
            {
                UserName = UName,
                Password = PWD,
                HostName = HName,
            };
            using (var connection = connectionFactory.CreateConnection())
            {
                using (var model = connection.CreateModel())
                {
                    var properties = model.CreateBasicProperties();
                    properties.Persistent = false;
                    byte[] messagebuffer = Encoding.Default.GetBytes(message);
                    //model.BasicPublish("amq.direct", "messages.to.me", properties, messagebuffer);
                    model.BasicPublish("amq.topic", "mytopic", properties, messagebuffer);
                }
            }
            Console.WriteLine("Message Sent");
        }
    }
}
