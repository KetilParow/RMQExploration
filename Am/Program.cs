using Amqp;
using Amqp.Framing;
using System;
using System.Text;

namespace Am
{
    public class Program
    {
        static void Main(string[] args)
        {           
            DoDaStuff("Is dis coming thru");
            Console.ReadKey();
        }

        private static void DoDaStuff(string inputMessage)
        {
            try
            {
                var message = new Message() { BodySection = new Data() { Binary = Encoding.UTF8.GetBytes(inputMessage) } };
                Address address = new Address("localhost", 5672, "guest", "guest", "/", "AMQP");
                Connection connection = new ConnectionFactory().CreateAsync(address).ConfigureAwait(false).GetAwaiter().GetResult();
                new SenderLink(new Session(connection), "sender-link", "q1").Send(message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
