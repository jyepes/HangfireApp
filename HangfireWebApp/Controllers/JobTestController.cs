namespace HangfireWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobTestController : ControllerBase
    {
        private readonly IJobTestService JobTestService;
        private readonly IBackgroundJobClient BackgroundJobClient;
        private readonly IRecurringJobManager RecurringJobManager;

        public JobTestController(IJobTestService jobTestService, 
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager recurringJobManager)
        {
            JobTestService = jobTestService;
            BackgroundJobClient = backgroundJobClient;
            RecurringJobManager = recurringJobManager;
        }
        
        [HttpGet("/FireAndForgetJob")]
        public ActionResult CreateFireAndForgetJob()
        {
            BackgroundJobClient.Enqueue(() => JobTestService.FireAndForgetJob());
            return Ok();
        }

        [HttpGet("/DelayedJob")]
        public ActionResult CreateDelayedJob()
        {
            BackgroundJobClient.Schedule(() => JobTestService.DelayedJob(), TimeSpan.FromSeconds(60));
            return Ok();
        }

        [HttpGet("/ReccuringJob")]
        public ActionResult CreateReccuringJob()
        {
            RecurringJobManager.AddOrUpdate("jobId", () => JobTestService.ReccuringJob(), Cron.Minutely);
            return Ok();
        }

        [HttpGet("/ContinuationJob")]
        public ActionResult CreateContinuationJob()
        {
            var parentJobId = BackgroundJobClient.Enqueue(() => JobTestService.FireAndForgetJob());
            BackgroundJobClient.ContinueJobWith(parentJobId, () => JobTestService.ContinuationJob());

            return Ok();
        }
    }
}
