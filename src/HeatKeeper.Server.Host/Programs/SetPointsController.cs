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
    private readonly IQueryExecutor _queryExecutor;

    public SetPointsController(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor)
    {
        _commandExecutor = commandExecutor;
        _queryExecutor = queryExecutor;
    }


    [HttpDelete("{setPointId}")]
    public async Task Delete([FromRoute] DeleteSetPointCommand command)
            => await _commandExecutor.ExecuteAsync(command);

    [HttpGet("{setPointId}")]
    public async Task<SetPointDetails> GetSetPointDetails([FromRoute] GetSetPointDetailsQuery query)
    {
        return await _queryExecutor.ExecuteAsync(query);
    }
}