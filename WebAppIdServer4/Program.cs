using IdentityServerHost;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebAppIdServer4;
using WebAppIdServer4.Data;
using WebAppIdServer4.Model.Entity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
//builder.Services.AddControllersWithViews();
builder.Services.AddIdentityServer((options) =>
    {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;
    })

#region ���ݿ�
//�������ݣ���Դ���ͻ��ˡ���ݣ���
.AddConfigurationStore(options =>
{
    options.ConfigureDbContext = p
        => p.UseMySql(builder.Configuration.GetConnectionString("MysqlCon") ?? string.Empty, new MySqlServerVersion(new Version(8, 0, 29)), sql =>
        {
            sql.MigrationsAssembly("WebAppIdServer4");
            sql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: System.TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
        });
})
// IdentityServer��ʹ��ʱ������ �������ݣ����ƣ�������û�����Ȩ��Ϣconsents��
.AddOperationalStore(option =>
{
    option.ConfigureDbContext = p
        => p.UseMySql(builder.Configuration.GetConnectionString("MysqlCon") ?? string.Empty, new MySqlServerVersion(new Version(8, 0, 29)), sql =>
        {
            sql.MigrationsAssembly("WebAppIdServer4");
            sql.EnableRetryOnFailure(
                      maxRetryCount: 5,
                      maxRetryDelay: System.TimeSpan.FromSeconds(30),
                      errorNumbersToAdd: null);
        });
    // �Զ����� token ����ѡ
    option.EnableTokenCleanup = true;
    // �Զ����� token ����ѡ
    option.TokenCleanupInterval = 30;
});
//.AddAspNetIdentity<UserEntity>();
//.AddTestUsers(TestUsers.Users);

// �����Ա��ص�
builder.Services.AddDbContext<IdentityDataContext>(option
    => option.UseMySql(builder.Configuration.GetConnectionString("MysqlCon") ?? string.Empty, new MySqlServerVersion(new Version(8, 0, 29))));

builder.Services.AddIdentity<UserEntity, IdentityRole>()
    .AddEntityFrameworkStores<IdentityDataContext>()
    .AddDefaultTokenProviders();

//���ݿ�Ǩ��
builder.Services.AddHostedService<SeedData>();
#endregion

//.AddInMemoryIdentityResources(IdentityConfig.IdentityResources)
//.AddInMemoryApiScopes(IdentityConfig.GetApiScopes())
//.AddInMemoryApiResources(IdentityConfig.ApiResources)
//.AddInMemoryClients(IdentityConfig.GetClients())
//.AddTestUsers(IdentityConfig.GetTestUsers().ToList())
//.AddDeveloperSigningCredential();

// ���IdentityServer4ʹ��chrome 80�汾���е�¼���޷���ת������
builder.Services.ConfigureNonBreakingSameSiteCookies();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    //option.DefaultChallengeScheme = "oidc";
    //options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, (options) =>
    {
        options.Cookie.IsEssential = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
        //options.SlidingExpiration = true;
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
        options.UsePkce = true;
        options.ClaimActions.MapAll();
        options.ClaimActions.MapJsonKey("sub","sub");
        oAuthOption.Scopes?.ForEach((item) =>
        {
            options.Scope.Add(item);
        });
        //����Ʊ��
        options.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
        {
            OnCreatingTicket = context =>
          {
              //var fullName = context.Identity.FindFirst("urn:github:name").Value;
              //var email = context.Identity.FindFirst(ClaimTypes.Email).Value;

              if (context.AccessToken is { })
              {
                  context.Identity?.AddClaim(new Claim("access_token", context.AccessToken));
              }
              string valueText = context.User.GetRawText();
              if (!string.IsNullOrWhiteSpace(valueText))
              {
                  // �������Ҫ�ǽ��û���Ϣд��Claim�У�����д����Щ��Claim����Ҫ������options.ClaimActionsȥ���ã������options.ClaimActions.MapAll()���ǽ����е��û���Ϣд��Claim��
                  context.RunClaimActions(context.User);

                  //options.ClaimActions.MapAll();
              }
              //var result = System.Text.Json.JsonSerializer.Deserialize<UserConfig>(context.User);
              //context.Response.Cookies.Append("name", result.name);
              return Task.CompletedTask;
          },

            // ���ʱ��ܾ�
            //OnAccessDenied = context => {
            //    return Task.CompletedTask;

            //},

            //// ��֤��ת
            //OnRedirectToAuthorizationEndpoint = context => {
            //    return Task.CompletedTask;
            //},

            //// ���ܵ�Ʊ�ݵ�ʱ�򴥷�
            //OnTicketReceived = context => {
            //    return Task.CompletedTask;
            //},

            //OnRemoteFailure = context => {
            //    return Task.CompletedTask;
            //}
        };
      

    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", b =>
{
    b.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
}));

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
app.UseIdentityServer();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapDefaultControllerRoute();
//});

app.MapRazorPages();

try
{
    app.Run();
}
catch (Exception ex)
{
    throw ex;
}


