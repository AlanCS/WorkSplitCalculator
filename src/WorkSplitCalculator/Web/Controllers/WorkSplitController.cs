using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Proxy;

namespace Web.Controllers
{
    [Route("WorkSplit")]
    public class WorkSplitController : ControllerBase
    {
        private readonly ILogger<WorkSplitController> logger;
        private readonly ISnsProxy snsProxy;

        public WorkSplitController(ILogger<WorkSplitController> logger, ISnsProxy snsProxy = null)
        {
            this.logger = logger;
            this.snsProxy = snsProxy ?? new SnsProxy("SnsTopic");
            
        }

        [HttpGet("{value}")]
        public async System.Threading.Tasks.Task<string> GetAsync(string value)
        {
            logger.LogInformation("Logged manually: " + value);

            await snsProxy.Publish(value);

            return value.ToUpper();
        }
    }
}
