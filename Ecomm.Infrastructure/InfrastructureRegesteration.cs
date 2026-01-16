using Ecomm.Core.Interfaces;
using Ecomm.Core.Services;
using Ecomm.Core.Validators;
using Ecomm.Infrastructure.Data;
using Ecomm.Infrastructure.Repositories;
using Ecomm.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
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
        // Extension method to register infrastructure services
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // Register DbContext
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    config.GetConnectionString("DefaultConnection")
                );
            });
            services.AddHttpContextAccessor(); // to access HttpContext in services 
            
            // register the validator to make SignUpDtoValidator and ....
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<SignUpDtoValidator>();
            // register token service
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IDeviceInfoProvider, DeviceInfoProvider>();
            services.AddScoped<IEmailService, SmtpEmailService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            //services.AddScoped<ITokenService,TokenService>();




            services.AddScoped<IUnitOfWork, UnitOfWork>();

            
            return services;
        }
    }
}
