using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static ISubscriptionClient subscriptionClient;

        static void Main(string[] args)
        {
            Console.WriteLine("PaymentSucess - Listner started.");
            MainAsync().GetAwaiter().GetResult();
        } 

        static async Task MainAsync()
        {
            string ServiceBusConnectionString = "Endpoint=sb://shebintest.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=dz8LSGVE7nlhgqqNOxCb0uILTq128BkA0PAGL/mW6do=";
            string TopicName = "sales";
            string SubscriptionName = "PaymentSucess"; // "UpdateStock";

            subscriptionClient = new SubscriptionClient(ServiceBusConnectionString, TopicName, SubscriptionName);

            // Register subscription message handler and receive messages in a loop.
            RegisterOnMessageHandlerAndReceiveMessages();

            Console.ReadKey();

            await subscriptionClient.CloseAsync();
        }

        static void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler);

            // Register the function that processes messages.
            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var messageBody = Encoding.UTF8.GetString(message.Body);

            //var serviceBusMessage = JsonConvert.DeserializeObject<ServiceBusMessage>(messageBody);

            Console.WriteLine($"Received message: UserInfo:{Encoding.UTF8.GetString(message.Body)}");

            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var exception = exceptionReceivedEventArgs.Exception;

            return Task.CompletedTask;
        }

        public class ServiceBusMessage
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string Content { get; set; }
        }
    }
}
