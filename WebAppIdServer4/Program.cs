using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebAppIdServer4;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddIdentityServer()
    .AddTestUsers(IdentityConfig.GetTestUsers().ToList())
    .AddInMemoryIdentityResources(IdentityConfig.IdentityResources)
    .AddInMemoryApiScopes(IdentityConfig.GetApiScopes())
    .AddInMemoryApiResources(IdentityConfig.ApiResources)
    .AddInMemoryClients(IdentityConfig.GetClients())
    .AddDeveloperSigningCredential();

// 解决IdentityServer4使用chrome 80版本进行登录后无法跳转的问题
builder.Services.ConfigureNonBreakingSameSiteCookies();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddCookie((options) =>
    {
        options.Cookie.IsEssential = true;
        //options.LoginPath = "/signin";
        //options.LogoutPath = "/signout";
    })
    .AddGitHub((options) =>
    {
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        OAuthOption oAuthOption = builder.Configuration.GetSection("Github").Get<OAuthOption>();
        if (oAuthOption == null)
            throw new Exception(nameof(oAuthOption));
        options.ClientId = oAuthOption.ClientId;
        options.ClientSecret = oAuthOption.ClientSecret;
        options.CallbackPath = oAuthOption.CallbackPath;
        oAuthOption.Scopes?.ForEach((item) =>
        {
            options.Scope.Add(item);
            //options.Scope.Add("urn:github:avatar_url");
            //options.Scope.Add("urn:github:bio");
            //authenticateResult.Principal.FindFirst(LinConsts.Claims.AvatarUrl)?.Value;
            //options.ClaimActions.MapJsonKey(LinConsts.Claims.AvatarUrl, "avatar_url");
            //options.ClaimActions.MapJsonKey(LinConsts.Claims.BIO, "bio");
            //options.ClaimActions.MapJsonKey(LinConsts.Claims.BlogAddress, "blog");
        });
       
        //创建票据
        options.Events.OnCreatingTicket += context =>
        {
            if (context.AccessToken is { })
            {
                context.Identity?.AddClaim(new Claim("access_token", context.AccessToken));
            }
            return Task.CompletedTask;
        };

    });
    //.AddIdentityServerJwt();
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
app.UseIdentityServer();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();

    endpoints.MapGet("/signout", async ctx =>
    {
        await ctx.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new AuthenticationProperties
            {
                RedirectUri = "/"
            });
    });
});
app.Run();
