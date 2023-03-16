using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;
using WebMvcClient;
using WebMvcClient.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//�ر��� JWT �����Ϣ����ӳ��
//���������� well-known �����Ϣ�����磬��sub�� �� ��idp�����޸��ŵ�������
//��������Ϣ����ӳ��ġ����������ڵ��� AddAuthentication()֮ǰ���
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
// ���Cookie��֤�����ͨ��OIDCЭ��Զ��������֤��ע��ļ����ط���Authority��ResponseType��ResponseMode��
builder.Services.AddAuthentication((option) =>
{
    // �����֤��Ȩ�����û���¼ʱ��ʹ��cookie
    // ע���� DefaultScheme������ DefaultAuthenticateScheme
    option.DefaultScheme = "Cookies";
    option.DefaultChallengeScheme = "oidc";
})
.AddCookie("Cookies")
//.AddOpenIdConnect("oidc", options =>
//{
//    options.Authority = "http://localhost:5000";  //��Ȩ��������ַ
//    options.SignInScheme = "Cookie";
//    options.RequireHttpsMetadata = false;  //��ʱ����https
//    options.ClientId = "CodeClient";
//    options.ClientSecret = "6KGqzUx6nfZZp0a4NH2xenWSJQWAT8la";
//    options.ResponseType = "code"; //����Authorization Code
//    options.Scope.Add("profile");
//    options.Scope.Add("openid");
//    //options.Scope.Add("code_scope1"); //�����Ȩ��Դ
//    options.SaveTokens = true; //��ʾ�ѻ�ȡ��Token�浽Cookie��
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

#region ��ģʽ
//.AddOpenIdConnect("oidc", option =>
//{
//    option.Authority = "http://localhost:5000";
//    option.RequireHttpsMetadata = false;
//    option.ClientId = "ImplicitClient";
//    option.ClientSecret = "6KGqzUx6nfZZp0a4NH2xenWSJQWAT8la";
//    option.SaveTokens = true;
//    option.SignInScheme = "Cookies";
//    // ��������
//    option.ResponseType = OpenIdConnectResponseType.IdTokenToken;
//    // ����ʽ
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
