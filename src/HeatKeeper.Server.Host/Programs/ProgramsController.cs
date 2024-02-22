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

    [HttpGet("{programId}")]
    public async Task<ProgramDetails> GetProgramDetails([FromRoute] GetProgramDetailsQuery query)
    {
        return await _queryExecutor.ExecuteAsync(query);
    }

    [HttpPost("{programId}/schedules")]
    public async Task<IActionResult> CreateSchedule([FromBodyAndRoute] CreateScheduleCommand command)
    {
        await _commandExecutor.ExecuteAsync(command);
        return CreatedAtAction(nameof(CreateSchedule), new ResourceId(command.ScheduleId));
    }

    [HttpPost("{programId}/activate")]
    public async Task<IActionResult> Activate([FromRoute] ActivateProgramCommand command)
    {
        await _commandExecutor.ExecuteAsync(command);
        return CreatedAtAction(nameof(Activate), null);
    }

    [HttpGet("{programId}/schedules")]
    public async Task<ScheduleInfo[]> Programs([FromRoute] SchedulesByProgramQuery query)
            => await _queryExecutor.ExecuteAsync(query);

    [HttpPatch(template: "{programId}")]
    public async Task Patch([FromBodyAndRoute] UpdateProgramCommand command)
        => await _commandExecutor.ExecuteAsync(command);

    [HttpDelete("{programId}")]
    public async Task Delete([FromRoute] DeleteProgramCommand command)
            => await _commandExecutor.ExecuteAsync(command);
}

