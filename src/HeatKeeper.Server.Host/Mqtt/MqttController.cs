using CQRS.Command.Abstractions;
using HeatKeeper.Server.Mqtt;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Mqtt;

[Route("api/[controller]")]
[ApiController]
public class MqttController(ICommandExecutor commandExecutor) : ControllerBase
{
    [HttpPost]
    public async Task Post([FromBody] PublishMqttMessageCommand command)
            => await commandExecutor.ExecuteAsync(command);
}