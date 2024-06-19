namespace Mango.Services.ShopingCartApi.RabbitMqSender
{
	public interface IRabbitMqCartMessageSender
	{
		void SendMessage(object message, string queueName);
	}
}
