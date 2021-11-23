using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RMQTestController
{
    static class Program
    {

        private const string defaultConfig = "testsetup.json";

        private static List<SenderClient> _senders = new List<SenderClient>();
        private static List<ReceiverClient> _receivers = new List<ReceiverClient>();
        private static TestSetup _testSetup;
        static int _messagesReceived = 0;
        static int _messagesSent = 0;
        static int _bytesReceived = 0;
        static private readonly object _lockObj = new object();
        static int _cursorTop = 0;
        static int consWidth = Console.WindowWidth;
        static (ConsoleColor, ConsoleColor) OriginalConsColors;
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello fools!");

            if (args == null || args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                args = new string[] { defaultConfig };
            }
            _testSetup = ReadSetup(args[0]);
            OriginalConsColors = (Console.BackgroundColor, Console.ForegroundColor);
            StartReceivers(_testSetup);
            StartSenders(_testSetup);

            for (int i = 0; i < _testSetup.TestParameters.Repeats;i++)
            {
                Console.WriteLine($"Running {i + 1} of {_testSetup.TestParameters.Repeats} repititions");
                _cursorTop = Console.CursorTop;
                await SendAllTheMessages();
            }
            
            while (true) 
            {
                //Console.Write("");
                if (_messagesSent <= _messagesReceived)
                {
                    break;
                }
                //Console.Write($" {_messagesSent - _messagesReceived} awaits delivery ");
                await Task.Delay(500);
                //Console.Write("");
            }
            Console.WriteLine($"\n{_messagesSent} sent.\n{_messagesReceived} received ({_bytesReceived / 1024} KBytes).\n");
            Console.WriteLine("Press any key to end testrun");
            Console.ReadKey();
        }

        private async static Task SendAllTheMessages()
        {
            await Task.Run(() => { Parallel.ForEach(_senders, s => { s.SendTestMessages(); }); }) ;
        }

        private static void StartReceivers(TestSetup testSetup)
        {
            var connFactory = new ConnectionFactory
            {
                HostName = testSetup.ServerSetup.HostName,
                UserName = testSetup.ServerSetup.UserName,
                Password = testSetup.ServerSetup.Password
            };
            for(int i = 0; i < testSetup.TestParameters.NumListeners;i++) 
            {
                _receivers.Add(new ReceiverClient(connFactory, testSetup.MessageParameters, ReceiverGotMessage));
            }
            Console.WriteLine($"{_receivers.Count} message receivers started.");
        }

        private static void StartSenders(TestSetup testSetup)
        {
            for (int i = 0; i < testSetup.TestParameters.NumSenders; i++)
            {
                _senders.Add(new SenderClient(testSetup, SenderSentMessage));
            }
            Console.WriteLine($"{_senders.Count} message senders started.");
        }
        private static void ReceiverGotMessage(IEnumerable<byte> payload)
        {
            lock (_lockObj)
            {
                _messagesReceived++;
                _bytesReceived += payload.Count();
            }
            OutputVisuals();
        }

        private static void SenderSentMessage()
        {
            lock (_lockObj) 
            { 
                _messagesSent++;                 
            }            
            OutputVisuals();
        }

        private static void OutputVisuals()
        {
            lock (_lockObj) {
                var totalLen = (int) ((double) _messagesSent / (_testSetup.TestParameters.NumMessagesToSend * _testSetup.TestParameters.NumSenders) * consWidth) ;
                var part = (double) _messagesReceived / Math.Max(_messagesSent,1);
                var readStrLen = (int) (part * totalLen);
                var sentNotReadStrLen = Math.Max(totalLen - readStrLen, 0);
                var sentNotReadStr = new string('+', sentNotReadStrLen);
                var readStr = new string('+', readStrLen);
            
            //await Task.Run(() =>
            //{
                Console.SetCursorPosition(0, _cursorTop);
                Console.Write(readStr);
                Console.BackgroundColor = OriginalConsColors.Item2;
                Console.ForegroundColor = OriginalConsColors.Item1;
                Console.Write(sentNotReadStr);
                Console.BackgroundColor = OriginalConsColors.Item1;
                Console.ForegroundColor = OriginalConsColors.Item2;
                //});
            }
        }

       
        private static TestSetup ReadSetup(string confFile)
        {
            Console.WriteLine("Reading test setup for rabbit MQ tests...");
            confFile = Path.Combine(new FileInfo(typeof(TestSetup).Assembly.Location).DirectoryName, confFile);
            if (!File.Exists(confFile)) 
            {
                return new TestSetup();
            }
            TestSetup testSetup = JsonConvert.DeserializeObject<TestSetup>(File.ReadAllText(confFile));
            testSetup.ConfigurationFile = confFile;

            Console.WriteLine(string.IsNullOrWhiteSpace(testSetup.ConfigurationFile)
                ? "No configuration file name supplied, or file not found."
                : $"Test setup read from {testSetup.ConfigurationFile}.");
            Console.WriteLine
                (
                $"------------------------------------------------------------\n" +
                $"{nameof(testSetup.ServerSetup.HostName)}:\t{testSetup.ServerSetup.HostName}\n" +
                $"{nameof(testSetup.ServerSetup.Port)}:\t{(testSetup.ServerSetup.Port.HasValue ? testSetup.ServerSetup.Port.ToString() : "(default)")}\n" +
                $"{nameof(testSetup.TestParameters.NumSenders)}:\t{testSetup.TestParameters.NumSenders}\n" +
                $"{nameof(testSetup.TestParameters.NumListeners)}:\t{testSetup.TestParameters.NumListeners}\n" +
                $"{nameof(testSetup.TestParameters.MessageSizeKb)}:\t{testSetup.TestParameters.MessageSizeKb}\n" +
                $"{nameof(testSetup.TestParameters.NumQueues)}:\t{testSetup.TestParameters.NumQueues}\n" +
                $"------------------------------------------------------------\n"
                );

            return testSetup;
        }
    }
}
