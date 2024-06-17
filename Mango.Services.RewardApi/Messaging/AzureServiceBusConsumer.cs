using Azure.Messaging.ServiceBus;
using Mango.Services.RewardApi.MEssages;
using Mango.Services.RewardApi.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.RewardApi.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IConfiguration configuration;
        private readonly IRewardService rewardService;
        private readonly string createdOrderTopic;
        private readonly string CreatedOrderRewardSubscribtion;
        private readonly string serviceBusConnectionString;

        private ServiceBusProcessor rewardProcessor;
        public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
        {
            this.configuration = configuration;
            this.rewardService = rewardService;

            serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");


            createdOrderTopic = configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            CreatedOrderRewardSubscribtion = configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_Subscrision");

            var rewardClient = new ServiceBusClient(serviceBusConnectionString);


            rewardProcessor = rewardClient.CreateProcessor(createdOrderTopic, CreatedOrderRewardSubscribtion); // topic or queue name
        }


        public async Task Start() //when the service starts to work the service listening the messages from queue so it's lifetime is singleton 
        {
            rewardProcessor.ProcessMessageAsync += OnRewardMessageReceived;
            rewardProcessor.ProcessErrorAsync += ErrorHandler;

            await rewardProcessor.StartProcessingAsync(); // без этого процесс не начнётся
        }
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnRewardMessageReceived(ProcessMessageEventArgs args)
        {
            //meesage receiving
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsMessages objMessage = JsonConvert.DeserializeObject<RewardsMessages>(body);

            try
            {
                //TODO - try to log email
                await rewardService.UpdateRewards(objMessage);
                await args.CompleteMessageAsync(args.Message); //to tell the bus that the message was обработанно успешно
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task Stop()
        {
            await rewardProcessor.StopProcessingAsync();
            await rewardProcessor.DisposeAsync();
        }
    }
}
