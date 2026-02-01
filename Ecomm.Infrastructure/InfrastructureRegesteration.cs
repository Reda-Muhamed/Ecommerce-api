using Ecomm.Core.Interfaces;
using Ecomm.Core.Interfaces.ImageStorage;
using Ecomm.Core.Services;
using Ecomm.Core.Validators.Auth;
using Ecomm.Core.Validators.Products;
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
            services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IImageStorageService,CloudinaryImageStorageService>();
            services.AddScoped<IProductService, ProductSevice>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ISellerRepository, SellerRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IAdminAttributeService, AdminAttributeService>();
            services.AddScoped<IAttributeRepository, AttributeRepository>();
            services.AddScoped<IAttributeValueRepository, AttributeValueRepository>();
            services.AddScoped<IAdminCategoryService, AdminCategoryService>();
            services.AddScoped<ICategoryServise, CategoryService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IAdminBrandService, AdminBrandService>();

            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICartItemRepository, CartItemRepository>();

            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();




            // fluent validation for DTOs
            services.AddValidatorsFromAssemblyContaining<SignUpDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<ChangePasswordDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<RefreshTokenDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<ForgotPasswordDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<ResetPasswordDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>();



            return services;
        }
    }
}
