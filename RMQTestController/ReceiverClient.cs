using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace RMQTestController
{
    internal class ReceiverClient : IDisposable
    {
        private readonly ConnectionFactory _connFactory;
        private readonly IConnection _connection;
        private readonly IModel _model;
        private readonly MessageParameters _messageParameters;

        public ReceiverClient(ConnectionFactory connFactory, MessageParameters messageParameters, Action<IEnumerable<byte>> _payloadReceived) 
        {
            _connFactory = connFactory;
            _connection = _connFactory.CreateConnection();
            _model = _connection.CreateModel();            
            _messageParameters = messageParameters;
            _model.BasicQos(0, 1, false);
            _model.BasicConsume(_messageParameters.ListenToQueue, false, new MessageReceiver(_model, _payloadReceived));
        }

        public void Dispose()
        {
            try { _model.Close(); } catch { }
            _model.Dispose();
            try { _connection.Close(); } catch { }
            _connection.Dispose();
        }
    }
}
