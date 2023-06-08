using Hangfire;
using HangfireSample.Job;
using HangfireSample.Services;
using Microsoft.AspNetCore.Mvc;

namespace HangfireSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HangfireController : ControllerBase
    {
        private readonly IRecurringJobManager _jobManager;

        public HangfireController(IRecurringJobManager jobManager)
        {
            _jobManager = jobManager;
        }

        [HttpGet]
        [Route("RecurringJobs")]
        public bool RecurringJobs()
        {

            _jobManager.AddOrUpdate<CargoJob>("SendCargo", x => x.SendToCargo(),Cron.Hourly);

            return true;
            
        }
    }
}
