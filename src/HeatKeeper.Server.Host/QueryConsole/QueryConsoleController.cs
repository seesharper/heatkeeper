using CQRS.Query.Abstractions;
using HeatKeeper.Server.QueryConsole;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.QueryConsole;


[Route("api/[controller]")]
[ApiController]
public class QueryConsoleController(IQueryExecutor queryExecutor) : ControllerBase
{
    [HttpPost()]
    public async Task<Table> Query([FromBody] DatabaseQuery query)
    {
        var result = await queryExecutor.ExecuteAsync(query);
        return result;
    }
}