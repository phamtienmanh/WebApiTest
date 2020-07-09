using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApiTest.DataAccess.Contexts;

namespace WebApiTest.Infrastructure.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(o =>
            {
                o.UseSqlServer(configuration.GetConnectionString(nameof(AppDbContext)));
            });
        }
    }
}
