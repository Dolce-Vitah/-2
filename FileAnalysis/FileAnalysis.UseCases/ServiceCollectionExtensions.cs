using FileAnalysis.Domain.Interfaces;
using FileAnalysis.UseCases.Interfaces;
using FileAnalysis.UseCases.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysis.UseCases
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            services.AddScoped<IStatisticsService, StatisticsService>();

            return services;
        }
    }
}
