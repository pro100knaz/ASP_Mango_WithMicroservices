namespace Mango.Services.OrderAPI.RabbitMqSender
{
	public interface IRabbitMqOrderMessageSender
	{
		void SendMessage(object message, string exchangeName);
	}
}
