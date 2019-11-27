using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Measurements
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        /// <summary>
        /// Gets the product version.
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            var versionAttribute = typeof(VersionController).Assembly.GetCustomAttributes<AssemblyInformationalVersionAttribute>().Single();
            return Ok(versionAttribute.InformationalVersion);
        }
    }
}
