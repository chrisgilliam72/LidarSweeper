using LidarSweeper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Ultraborg;

namespace LidarLib
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddLidarSweepServices(this IServiceCollection services)
        {
            services.AddSingleton<IUltraborgAPI, UltraborgAPI>();
            services.AddSingleton<ILidarService, LidarService>();
            return services;
        }
    }
}
