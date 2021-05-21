using Contracts;
using Entities;
using Entities.DTO;
using HoaiBaoCompanyApi.ActionFilters;
using LoggerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repostitory;

namespace HoaiBaoCompanyApi.Extensions {
    public static class ServicesExtensions {
        public static void ConfigureCors(this IServiceCollection services) {
            services.AddCors(option => {
                option.AddPolicy("CorsPolicy", builder => {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        public static void ConfigureIis(this IServiceCollection service) {
            service.Configure<IISOptions>(options => { });
        }

        public static void ConfigureLoggerService(this IServiceCollection services) {
            services.AddScoped<ILoggerManager, LoggerManager>();
        }
        
        public static void ConfigureSqlServerConnection(this IServiceCollection service, IConfiguration configuration) {
            service.AddDbContext<RepositoryContext>(option => {
                option.UseSqlServer(configuration.GetConnectionString("SqlServerConnection"), 
                    b => {
                    b.MigrationsAssembly("HoaiBaoCompanyApi");
                });
            });
        }

    public static void ConfigureRepositoryManager(this IServiceCollection services) {
            services.AddScoped<IRepositoryManager, RepositoryManager>();
        }

        public static void ConfigureActionFilters(this IServiceCollection services) {
            services.AddScoped<ValidateActionFilterAttribute>();
            services.AddScoped<ValidateExistCompanyFilter>();
            services.AddScoped<ValidateExistEmployeeFilter>();
        }

        public static void ConfigureDataShaping(this IServiceCollection services) {
            services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();
        }
    }
}