using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace RMQTestController
{
    internal class SenderClient :IDisposable
    {
        private TestSetup _testSetup;
        private Action _afterSend;
        private IConnection _connection;

        public SenderClient(TestSetup testSetup, Action afterSend = null)
        {
            _testSetup = testSetup;
            _afterSend = afterSend?? (() => { /*Do Nuttn*/ });
            var connectionFactory = new ConnectionFactory()
            {
                UserName = _testSetup.ServerSetup.UserName,
                Password = _testSetup.ServerSetup.Password,
                HostName = _testSetup.ServerSetup.HostName
            };

            _connection = connectionFactory.CreateConnection();
        }

        private Task beforeSendDummy()
        {
            return Task.Run(() => {/*Donothing*/});
        }

        public void Dispose()
        {
            try { _connection.Close(); } catch { }
            _connection.Dispose();
        }

        public void SendTestMessages()
        {
            Directmessages directmessages = new Directmessages(_testSetup.MessageParameters, _connection);
            Random rnd = new Random();
            for (int i = 0; i < _testSetup.TestParameters.NumMessagesToSend; i++)
            {
                var payload = new byte[_testSetup.TestParameters.MessageSizeKb*1024];
                rnd.NextBytes(payload);
                directmessages.SendMessage(payload);
                _afterSend();
            }
        }
    }
    internal class Directmessages
    {        
        private readonly MessageParameters _messageParameters;
        private readonly IConnection _connection;
        public Directmessages(MessageParameters messageParameters, IConnection connection)
        {
            _messageParameters = messageParameters;
            _connection = connection;
        }
        public void SendMessage(byte[] payload)
        {
            using (var model = _connection.CreateModel()) {
                var properties = model.CreateBasicProperties();
                properties.Persistent = false;
                //Main entry point to the RabbitMQ .NET AMQP client           
                model.BasicPublish(_messageParameters.SendToExchange, _messageParameters.RoutingKey, properties, payload);
            }
            
            //Console.WriteLine("Message Sent");
        }
    }
}

