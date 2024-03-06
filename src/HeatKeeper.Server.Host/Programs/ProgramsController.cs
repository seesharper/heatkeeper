using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Programs;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Programs;

[Route("api/[controller]")]
[ApiController]
public class ProgramsController(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor) : ControllerBase
{
    [HttpGet("{programId}")]
    public async Task<ProgramDetails> GetProgramDetails([FromRoute] GetProgramDetailsQuery query)
    {
        return await queryExecutor.ExecuteAsync(query);
    }

    [HttpPost("{programId}/schedules")]
    public async Task<IActionResult> CreateSchedule([FromBodyAndRoute] CreateScheduleCommand command)
    {
        await commandExecutor.ExecuteAsync(command);
        return CreatedAtAction(nameof(CreateSchedule), new ResourceId(command.ScheduleId));
    }

    [HttpPost("{programId}/activate")]
    public async Task<IActionResult> Activate([FromRoute] ActivateProgramCommand command)
    {
        await commandExecutor.ExecuteAsync(command);
        return CreatedAtAction(nameof(Activate), null);
    }

    [HttpGet("{programId}/schedules")]
    public async Task<ScheduleInfo[]> Programs([FromRoute] SchedulesByProgramQuery query)
            => await queryExecutor.ExecuteAsync(query);

    [HttpPatch(template: "{programId}")]
    public async Task Patch([FromBodyAndRoute] UpdateProgramCommand command)
        => await commandExecutor.ExecuteAsync(command);

    [HttpDelete("{programId}")]
    public async Task Delete([FromRoute] DeleteProgramCommand command)
            => await commandExecutor.ExecuteAsync(command);
}

