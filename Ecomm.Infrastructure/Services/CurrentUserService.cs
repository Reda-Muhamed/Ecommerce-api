using Ecomm.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Services
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;

                if (user == null || user.Identity?.IsAuthenticated != true)
                    return null;

                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(userIdClaim))
                    return null;

                return Guid.TryParse(userIdClaim, out var userId)
                    ? userId
                    : null;
            }
        }


        public string? Email =>
            _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.Email)?.Value;

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?
                .Identity?.IsAuthenticated ?? false;
    }

}
