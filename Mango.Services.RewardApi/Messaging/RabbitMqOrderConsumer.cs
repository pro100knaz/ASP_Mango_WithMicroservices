
using Mango.Services.RewardApi.MEssages;
using Mango.Services.RewardApi.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.RewardApi.Messaging
{
	public class RabbitMqOrderConsumer : BackgroundService
	{
		private readonly IConfiguration configuration;
		private readonly RewardService rewardService;
		private IConnection connection;
		private IModel channel;
		private readonly string orderCreatedTopic;
		private readonly string orderCreatedEmailMessage;
		private readonly string queueName="";
		public RabbitMqOrderConsumer(IConfiguration configuration, RewardService rewardService)
        {
			this.configuration = configuration;
			this.rewardService = rewardService;


			var factory = new ConnectionFactory
			{
				HostName = "localhost",
				Password = "guest",
				UserName = "guest"
			};
			connection = factory.CreateConnection();
			channel = connection.CreateModel();

			orderCreatedTopic = configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
			//orderCreatedEmailMessage = configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");

			channel.ExchangeDeclare(orderCreatedTopic, ExchangeType.Fanout, durable:false);

			queueName = channel.QueueDeclare().QueueName;
			channel.QueueBind(queueName, orderCreatedTopic, "");

		}
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{

			stoppingToken.ThrowIfCancellationRequested();

			var consumer = new EventingBasicConsumer(channel);
			consumer.Received += async (chan, eventArgs) =>
			{
				var content = Encoding.UTF8.GetString(eventArgs.Body.ToArray()); // it is email is string

				RewardsMessages message = JsonConvert.DeserializeObject<RewardsMessages>(content);
				 HandleMessage(message).GetAwaiter().GetResult();

				//message weas received and consumed we have to notify about it
				channel.BasicAck(eventArgs.DeliveryTag, false);

			};


			channel.BasicConsume(queueName, false, consumer);

			return Task.CompletedTask;

		}


		private async Task HandleMessage(RewardsMessages message)
		{

			rewardService.UpdateRewards(message).GetAwaiter().GetResult();

		}

	}
}
