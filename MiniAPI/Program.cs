using Autofac.Extensions.DependencyInjection;
using Autofac;
using MiniAPI;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MiniAPI.Map;
using MiniAPI.Services.IService;
using MiniAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IdentityModel;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Directory.GetCurrentDirectory(),
    EnvironmentName = Environments.Development,
    WebRootPath = "wwwroot"
});

builder.Configuration.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "JsonConfig"))
    .AddJsonFile("config.json");


builder.Services.AddDbContextPool<DataContext>(c => c.UseInMemoryDatabase("UserList"));
//builder.Services.AddDbContextFactory<DataContext>((c) => c.UseMySql(builder.Configuration["ConnectionStrings::MysqlCon"], new MySqlServerVersion(new Version(8, 0, 29))));

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new AutofacModule()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme,
    option => {
        option.Authority = "http://localhost:5000";
        option.RequireHttpsMetadata = false;
        option.ApiName = "api1";
        option.ApiSecret = "apipwd";
    });

builder.Services.AddAuthorization();

#region 授权
//builder.Services.AddAuthorization((c) => {
//    //new Claim(ClaimTypes.Role, "admin");
//    //new Claim(JwtClaimTypes.Role, "admin");
//    //new Claim("username", "zhangsan");
//    //c.AddPolicy("policy3", d => d.RequireClaim(JwtClaimTypes.Role, "admin"));

//    c.AddPolicy("policy3", c => c.RequireClaim(JwtClaimTypes.Name, "letian"));
//    //c.AddPolicy("policy2", c => c.RequireClaim(JwtClaimTypes.Role, "admin"));
//});
#endregion

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.IncludeFields = true;
});
builder.Logging.AddJsonConsole();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IdentityService>();

var app = builder.Build();

string hostUrl = builder.Configuration["Kestrel:Url"] ?? "";
if (!string.IsNullOrEmpty(hostUrl))
{
    app.Urls.Add(hostUrl.ToString());
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.GetUserAction();

app.MapGet("/identity", [Authorize(Roles = "admin")] (HttpRequest httpRequest) =>
{
    return new JsonResult(from c in httpRequest.HttpContext.User.Claims?.ToList() select new { c.Type, c.Value });
});

//app.MapGet("/identity", (HttpRequest httpRequest) =>
//{
//    return new JsonResult(from c in httpRequest.HttpContext.User.Claims?.ToList() select new { c.Type, c.Value });
//})
//.RequireAuthorization((option) => {
//    option.RequireClaim("username", "zhangsan");
//});

try
{
    app.Logger.LogInformation("项目开始启动");

    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogError("项目启动异常" + JsonSerializer.Serialize(ex));
    throw;
}


