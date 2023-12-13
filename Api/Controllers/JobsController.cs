using Api.Models.Consts;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class JobsController : ControllerBase
{
    [HttpPost(Name = nameof(QueueJob))]
    public IActionResult QueueJob()
    {
        RecurringJob.TriggerJob(RecurringJobNames.DoSomething);
        return Ok();
    }
}
