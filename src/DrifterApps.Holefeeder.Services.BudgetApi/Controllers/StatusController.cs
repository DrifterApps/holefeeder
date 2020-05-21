using System;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Services.BudgetDto;
using Microsoft.AspNetCore.Mvc;

namespace DrifterApps.Holefeeder.Services.BudgetApi.Controllers
{
    [Route("api/v1/[controller]")]
    public class StatusController : Controller
    {
        private struct Routes
        {
            public const string GET_STATUS = "get_status";
        }

        [HttpGet(Name = Routes.GET_STATUS)]
        [ProducesResponseType(typeof(StatusDto[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                var assembly = Assembly.GetEntryAssembly();
                var status = new StatusDto
                {
                    Version = assembly?.GetName()?.Version?.ToString() ?? "unknown",
                    AssemblyFileVersion =
                        assembly?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "unknown",
                    AssemblyInformationalVersion =
                        assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                            ?.InformationalVersion ?? "unknown",
                    ServerDateTime = DateTime.Now
                };
                
                return Ok(status);
            }, cancellationToken).ConfigureAwait(false);
        }
    }
}
