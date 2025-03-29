using BlindBoxShop.Application.Security;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlindBoxShop.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
           services.AddDbContextFactory<RepositoryContext>(opts => opts.UseSqlServer(configuration.GetConnectionString("sqlConnection"), opts =>
           {
               opts.EnableRetryOnFailure();
           }));

        public static void ConfigureAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();
        }

        public static void ConfigureIdentityCore(this IServiceCollection services)
        {
            services.AddIdentityCore<User>(options =>
            {
                // Password settings - less strict for development
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false; // Không yêu cầu ký tự đặc biệt
                options.Password.RequiredLength = 6; // Giảm độ dài mật khẩu xuống 6
                options.Password.RequiredUniqueChars = 1;

                // User settings
                options.User.RequireUniqueEmail = true;
                
                // Không yêu cầu xác nhận email khi đăng nhập (chỉ trong môi trường phát triển)
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                // Claims identity settings
                options.ClaimsIdentity.UserNameClaimType = "username";
                options.ClaimsIdentity.RoleClaimType = "role";

                // Lockout settings - more lenient for development
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Token providers
                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
            })
                .AddRoles<Roles>()
                .AddEntityFrameworkStores<RepositoryContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders().AddTokenProvider<CustomEmailConfirmationTokenProvider<User>>("CustomEmailConfirmation");

            services.Configure<DataProtectionTokenProviderOptions>(o =>
            {
                o.TokenLifespan = TimeSpan.FromHours(5);
            });

            services.Configure<CustomEmailConfirmationTokenProviderOptions>(o =>
            {
                o.TokenLifespan = TimeSpan.FromDays(3);
            });
        }

        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
            services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureServiceManager(this IServiceCollection services) =>
            services.AddScoped<IServiceManager, ServiceManager>();
    }
}
