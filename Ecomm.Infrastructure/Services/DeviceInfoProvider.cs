using Ecomm.Core.DTOs;
using Ecomm.Core.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Services
{
    public class DeviceInfoProvider : IDeviceInfoProvider
    {
        private readonly IHttpContextAccessor httpContext;

        public DeviceInfoProvider(IHttpContextAccessor httpContext)
        {
            this.httpContext = httpContext;
        }

        public DeviceInfoDto GetDeviceInfo()
        {
            var request = httpContext.HttpContext!.Request;

            var userAgent = request.Headers["User-Agent"].FirstOrDefault() ?? string.Empty;

            // Try x-forwarded-for first (if behind reverse proxy), otherwise remote IP
            string ipAddress = string.Empty;
            if (request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ipAddress = request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim() ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(ipAddress) && request.HttpContext.Connection.RemoteIpAddress != null)
            {
                ipAddress = request.HttpContext.Connection.RemoteIpAddress!.ToString() ?? string.Empty;
            }

            // Optionally capture a few headers (limit count & length to prevent DoS)
            var allowedHeaders = new[] { "Accept-Language", "Referer" };
            var headers = request.Headers
                .Where(h => allowedHeaders.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
                .ToDictionary(h => h.Key, h => h.Value.ToString());

            return new DeviceInfoDto(userAgent, ipAddress, headers);

        }
    }

}
