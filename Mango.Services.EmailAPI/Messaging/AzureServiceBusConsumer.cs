using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Service;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer:IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string mangoNewUserQueue;
        private readonly string orderCreatedTopic;
        private readonly string OrderCreatedEmailSubscription;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private ServiceBusProcessor _emailClientProcessor;
        private ServiceBusProcessor _NewUserEmailClientProcessor;
        private ServiceBusProcessor _orderCreatedEmailClientProcessor;
        public AzureServiceBusConsumer(IConfiguration configuration,EmailService emailService)
        {
                _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            mangoNewUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:MangoNewUserQueue");
            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            OrderCreatedEmailSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Email_Subscription");
            var client = new ServiceBusClient(serviceBusConnectionString);

            _emailClientProcessor = client.CreateProcessor(emailCartQueue);
            _NewUserEmailClientProcessor= client.CreateProcessor(mangoNewUserQueue);
            _orderCreatedEmailClientProcessor = client.CreateProcessor(orderCreatedTopic, OrderCreatedEmailSubscription);
            _emailService = emailService;
            
        }

        public async Task Start()
        {
            _emailClientProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailClientProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailClientProcessor.StartProcessingAsync();

            _NewUserEmailClientProcessor.ProcessMessageAsync += OnNewUserRegisterationRequestReceived;
            _NewUserEmailClientProcessor.ProcessErrorAsync += ErrorHandler;
            await _NewUserEmailClientProcessor.StartProcessingAsync();

            _orderCreatedEmailClientProcessor.ProcessMessageAsync += OnNewOrderCreatedEmailRequestReceived;
            _orderCreatedEmailClientProcessor.ProcessErrorAsync += ErrorHandler;
            await _orderCreatedEmailClientProcessor.StartProcessingAsync();
        }

     

        public async Task Stop()
        {
            await _emailClientProcessor.StopProcessingAsync();
            await _emailClientProcessor.DisposeAsync();

            await _NewUserEmailClientProcessor.StopProcessingAsync();
            await _NewUserEmailClientProcessor.DisposeAsync();

            await _orderCreatedEmailClientProcessor.StopProcessingAsync();
            await _orderCreatedEmailClientProcessor.DisposeAsync();

        }
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.Message);
            return Task.CompletedTask;
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            CartDto objMessage= JsonConvert.DeserializeObject<CartDto>(body);

            try
            {
                await _emailService.EmailCartAndLog(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task OnNewUserRegisterationRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            string objMessage = JsonConvert.DeserializeObject<string>(body);

            try
            {
                await _emailService.RegisterUserEmailAndLog(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task OnNewOrderCreatedEmailRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            RewardsMessage objMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);

            try
            {
                await _emailService.LogOrderPlaced(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
