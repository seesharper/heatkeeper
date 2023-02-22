using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Programs;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Programs;

[Route("api/[controller]")]
[ApiController]
public class SchedulesController : ControllerBase
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly IQueryExecutor _queryExecutor;

    public SchedulesController(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor)
    {
        _commandExecutor = commandExecutor;
        _queryExecutor = queryExecutor;
    }

    [HttpPost("{scheduleId}/setPoints")]
    public async Task<IActionResult> CreateSetPoint([FromBodyAndRoute] CreateSetPointCommand command)
    {
        await _commandExecutor.ExecuteAsync(command);
        return CreatedAtAction(nameof(CreateSetPoint), new ResourceId(command.SetPointId));
    }

    [HttpGet("{scheduleId}/setPoints")]
    public async Task<SetPoint[]> SetPoints([FromRoute] SetPointsByScheduleQuery query)
           => await _queryExecutor.ExecuteAsync(query);
}