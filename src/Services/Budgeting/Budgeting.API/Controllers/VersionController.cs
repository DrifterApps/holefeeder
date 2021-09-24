﻿using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace DrifterApps.Holefeeder.Budgeting.API.Controllers
{
    [Route("api/v2/[controller]")]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                var assembly = Assembly.GetEntryAssembly();
                if (assembly is null)
                {
                    return Problem("Null assembly");
                }

                var status = new
                {
                    Name = assembly.FullName,
                    Version = assembly.GetName().Version?.ToString() ?? "unknown",
                    AssemblyFileVersion =
                        assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "unknown",
                    AssemblyInformationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                        ?.InformationalVersion ?? "unknown",
                    ServerDateTime = DateTime.Now
                };

                return Ok(status);
            }, cancellationToken);
        }
    }
}
