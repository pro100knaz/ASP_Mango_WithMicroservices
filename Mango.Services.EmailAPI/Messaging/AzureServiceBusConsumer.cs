﻿using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.MEssages;
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
        
        private readonly string orderCreatedTopic;
        private readonly string orederCreatedEmailSubscribtion;



        private ServiceBusProcessor ShoppingCartprocessor;
        private ServiceBusProcessor EmailProcessor;
        private ServiceBusProcessor orderCreatedEmailRequestProcessor;
        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            this.configuration = configuration;
            this.emailService = emailService;
            serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");

            emailCartQueue = configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            registerEmailQueue = configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");

            orderCreatedTopic = configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            orederCreatedEmailSubscribtion = configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Email_Subscrision");

            var ShopingCartclient = new ServiceBusClient(serviceBusConnectionString);
            var Emailclient = new ServiceBusClient(serviceBusConnectionString);
            var orderCreatedEmail = new ServiceBusClient(serviceBusConnectionString);

            ShoppingCartprocessor = ShopingCartclient.CreateProcessor(emailCartQueue); // topic or queue name
            EmailProcessor = Emailclient.CreateProcessor(registerEmailQueue); // topic or queue name
            orderCreatedEmailRequestProcessor = orderCreatedEmail.CreateProcessor(orderCreatedTopic, orederCreatedEmailSubscribtion);
        }


        public async Task Start() //when the service starts to work the service listening the messages from queue so it's lifetime is singleton 
        {
            ShoppingCartprocessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            ShoppingCartprocessor.ProcessErrorAsync += ErrorHandler;

            await ShoppingCartprocessor.StartProcessingAsync(); // без этого процесс не начнётся


            EmailProcessor.ProcessMessageAsync += OnEmailRegisterReceived;
            EmailProcessor.ProcessErrorAsync += ErrorHandler;

            await EmailProcessor.StartProcessingAsync(); // без этого процесс не начнётся
                                                         //
            orderCreatedEmailRequestProcessor.ProcessMessageAsync += OnOrderCreatedEmailRequest;
            orderCreatedEmailRequestProcessor.ProcessErrorAsync += ErrorHandler;

            await orderCreatedEmailRequestProcessor.StartProcessingAsync(); // без этого процесс не начнётся
        }

        private async Task OnOrderCreatedEmailRequest(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsMessages objMessage = JsonConvert.DeserializeObject<RewardsMessages>(body);

            try
            {
                 await emailService.LogOrderPlaced(objMessage);
                 await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
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



            await orderCreatedEmailRequestProcessor.StopProcessingAsync();
            await orderCreatedEmailRequestProcessor.DisposeAsync();
            // processor.ProcessMessageAsync -= OnEmailCartRequestReceived;
            // processor.ProcessErrorAsync -= ErrorHandler;
        }
    }
}
