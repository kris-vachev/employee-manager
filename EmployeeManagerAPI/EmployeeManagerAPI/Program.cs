using EmployeeManagerAPI.Helpers;
using EmployeeManagerAPI.Infrastructure.Helpers;
using EmployeeManagerAPI.Infrastructure.Interfaces;
using EmployeeManagerAPI.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace EmployeeManagerAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var configuration = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.Issuer,
                    ValidAudience = configuration.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.Secret)),
                    RoleClaimType = ClaimTypes.Role
                };
            });
            builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection("CacheSettings"));
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
            builder.Services.AddScoped<IDataProvider, DataProvider>();
            builder.Services.AddScoped<IDepartmentDataProvider, DepartmentDataProvider>();
            builder.Services.AddScoped<IEmployeeDataProvider, EmployeeDataProvider>();
            builder.Services.AddScoped<IUserDataProvider, UserDataProvider>();
            builder.Services.AddSingleton<ICacheManager, CacheManager>();
            builder.Services.AddMemoryCache();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // enable CORS
            var corsConfiguration = builder.Configuration.GetSection("CORSPolicySettings").Get<CORSPolicySettings>();
            builder.Services.AddCors(options => options.AddPolicy(name: corsConfiguration.Name,
                policy =>
                {
                    policy.WithOrigins(corsConfiguration.OriginUrl)
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.DisplayRequestDuration();
                });
            }

            app.UseCors(corsConfiguration.Name);

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}