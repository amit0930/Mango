using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private string connectionString = "";
        public async Task PublishMessage(object message, string topic_queue_name)
        {
            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(topic_queue_name);
            var jsonMessage=JsonConvert.SerializeObject(message);
            ServiceBusMessage finalSBMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId=Guid.NewGuid().ToString(),
            };

            await sender.SendMessageAsync(finalSBMessage);
            await client.DisposeAsync();
        }
    }
}
