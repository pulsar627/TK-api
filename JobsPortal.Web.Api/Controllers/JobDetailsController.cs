using JobsPortal.Business.Interfaces;
using JobsPortal.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobsPortal.Web.Api.Controllers
{
    [Route("api/jobs")]
    [ApiController]
    public class JobDetailsController : ControllerBase
    {
        private readonly IJobDetailsManager _jobDetailsManager;
        public JobDetailsController(IJobDetailsManager jobDetailsManager)
        {
            _jobDetailsManager = jobDetailsManager;
        }
        [HttpGet("{first}/{last}")]
        public IActionResult GetByPageNum([FromQuery] JobDetailsDto filters, int first = 0, int last = 100)
        {
            return Ok(_jobDetailsManager.GetByPageNum(first, last, filters));
        }

        [HttpGet("count")]
        public IActionResult GetCount()
        {
            return Ok(_jobDetailsManager.Count());
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody]JobDetailsDto jobDetails)
        {
            return Ok(_jobDetailsManager.Update(jobDetails));
        }
    }
}