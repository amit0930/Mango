using Azure.Messaging.ServiceBus;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Messaging;
using Mango.Services.RewardAPI.Service;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer:IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string orderCreatedTopic;
        private readonly string OrderCreatedRewardsSubscription;
        private readonly IConfiguration _configuration;
        private readonly RewardService _rewardService;
        private ServiceBusProcessor _orderCreatedTopicProcessor;
        
        public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
        {
                _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            OrderCreatedRewardsSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_Subscription");
            var client = new ServiceBusClient(serviceBusConnectionString);

            _orderCreatedTopicProcessor = client.CreateProcessor(orderCreatedTopic,OrderCreatedRewardsSubscription);
           
            _rewardService = rewardService;
            
        }

        public async Task Start()
        {
            _orderCreatedTopicProcessor.ProcessMessageAsync += OnNewOrderRewardsRequestReceived;
            _orderCreatedTopicProcessor.ProcessErrorAsync += ErrorHandler;
            await _orderCreatedTopicProcessor.StartProcessingAsync();

           
        }

     

        public async Task Stop()
        {
            await _orderCreatedTopicProcessor.StopProcessingAsync();
            await _orderCreatedTopicProcessor.DisposeAsync();

        }
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.Message);
            return Task.CompletedTask;
        }

        private async Task OnNewOrderRewardsRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            RewardsMessage objMessage= JsonConvert.DeserializeObject<RewardsMessage>(body);

            try
            {
                await _rewardService.UpdateRewards(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

      


    }
}
