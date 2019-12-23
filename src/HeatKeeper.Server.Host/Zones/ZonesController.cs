using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Zones;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Zones
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZonesController
    {
        private readonly IQueryExecutor queryExecutor;

        public ZonesController(IQueryExecutor queryExecutor)
        {
            this.queryExecutor = queryExecutor;
        }


        [HttpGet("{zoneId}")]
        public async Task<ZoneDetails> GetZoneDetails([FromRoute]ZoneDetailsQuery query)
        {
            return await queryExecutor.ExecuteAsync(query);
        }
    }
}
