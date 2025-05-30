﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Services;
using System.Text.Json;
using WebApi.Extensions;

namespace edgentauems.Filters
{
    public class HandleJwtToken : ActionFilterAttribute
    {
        private readonly ILogger<HandleJwtToken> _logger;
        private readonly MemberService _memberService;

        public HandleJwtToken(ILogger<HandleJwtToken> logger, MemberService memberService)
        {
            _logger = logger;
            _memberService = memberService;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HasActionAttribute<AllowAnonymousAttribute>())
            {
                await next();
                return;
            }

            var memberId = context.HttpContext.User.FindFirst("memberId")?.Value;

            if (string.IsNullOrEmpty(memberId))
            {
                _logger.LogError($"Token information error, Claims: {JsonSerializer.Serialize(context.HttpContext.User.Claims)}");
                throw new UnauthorizedAccessException("Token information error");
            }

            var member = _memberService.Get(Convert.ToInt32(memberId));
            context.HttpContext.Items.Add("Member", member);

            await next();
        }

        private bool HasActionAttribute<T>(ActionExecutingContext context) where T : Attribute
            => context.ActionDescriptor.EndpointMetadata.Any(x => x.GetType() == typeof(T));
    }
}
