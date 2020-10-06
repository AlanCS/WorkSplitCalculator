using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkSplitCalculator.Server.Controllers
{
    [Route("api/infrastructure")]
    public class InfrastructureController : ControllerBase
    {
        // this controller is to test the basics, like exception handling, security, etc

        private readonly IWebHostEnvironment env;

        public InfrastructureController(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public enum InfrastructureActions
        {
            ReturnSuccess = 1,
            ThrowException = 2
        }

        [HttpPost]
        public ActionResult PerformAction([FromBody] InfrastructureActions action)
        {
            if (!env.IsDevelopment()) return BadRequest();

            // we could test any security mechanisms here
            switch (action)
            {
                case InfrastructureActions.ThrowException:
                    throw new System.Exception("Testing unhandled exception!");
                default:
                    break;
            }

            return Ok();
        }
    }
}
