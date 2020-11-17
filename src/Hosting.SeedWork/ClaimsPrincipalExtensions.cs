﻿using System;
using System.Security.Claims;
using DrifterApps.Holefeeder.Framework.SeedWork;

namespace DrifterApps.Holefeeder.Hosting.SeedWork
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUniqueId(this ClaimsPrincipal self)
        {
            self.ThrowIfNull(nameof(self));

            var value = self.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return string.IsNullOrWhiteSpace(value) ? Guid.Empty : Guid.Parse(value);
        }
    }
}
