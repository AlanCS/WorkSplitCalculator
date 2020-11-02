using Microsoft.AspNetCore.Mvc;

namespace AWSServerless1.Controllers
{
    [Route("api/WorkSplit")]
    public class WorkSplitController : ControllerBase
    {
        // GET api/values/5
        [HttpGet("{value}")]
        public string Get(string value)
        {
            return value.ToUpper();
        }
    }
}
