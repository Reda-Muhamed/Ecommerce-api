using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm.Infrastructure
{
    public static class InfrastructureRegesteration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // Register DbContext
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    config.GetConnectionString("DefaultConnection")
                );
            });

            // Register repositories (example)
            // services.AddScoped<IUserRepository, UserRepository>();
            // services.AddScoped<IProductRepository, ProductRepository>();
            // services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Add other infrastructure services here (cache, email, sms, file storage, etc.)
            // services.AddScoped<IFileStorageService, CloudinaryStorageService>();

            return services;
        }
    }
}
