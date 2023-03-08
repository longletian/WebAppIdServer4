using Autofac.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using MiniAPI.Model;
using MiniAPI.Services;
using MiniAPI.Services.IService;

namespace MiniAPI.Map
{
    public static class Identity
    {
        public static void GetUserIdentity(this WebApplication app)
        {
            var routeGroup = app.MapGroup("/identity");
            routeGroup.MapGet("/", () =>
            {
                //return 
            });

        }

    }
}
