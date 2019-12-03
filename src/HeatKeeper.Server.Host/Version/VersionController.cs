using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Version;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Measurements
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        private readonly IQueryExecutor queryExecutor;

        public VersionController(IQueryExecutor queryExecutor)
        {
            this.queryExecutor = queryExecutor;
        }

        [HttpGet]
        public async Task<AppVersion> Get([FromQuery]VersionQuery query)
        {
            return await queryExecutor.ExecuteAsync(query);
        }
    }
}
