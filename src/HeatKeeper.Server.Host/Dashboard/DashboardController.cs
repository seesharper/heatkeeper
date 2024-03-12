using CQRS.Query.Abstractions;
using HeatKeeper.Server.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Dashboard
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController(IQueryExecutor queryExecutor) : ControllerBase
    {
        [HttpGet("locations")]
        public async Task<DashboardLocation[]> Get([FromRoute] DashboardLocationsQuery query)
            => await queryExecutor.ExecuteAsync(query);       
    }
}
