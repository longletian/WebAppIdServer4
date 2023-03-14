using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;
using WebMvcClient;
using WebMvcClient.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//关闭了 JWT 身份信息类型映射
//这样就允许 well-known 身份信息（比如，“sub” 和 “idp”）无干扰地流过。
//这个身份信息类型映射的“清理”必须在调用 AddAuthentication()之前完成
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
// 添加Cookie认证、添加通过OIDC协议远程请求认证（注意的几个地方：Authority、ResponseType、ResponseMode）
builder.Services.AddAuthentication((option) =>
{
    // 添加认证授权，当用户登录时，使用cookie
    // 注意是 DefaultScheme而不是 DefaultAuthenticateScheme
    option.DefaultScheme = "Cookies";
    option.DefaultChallengeScheme = "oidc";
})
.AddCookie("Cookies")
//.AddOpenIdConnect("oidc", options =>
//{
//    options.Authority = "http://localhost:5000";  //授权服务器地址
//    options.SignInScheme = "Cookie";
//    options.RequireHttpsMetadata = false;  //暂时不用https
//    options.ClientId = "CodeClient";
//    options.ClientSecret = "6KGqzUx6nfZZp0a4NH2xenWSJQWAT8la";
//    options.ResponseType = "code"; //代表Authorization Code
//    options.Scope.Add("profile");
//    options.Scope.Add("openid");
//    //options.Scope.Add("code_scope1"); //添加授权资源
//    options.SaveTokens = true; //表示把获取的Token存到Cookie中
//    options.GetClaimsFromUserInfoEndpoint = true;
//});

.AddOpenIdConnect("oidc", options =>
 {
     options.SignInScheme = "Cookies";
     options.Authority = "http://localhost:5000";
     options.RequireHttpsMetadata = false;
     options.ClientId = "HybridClient";
     options.ClientSecret = "6KGqzUx6nfZZp0a4NH2xenWSJQWAT8la";
     options.ResponseType = "id_token code";
     options.Scope.Add("hybrid_scope1");
     options.SaveTokens = true;
     options.GetClaimsFromUserInfoEndpoint = true;
 });

#region 简化模式
//.AddOpenIdConnect("oidc", option =>
//{
//    option.Authority = "http://localhost:5000";
//    option.RequireHttpsMetadata = false;
//    option.ClientId = "ImplicitClient";
//    option.ClientSecret = "6KGqzUx6nfZZp0a4NH2xenWSJQWAT8la";
//    option.SaveTokens = true;
//    option.SignInScheme = "Cookies";
//    // 请求类型
//    option.ResponseType = OpenIdConnectResponseType.IdTokenToken;
//    // 请求方式
//    option.ResponseMode = OpenIdConnectResponseMode.FormPost;
//});
#endregion

builder.Services.AddAuthorization();
builder.Services.AddScoped<RequestService>();
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureNonBreakingSameSiteCookies();

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
