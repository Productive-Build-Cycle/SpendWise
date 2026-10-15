using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpendWise.Application.Interfaces;
using SpendWise.Infrastructure.Authentication.Jwt;
using SpendWise.Infrastructure.Authentication.RefreshTokens;
using SpendWise.Infrastructure.Identity;
using SpendWise.Infrastructure.Persistence;
using SpendWise.Infrastructure.Persistence.Repositories;

namespace SpendWise.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException(
                                   "Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = false;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager();

        var refreshTokenOptions =
            configuration.GetSection(RefreshTokenOptions.SectionName).Get<RefreshTokenOptions>() ??
            new RefreshTokenOptions();
        refreshTokenOptions.Validate();
        services.AddSingleton(refreshTokenOptions);

        refreshTokenOptions.Validate();

        services.AddSingleton(refreshTokenOptions);
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITransactionManager, EfTransactionManager>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddSingleton<IRefreshTokenService, RefreshTokenService>();

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>() ?? throw new InvalidOperationException("JWT configuration is missing.");

        jwtOptions.Validate();

        var jwtKeyProvider = new PemJwtKeyProvider(jwtOptions);

        services.AddSingleton(jwtOptions);

        services.AddSingleton<IJwtKeyProvider>(_ => jwtKeyProvider);

        services.AddSingleton<IJwtService, JwtService>();

        return services;
    }
}