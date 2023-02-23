using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Programs;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Programs;

[Route("api/[controller]")]
[ApiController]
public class ProgramsController : ControllerBase
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly IQueryExecutor _queryExecutor;

    public ProgramsController(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor)
    {
        _commandExecutor = commandExecutor;
        _queryExecutor = queryExecutor;
    }

    [HttpPost("{programId}/schedules")]
    public async Task<IActionResult> CreateSchedule([FromBodyAndRoute] CreateScheduleCommand command)
    {
        await _commandExecutor.ExecuteAsync(command);
        return CreatedAtAction(nameof(CreateSchedule), new ResourceId(command.ScheduleId));
    }

    [HttpGet("{programId}/schedules")]
    public async Task<Schedule[]> Programs([FromRoute] SchedulesByProgramQuery query)
            => await _queryExecutor.ExecuteAsync(query);

    [HttpPatch(template: "{programId}")]
    public async Task Patch([FromBodyAndRoute] UpdateProgramCommand command)
        => await _commandExecutor.ExecuteAsync(command);

    [HttpDelete("{programId}")]
    public async Task Delete([FromRoute] DeleteProgramCommand command)
            => await _commandExecutor.ExecuteAsync(command);
}