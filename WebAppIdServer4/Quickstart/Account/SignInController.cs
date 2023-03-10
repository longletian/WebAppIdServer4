using AspNet.Security.OAuth.GitHub;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Net;
using System.Security.Claims;

namespace WebAppIdServer4.Quickstart.Account
{

    [Route("signin")]
    public class SignInController : Controller
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public SignInController(IHttpContextAccessor _httpContextAccessor)
        {
            httpContextAccessor = _httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> SignIn(string returnUrl)
        {
            var request = httpContextAccessor.HttpContext.Request;
            var response = httpContextAccessor.HttpContext.Response;

            var claims = new List<Claim>()//身份验证信息
            {
                new Claim("Userid","1"),
                new Claim(ClaimTypes.Role,"Admin"),
                new Claim(ClaimTypes.Role,"User"),
                new Claim(ClaimTypes.Email,$"xxx@163.com"),
                new Claim("Account","Administrator"),
                new Claim("role","admin"),
            };

            ClaimsPrincipal userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Customer"));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30),//过期时间：30分钟

            });
            if (!returnUrl.StartsWith("http"))
            {
                returnUrl = "https://" + Request.Host.Host + returnUrl;
            }

            //return Redirect(returnUrl);
            return Redirect($"/signin?scheme={CookieAuthenticationDefaults.AuthenticationScheme}&returnUrl={WebUtility.UrlEncode(returnUrl)}");
        }

        [HttpGet("~/OpenId")]
        public async Task<string> OpenId(string provider = null)
        {
            var authenticateResult = await httpContextAccessor.HttpContext.AuthenticateAsync(provider);
            if (!authenticateResult.Succeeded) return null;
            var openIdClaim = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier);
            return openIdClaim?.Value;
        }

        [HttpGet("~/signin-callback")]
        public async Task<IActionResult> Home(string provider = null, string redirectUrl = "")
        {
            var authenticateResult = await httpContextAccessor.HttpContext.AuthenticateAsync(provider);
            if (!authenticateResult.Succeeded) return Redirect(redirectUrl);
            var openIdClaim = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier);
            if (openIdClaim == null || string.IsNullOrWhiteSpace(openIdClaim.Value))
                return Redirect(redirectUrl);

            //TODO 记录授权成功后的信息 

            string email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            string name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;
            string gitHubName = authenticateResult.Principal.FindFirst(GitHubAuthenticationConstants.Claims.Name)?.Value;
            string gitHubUrl = authenticateResult.Principal.FindFirst(GitHubAuthenticationConstants.Claims.Url)?.Value;
            //startup 中 AddGitHub配置项  options.ClaimActions.MapJsonKey(LinConsts.Claims.AvatarUrl, "avatar_url");
            //string avatarUrl = authenticateResult.Principal.FindFirst(LinConsts.Claims.AvatarUrl)?.Value;

            return Redirect($"{redirectUrl}?openId={openIdClaim.Value}");
        }



        //private string ReturnUrl = "";

        //public async Task<IActionResult> OnPost([FromForm] string provider)
        //{
        //    if (string.IsNullOrWhiteSpace(provider))
        //    {
        //        return BadRequest();
        //    }

        //    return await IsProviderSupportedAsync(HttpContext, provider) is false
        //        ? BadRequest()
        //        : Challenge(new AuthenticationProperties
        //        {
        //            RedirectUri = Url.IsLocalUrl(ReturnUrl) ? ReturnUrl : "/"
        //        }, provider);
        //}

        //private static async Task<AuthenticationScheme[]> GetExternalProvidersAsync(HttpContext context)
        //{
        //    var schemes = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
        //    return (await schemes.GetAllSchemesAsync())
        //        .Where(scheme => !string.IsNullOrEmpty(scheme.DisplayName))
        //        .ToArray();
        //}

        //private static async Task<bool> IsProviderSupportedAsync(HttpContext context, string provider) =>
        //    (await GetExternalProvidersAsync(context))
        //    .Any(scheme => string.Equals(scheme.Name, provider, StringComparison.OrdinalIgnoreCase));

    }
}
