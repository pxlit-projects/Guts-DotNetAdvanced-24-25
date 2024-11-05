using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuizApplication.AppLogic;
using QuizApplication.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.Tests.Web.Helpers
{
    // Source https://cezarypiatek.github.io/post/mocking-dependencies-in-asp-net-core/
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly Action<IServiceCollection>? _overrideDependencies;

        public CustomWebApplicationFactory(Action<IServiceCollection>? overrideDependencies = null)
        {
            _overrideDependencies = overrideDependencies;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services => _overrideDependencies?.Invoke(services));
        }

    }
}
