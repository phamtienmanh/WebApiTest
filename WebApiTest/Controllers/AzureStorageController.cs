using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApiTest.Infrastructure.Models;
using WebApiTest.Infrastructure.Services;

namespace WebApiTest.Controllers
{
    [Route("api/message")]
    [ApiController]
    public class AzureStorageController : ControllerBase
    {
        private readonly IAzureStorageServices _azureStorageServices;
        public AzureStorageController(IAzureStorageServices azureStorageServices)
        {
            _azureStorageServices = azureStorageServices;
        }

        [HttpPost]
        public IActionResult Create([FromBody] QueueModel queueModel)
        {
            var message = JsonConvert.SerializeObject(queueModel);
            _azureStorageServices.CreateMess(message);
            return Ok();
        }
    }
}