using Application.Common.Interfaces;
using Domain.Common.Definitions;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace Infrastructure.Services
{
    public class CurrentRequestService : ICurrentRequestService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentRequestService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId => Convert.ToInt32(_httpContextAccessor.HttpContext?.User?.FindFirstValue(CustomClaimTypes.UserId) ?? "0");
        public string RequestId => _httpContextAccessor?.HttpContext?.TraceIdentifier;
    }
}
