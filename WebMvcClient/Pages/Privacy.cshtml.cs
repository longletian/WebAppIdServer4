﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebMvcClient.Pages
{
    [Authorize]
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        public PrivacyModel(ILogger<PrivacyModel> logger, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        public void OnGet()
        {
            ViewData["accessToken"] = _contextAccessor.HttpContext.GetTokenAsync("access_token").Result;
            ViewData["idToken"] = _contextAccessor.HttpContext.GetTokenAsync("id_token").Result;
        }
    }
}