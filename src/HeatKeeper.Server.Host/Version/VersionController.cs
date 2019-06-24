using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HeatKeeper.Abstractions.CQRS;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Measurements
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        private readonly ICommandExecutor commandExecutor;

        public VersionController(ICommandExecutor commandExecutor)
        {
            this.commandExecutor = commandExecutor;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var versionAttribute = typeof(VersionController).Assembly.GetCustomAttributes<AssemblyInformationalVersionAttribute>().Single();
            return Ok(versionAttribute.InformationalVersion);
        }
    }
}