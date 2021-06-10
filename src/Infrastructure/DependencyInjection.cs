using Application.Common.Interfaces;
using Infrastructure.Database;
using Infrastructure.Identity;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            //services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(databaseName: "BRaynov"));
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(
                    configuration.GetConnectionString("SqlightConnection"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                );
                options.EnableSensitiveDataLogging(env.IsDevelopment());
            });

            services.AddScoped<IAppDbContext>(provider => provider.GetService<AppDbContext>());

            // ===== Add Identity ========
            services
                .AddIdentity<ApplicationUser, ApplicationRole>(config =>
                {
                    config.User.RequireUniqueEmail = true;
                    config.SignIn.RequireConfirmedEmail = false;
                    config.Lockout.MaxFailedAccessAttempts = int.MaxValue;
                    config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                })
                .AddEntityFrameworkStores<AppDbContext>();

            // ===== Add Jwt Authentication ========
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Jwt:SecretKey"]))
                    };

                    //// SignalR
                    //cfg.Events = new JwtBearerEvents
                    //{
                    //    OnMessageReceived = context =>
                    //    {
                    //        var accessToken = context.Request.Query["access_token"];
                    //        // If the request is for our hub...
                    //        var path = context.HttpContext.Request.Path;
                    //        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    //        {
                    //            // Read the token out of the query string
                    //            context.Token = accessToken;
                    //        }
                    //        return Task.CompletedTask;
                    //    }
                    //};
                });

            services.AddScoped<ICurrentRequestService, CurrentRequestService>();
            services.AddSingleton<IDbCacheEntityManager, DbCacheEntityManager>();

            return services;
        }
    }
}
