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
		//кончно лучше внутри файла конфигуурации
		private const string connectionString = "Endpoint=sb://mangowebtestmd.servicebus.windows.net/;SharedAccessKeyName=DefaultSharedAccessKey;SharedAccessKey=MRySQaRpji8CBkLOAppMwkdQRaslve21b+ASbO83Cj8=;EntityPath=emailshoppingcart";
		public async Task PublishMessage(object message, string topic_queue_Name)
		{
			await using var client = new ServiceBusClient(connectionString);

			ServiceBusSender sender = client.CreateSender(topic_queue_Name);

			var jsonMessage = JsonConvert.SerializeObject(message);

			ServiceBusMessage fileMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
			{
				CorrelationId = Guid.NewGuid().ToString(),
			};

			await sender.SendMessageAsync(fileMessage);

			await client.DisposeAsync();
		}
	}
}
