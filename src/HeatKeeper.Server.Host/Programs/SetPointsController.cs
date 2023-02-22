using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Programs;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Programs;

[Route("api/[controller]")]
[ApiController]
public class SetPointsController : ControllerBase
{
    private readonly ICommandExecutor _commandExecutor;

    public SetPointsController(ICommandExecutor commandExecutor)
        => _commandExecutor = commandExecutor;

    [HttpPatch("{setPointId}")]
    public async Task Patch([FromBodyAndRoute] UpdateSetPointCommand command)
          => await _commandExecutor.ExecuteAsync(command);

    [HttpDelete("{setPointId}")]
    public async Task Delete([FromRoute] DeleteSetPointCommand command)
            => await _commandExecutor.ExecuteAsync(command);
}