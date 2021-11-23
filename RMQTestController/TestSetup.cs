using System;
using System.Collections.Generic;
using System.Text;

namespace RMQTestController
{

    internal class ServerSetup 
    {
        public string HostName { get; set; } = "localhost";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public int? Port { get; set; }
    }
    internal class TestParameters 
    {
        public int Repeats { get; set; } = 1;
        public int NumSenders { get; set; } = 1;
        public int NumListeners { get; set; } = 1;
        public int NumQueues { get; set; } = 1;
        public int MessageSizeKb { get; set; } = 128;
        public int NumMessagesToSend { get; set; } = 1024;
    }

    internal class MessageParameters
    {
        public string ListenToQueue { get; set; } = "oh.yes.dis.dashit";
        public string SendToExchange { get; set; } = "Parows.exchange";
        public string RoutingKey { get; set; } = "Parows_Key";
    }

    internal class TestSetup
    {
        public string ConfigurationFile{ get; set; }
        public ServerSetup ServerSetup { get; set; } = new ServerSetup();
        public TestParameters TestParameters { get; set; } = new TestParameters();
        public MessageParameters MessageParameters { get; set; } = new MessageParameters();
    }
}
