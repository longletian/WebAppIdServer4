using Asp.Versioning;
using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Mvc;
using MiniAPI.Model;
using MiniAPI.Services.IService;
using System.Xml.Linq;

namespace MiniAPI.Map
{
    public static class User
    {

        public static void GetUserAction(this WebApplication app)
        {
            var userService = app.Services.GetRequiredService<IUserService>();

            //var versionSet = app.NewApiVersionSet()
            //    .HasApiVersion(new ApiVersion(1.0))
            //    .ReportApiVersions()
            //    .Build();

            var routeGroup = app.MapGroup("/user")
                .WithName("UserService");
                ////.WithApiVersionSet(versionSet)
                //.MapToApiVersion(1.0);

            routeGroup.MapGet("/", userService.GetAllToUsersAsync)
                .WithName("GetAllToUsersAsync");

            routeGroup.MapGet("/{id}", userService.GetUserByIdAsync)
                .WithName("GetUserByIdAsync");
    
            routeGroup.MapGet("/page", (int id, int page, [FromHeader(Name = "token")] string token) =>
            {
                if (!string.IsNullOrEmpty(token) && token == "123456")
                    return Results.Ok("验证成功");
                Console.WriteLine($"{id}---{page}");
                return Results.Ok("请求成功");
            }).WithName("page");

            routeGroup.MapPost("/", ([FromBody] CreateUser createUser) => userService.CreateUserAsync(createUser))
                .WithName("CreateUserAsync");

            routeGroup.MapPut("/", ([AsParameters] EditUser editUser) => userService.UpdateUser(editUser))
                .WithName("UpdateUser");

            routeGroup.MapPost("/upload", async (IFormFile formFile) =>
            {
                var tempFile = Path.GetTempFileName();
                app.Logger.LogInformation(tempFile);
                using var stream = File.OpenWrite(tempFile);
                await formFile.CopyToAsync(stream);
            }).WithName("upload");

            routeGroup.MapPost("/upload_many", async (IFormFileCollection myFiles) =>
            {
                foreach (var file in myFiles)
                {
                    var tempFile = Path.GetTempFileName();
                    app.Logger.LogInformation(tempFile);
                    using var stream = File.OpenWrite(tempFile);
                    await file.CopyToAsync(stream);
                }
            }).WithName("upload_many");
        }
    }
}
