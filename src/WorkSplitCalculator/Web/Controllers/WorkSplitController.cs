using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AWSServerless1.Controllers
{
    [Route("WorkSplit")]
    public class WorkSplitController : ControllerBase
    {
        private readonly ILogger<WorkSplitController> logger;

        public WorkSplitController(ILogger<WorkSplitController> logger)
        {
            this.logger = logger;
        }
        
        [HttpGet("{value}")]
        public string Get(string value)
        {
            logger.LogInformation(value);

            return value.ToUpper();            
        }
    }
}
