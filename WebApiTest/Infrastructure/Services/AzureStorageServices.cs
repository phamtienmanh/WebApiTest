using System;
using System.Linq;
using System.Text;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Configuration;

namespace WebApiTest.Infrastructure.Services
{
    public interface IAzureStorageServices
    {
        void CreateMess(string message);
    }

    public class AzureStorageServices: IAzureStorageServices
    {
        private readonly QueueClient _queueClient;
//        private readonly QueueClient _queueClient2;
        public AzureStorageServices(IConfiguration configuration)
        {
            try
            {
                _queueClient = new QueueClient(configuration.GetConnectionString("AzureStorage"),
                    configuration["QueueName"]);
                _queueClient.CreateIfNotExists();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
//            _queueClient2 = new QueueClient(configuration.GetConnectionString("AzureStorage"),
//                "webapitest2");
//            _queueClient2.CreateIfNotExists();
        }

        public void CreateMess(string message)
        {
            if (_queueClient.Exists())
            {
                var plainTextBytes = Encoding.UTF8.GetBytes(message);
                _queueClient.SendMessage(System.Convert.ToBase64String(plainTextBytes));
//                _queueClient2.SendMessage(System.Convert.ToBase64String(plainTextBytes));
            }
        }
    }
}
