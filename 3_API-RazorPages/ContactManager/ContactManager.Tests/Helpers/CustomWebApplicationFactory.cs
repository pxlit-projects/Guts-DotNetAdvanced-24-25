using ContactManager.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContactManager.Tests.Helpers
{
    // Source https://cezarypiatek.github.io/post/mocking-dependencies-in-asp-net-core/
    public class CustomWebApplicationFactory(Action<IServiceCollection>? overrideDependencies = null)
        : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseTestServer();
            builder.ConfigureTestServices(services =>
            {
                overrideDependencies?.Invoke(services);

                services.AddDbContext<ContactManagerDbContext>(options =>
                {
                    string connectionString = "DataSource=:memory:";
                    options.UseSqlite(connectionString);
                });

                var serviceProvider = services.BuildServiceProvider();

                using var scope = serviceProvider.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var dbContext = scopedServices.GetRequiredService<ContactManagerDbContext>();

                dbContext.Database.EnsureDeleted();
                dbContext.Database.Migrate();
            });
        }

    }
}
