using System;
using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using WebApiTest.Infrastructure.Models;

namespace WebApiTest
{
    public class Functions
    {
        public static void ProcessQueueMessage([QueueTrigger("%QueueName%")] string message, ILogger logger)
        {
            try
            {
                var model = JsonSerializer.Deserialize<QueueModel>(message);
                logger.LogInformation("ProcessQueueMessage Success: " + message);
            }
            catch (Exception e)
            {
                logger.LogError("ProcessQueueMessage Error: " + e.Message);
                throw;
            }
        }

        public static void ProcessQueueMessage2([QueueTrigger("webapitest2")] string message, ILogger logger)
        {
            try
            {
                var model = JsonSerializer.Deserialize<QueueModel>(message);
                logger.LogInformation("ProcessQueueMessage Success: " + message);
            }
            catch (Exception e)
            {
                logger.LogError("ProcessQueueMessage Error: " + e.Message);
                throw;
            }
        }
    }
}