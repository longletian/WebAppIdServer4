using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
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

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie((options) => { 
        //options.LoginPath = "/signin";
        //options.LogoutPath = "/signout";
    })
    .AddGitHub((option) =>
    {
        option.ClientId = "a6fdb51b52baff009c5c";
        option.ClientSecret = "9c09882db3bc96d053e301daea3dac93dc5dd6e3";
        option.CallbackPath = "/api/github";
    });

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
});
app.Run();
