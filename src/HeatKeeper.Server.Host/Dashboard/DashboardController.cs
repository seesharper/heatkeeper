using CQRS.Query.Abstractions;
using HeatKeeper.Server.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Dashboard
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IQueryExecutor queryExecutor;

        public DashboardController(IQueryExecutor queryExecutor)
            => this.queryExecutor = queryExecutor;

        [HttpGet("locations")]
        public async Task<DashboardLocation[]> Get([FromRoute] DashboardLocationsQuery query)
            => await queryExecutor.ExecuteAsync(query);

        [HttpGet("temperatures")]
        public async Task<DashboardTemperature[]> GetTemperatures([FromRoute] DashboardTemperaturesQuery query)
            => await queryExecutor.ExecuteAsync(query);
    }
}
