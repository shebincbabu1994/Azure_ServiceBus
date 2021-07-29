using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace user_send_microservice
{
    class Program
    {
        static ITopicClient topicClient;
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            string ServiceBusConnectionString = "Endpoint=sb://shebintest.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=dz8LSGVE7nlhgqqNOxCb0uILTq128BkA0PAGL/mW6do=";
            string TopicName = "sales";

            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);
            string lg;
            // Send messages.
            do
            {

                await SendUserMessage();
            }
            while ((lg=Console.ReadLine()) == "1");


            await topicClient.CloseAsync();
        }


        static async Task SendUserMessage()
        {
            List<User> users = GetDummyDataForUser();

            var serializeUser = JsonConvert.SerializeObject(users);

            string messageType = "userData";

            string messageId = Guid.NewGuid().ToString();

            var message = new ServiceBusMessage
            {
                Id = messageId,
                Type = messageType,
                Content = serializeUser
            };

            var serializeBody = JsonConvert.SerializeObject(message);

            // send data to bus

            var busMessage = new Message(Encoding.UTF8.GetBytes(serializeUser));
            busMessage.UserProperties.Add("Type", messageType);
            busMessage.MessageId = messageId;

            await topicClient.SendAsync(busMessage);

            Console.WriteLine("message has been sent");

        }

       public class User
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        public class ServiceBusMessage
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string Content { get; set; }
        }

        private static List<User> GetDummyDataForUser()
        {
            User user = new User();
            List<User> lstUsers = new List<User>();
            for (int i = 1; i < 3; i++)
            {
                user = new User();
                user.Id = i;
                user.Name = "User" + i;

                lstUsers.Add(user);
            }

            return lstUsers;
        }
    }
}
