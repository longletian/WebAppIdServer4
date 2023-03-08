using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using WebMvcClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// 添加Cookie认证、添加通过OIDC协议远程请求认证（注意的几个地方：Authority、ResponseType、ResponseMode）
builder.Services.AddAuthentication((option) =>
{
    // 添加认证授权，当用户登录时，使用cookie
    option.DefaultAuthenticateScheme = "Cookies";
    option.DefaultChallengeScheme = "oidc";
})
.AddCookie("Cookies")
.AddOpenIdConnect("oidc", option =>
{
    option.Authority = "http://localhost:5000";
    option.RequireHttpsMetadata = false;
    option.ClientId = "ImplicitClient";
    option.ClientSecret = "6KGqzUx6nfZZp0a4NH2xenWSJQWAT8la";
    option.SaveTokens = true;
    option.SignInScheme = "Cookies";
    // 请求类型
    option.ResponseType = OpenIdConnectResponseType.IdTokenToken;
    // 请求方式
    option.ResponseMode = OpenIdConnectResponseMode.FormPost;
});

builder.Services.ConfigureNonBreakingSameSiteCookies();



builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
