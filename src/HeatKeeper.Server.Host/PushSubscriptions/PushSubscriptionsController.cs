using CQRS.Command.Abstractions;
using HeatKeeper.Server.PushSubscriptions;
using Microsoft.AspNetCore.Mvc;
namespace HeatKeeper.Server.Host.PushSubscriptions;


[Route("api/[controller]")]
[ApiController]
public class PushSubscriptionsController(ICommandExecutor commandExecutor) : ControllerBase
{
    [HttpPost()]
    public async Task<IActionResult> CreatePushSubscription([FromBody] CreatePushSubscriptionCommand command)
    {
        await commandExecutor.ExecuteAsync(command);
        return Ok();
    }
}