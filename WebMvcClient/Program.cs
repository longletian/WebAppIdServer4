using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using WebMvcClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// ���Cookie��֤�����ͨ��OIDCЭ��Զ��������֤��ע��ļ����ط���Authority��ResponseType��ResponseMode��
builder.Services.AddAuthentication((option) =>
{
    // �����֤��Ȩ�����û���¼ʱ��ʹ��cookie
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
    // ��������
    option.ResponseType = OpenIdConnectResponseType.IdTokenToken;
    // ����ʽ
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
