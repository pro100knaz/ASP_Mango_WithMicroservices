
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
	public class RabbitMqAuthConsumer : BackgroundService
	{
		private readonly IConfiguration configuration;
		private readonly EmailService emailService;
		private IConnection connection;
		private IModel channel;
		private readonly string registerEmailQueue;
		public RabbitMqAuthConsumer(IConfiguration configuration, EmailService emailService)
		{
			this.configuration = configuration;
			this.emailService = emailService;


			var factory = new ConnectionFactory
			{
				HostName = "localhost",
				Password = "guest",
				UserName = "guest"
			};
			connection = factory.CreateConnection();
			channel = connection.CreateModel();

			registerEmailQueue = configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");

			channel.QueueDeclare(registerEmailQueue, false, false, false, null);



		}
		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{

			stoppingToken.ThrowIfCancellationRequested();

			var consumer = new EventingBasicConsumer(channel);
			consumer.Received += async (chan, eventArgs) =>
			{
				var content = Encoding.UTF8.GetString(eventArgs.Body.ToArray()); // it is email is string

				string email = JsonConvert.DeserializeObject<string>(content);
				HandleMessage(email).GetAwaiter().GetResult();

				//message weas received and consumed we have to notify about it
				channel.BasicAck(eventArgs.DeliveryTag, false);

			};


			channel.BasicConsume(registerEmailQueue, false, consumer);

			return Task.CompletedTask;

		}


		private async Task HandleMessage(string email)
		{

			emailService.RegisterUserEmailLog(email).GetAwaiter().GetResult();

		}

	}
}
