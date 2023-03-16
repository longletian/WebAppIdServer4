using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace WebAppIdServer4.Data
{
    public class SeedData : IHostedService
    {
        private readonly IServiceProvider serviceProvider;

        public SeedData(IServiceProvider _serviceProvider)
        {
            serviceProvider = _serviceProvider;
        }

        private static void EnsureSeedData(ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                Console.WriteLine("Clients 正在初始化");
                foreach (var client in IdentityConfig.GetClients())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                Console.WriteLine("IdentityResources 正在初始化");
                foreach (var resource in IdentityConfig.IdentityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                Console.WriteLine("ApiResources 正在初始化");
                foreach (var resource in IdentityConfig.ApiResources)
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                Console.WriteLine("ApiScopes 正在初始化");
                foreach (var resource in IdentityConfig.GetApiScopes())
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }


        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>() ?? throw new Exception("");
            bool.TryParse(configuration.GetSection("IsSeed").ToString(), out bool isSeed);
            if (isSeed)
            {
                Console.WriteLine("Seeding database...");
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    scope.ServiceProvider.GetService<PersistedGrantDbContext>()
                        .Database.Migrate();

                    scope.ServiceProvider.GetService<IdentityDataContext>()
                        .Database.Migrate();

                    var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                    context.Database.Migrate();

                    EnsureSeedData(context);
                }
                Console.WriteLine("Done seeding database.");
                Console.WriteLine();
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
