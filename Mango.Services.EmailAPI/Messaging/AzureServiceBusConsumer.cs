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
        private readonly string registerEmailQueue;
        private readonly string serviceBusConnectionString;

        private ServiceBusProcessor ShoppingCartprocessor;
        private ServiceBusProcessor EmailProcessor;
        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            this.configuration = configuration;
            this.emailService = emailService;
            serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");

            emailCartQueue = configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            registerEmailQueue = configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");

            var ShopingCartclient = new ServiceBusClient(serviceBusConnectionString);
            var Emailclient = new ServiceBusClient(serviceBusConnectionString);

            ShoppingCartprocessor = ShopingCartclient.CreateProcessor(emailCartQueue); // topic or queue name
            EmailProcessor = Emailclient.CreateProcessor(registerEmailQueue); // topic or queue name
        }


        public async Task Start() //when the service starts to work the service listening the messages from queue so it's lifetime is singleton 
        {
            ShoppingCartprocessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            ShoppingCartprocessor.ProcessErrorAsync += ErrorHandler;

            await ShoppingCartprocessor.StartProcessingAsync(); // без этого процесс не начнётся


            EmailProcessor.ProcessMessageAsync += OnEmailRegisterReceived;
            EmailProcessor.ProcessErrorAsync += ErrorHandler;

            await EmailProcessor.StartProcessingAsync(); // без этого процесс не начнётся
        }

        private async Task OnEmailRegisterReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            string objMessage = JsonConvert.DeserializeObject<string>(body);

            try
            {
                //TODO - try to log email
                await emailService.RegisterUserEmailLog(objMessage);
                await args.CompleteMessageAsync(args.Message); //to tell the bus that the message was обработанно успешно
            }
            catch (Exception ex)
            {

                throw;
            }
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
            await ShoppingCartprocessor.StopProcessingAsync();
            await ShoppingCartprocessor.DisposeAsync();


            await EmailProcessor.StopProcessingAsync();
            await EmailProcessor.DisposeAsync();
            // processor.ProcessMessageAsync -= OnEmailCartRequestReceived;
            // processor.ProcessErrorAsync -= ErrorHandler;
        }
    }
}
