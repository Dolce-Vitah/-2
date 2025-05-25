using FileStorage.Domain.Interfaces;
using FileStorage.Infrastructure.Database;
using FileStorage.Infrastructure.FileSystem;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<NpgsqlConnection>(_ =>
            {
                var connectionString = config.GetConnectionString("DefaultConnection");
                var conn = new NpgsqlConnection(connectionString);
                conn.Open();
                return conn;
            });

            services.AddDbContext<FileDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IFileStorage, LocalFileStorage>();

            return services;
        }
    }
}
