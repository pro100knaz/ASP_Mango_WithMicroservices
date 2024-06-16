using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models.DTO;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IConfiguration configuration;
        private readonly EmailService emailService;
        private readonly string emailCartQueue;
        private readonly string serviceBusConnectionString;

        private ServiceBusProcessor processor;
        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            this.configuration = configuration;
            this.emailService = emailService;
            serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");

            emailCartQueue = configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);

            processor = client.CreateProcessor(emailCartQueue); // topic or queue name
        }

        public async Task Start() //when the service starts to work the service listening the messages from queue so it's lifetime is singleton 
        {
            processor.ProcessMessageAsync += OnEmailCartRequestReceived;
            processor.ProcessErrorAsync += ErrorHandler;

            await processor.StartProcessingAsync(); // без этого процесс не начнётся
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            //meesage receiving
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CartDto objMessage = JsonConvert.DeserializeObject<CartDto>(body);

            try
            {
                //TODO - try to log email
                await emailService.EmailCartAndLog(objMessage);
                await args.CompleteMessageAsync(args.Message); //to tell the bus that the message was обработанно успешно
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task Stop() //whe
        {
            await processor.StopProcessingAsync();
            await processor.DisposeAsync();

            // processor.ProcessMessageAsync -= OnEmailCartRequestReceived;
            // processor.ProcessErrorAsync -= ErrorHandler;
        }
    }
}
