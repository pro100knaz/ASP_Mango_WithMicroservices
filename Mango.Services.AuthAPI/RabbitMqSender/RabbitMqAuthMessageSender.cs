using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Mango.Services.AuthAPI.RabbitMqSender
{
	public class RabbitMqAuthMessageSender : IRabbitMqAuthMessageSender
	{
		private readonly string _hostName;
		private readonly string _userName;
		private readonly string _password;


		private IConnection connection;

        public RabbitMqAuthMessageSender()
        {
			_hostName = "localhost";
			_userName = "guest";
			_password = "guest";
        }

        public void SendMessage(object message, string queueName)
		{
			var factory = new ConnectionFactory
			{
				HostName = _hostName,
				UserName = _userName,
				Password = _password
			};

			connection = factory.CreateConnection();

			//establish a channeel for that connection (to pass any message)
			using var channel = connection.CreateModel();

			//channel.QueueDeclare(queueName); // обьявляется очередь
			channel.QueueDeclare(queueName, false,false,false,null);

			var json = JsonConvert.SerializeObject(message);
			var body  = Encoding.UTF8.GetBytes(json);

			channel.BasicPublish(exchange: "", routingKey: queueName, null,  body: body);



		}
	}
}
