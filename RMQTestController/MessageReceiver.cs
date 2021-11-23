using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMQTestController
{
    public class MessageReceiver : DefaultBasicConsumer
    {
        private readonly IModel _amqpModel;
        private Action<IEnumerable<byte>> _payloadReceived;
        public MessageReceiver(IModel channel, Action<IEnumerable<byte>> payloadReceived) : base()
        {
            _amqpModel = channel;
            _payloadReceived = payloadReceived;
        }
        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            //Console.WriteLine($"\nConsuming Message");
            //Console.WriteLine(string.Concat("Message received from the exchange ", exchange));
            //Console.WriteLine(string.Concat("Consumer tag: ", consumerTag));
            //Console.WriteLine(string.Concat("Delivery tag: ", deliveryTag));
            //Console.WriteLine(string.Concat("Routing tag: ", routingKey));
            var payload = body.ToArray();
            _amqpModel.BasicAck(deliveryTag, true);
            _payloadReceived(payload);
        }
    }
}
