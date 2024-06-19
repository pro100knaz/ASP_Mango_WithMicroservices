using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Mango.Services.OrderAPI.RabbitMqSender
{
	public class RabbitMqOrderMessageSender : IRabbitMqOrderMessageSender
	{
		private readonly string _hostName;
		private readonly string _userName;
		private readonly string _password;


		private IConnection connection;

		public RabbitMqOrderMessageSender()
		{
			_hostName = "localhost";
			_userName = "guest";
			_password = "guest";
		}

		public void SendMessage(object message, string exchangeName)
		{			
			if (ConnectionExist())
			{

				using var channel = connection.CreateModel();

				//channel.QueueDeclare(queueName); // обьявляется очередь
				//channel.QueueDeclare(queueName, false, false, false, null);

				//There we need fanout or topic 
				//durable - if application will restart than that exchange well remain
				channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, durable: false);


				var json = JsonConvert.SerializeObject(message);
				var body = Encoding.UTF8.GetBytes(json);

				channel.BasicPublish(exchange: exchangeName, "", null, body: body);


			}
		}

		private void CreateConnection()
		{
			try
			{
				var factory = new ConnectionFactory
				{
					HostName = _hostName,
					UserName = _userName,
					Password = _password
				};


				connection = factory.CreateConnection();
			}
			catch (Exception ex)
			{

				throw;
			}
		}


		private bool ConnectionExist()
		{
			if (connection != null)
			{
				return true;
			}
			CreateConnection();
			return true;
		}

	}
}
