using System.Threading.Tasks;
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
        {
            this.queryExecutor = queryExecutor;
        }

        [HttpGet("locations")]
        public async Task<DashboardLocation[]> Get([FromRoute]DashboardLocationsQuery query)
        {
            return await queryExecutor.ExecuteAsync(query);
        }
    }
}
