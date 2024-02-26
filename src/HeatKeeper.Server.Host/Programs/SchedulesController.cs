using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Programs;
using HeatKeeper.Server.Zones;
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
    public async Task<SetPointInfo[]> SetPoints([FromRoute] SetPointsByScheduleQuery query)
           => await _queryExecutor.ExecuteAsync(query);

    [HttpGet("{scheduleId}/zones")]
    public async Task<ZoneInfo[]> Zones([FromRoute] GetZonesNotAssignedToScheduleQuery query)
           => await _queryExecutor.ExecuteAsync(query);

    [HttpPatch("{scheduleId}")]
    public async Task Patch([FromBodyAndRoute] UpdateScheduleCommand command)
         => await _commandExecutor.ExecuteAsync(command);

    [HttpDelete("{scheduleId}")]
    public async Task Delete([FromRoute] DeleteScheduleCommand command)
            => await _commandExecutor.ExecuteAsync(command);

    [HttpPost("{scheduleId}/activate")]
    public async Task<IActionResult> Activate([FromRoute] SetActiveScheduleCommand command)
    {
        await _commandExecutor.ExecuteAsync(command);
        return CreatedAtAction(nameof(Activate), null);
    }

    [HttpGet("{scheduleId}")]
    public async Task<ScheduleDetails> GetScheduleDetails([FromRoute] GetScheduleDetailsQuery query)
    {
        return await _queryExecutor.ExecuteAsync(query);
    }
}