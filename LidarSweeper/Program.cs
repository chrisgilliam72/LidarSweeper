using Iot.Device.Board;
using LidarSweeper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ultraborg;
using Ultraborg.Library.Servo;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IUltraborgAPI, UltraborgAPI>();
        services.AddSingleton<ILidarService, LidarService>();

    })
    .Build();

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
   builder
       .SetMinimumLevel(LogLevel.Information)
       .AddConsole();
});

ILogger logger = loggerFactory.CreateLogger<Program>();


var lidar = host.Services.GetRequiredService<ILidarService>();
lidar.Init();
Console.WriteLine("Push any key to start scanning..");
Console.ReadKey();
await lidar.StartAsync();

Console.ReadLine();