namespace Mango.Services.AuthAPI.RabbitMqSender
{
	public interface IRabbitMqAuthMessageSender
	{
		void SendMessage(object message, string queueName);
	}
}
