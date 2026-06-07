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

var ultraborgAPI= host.Services.GetRequiredService<IUltraborgAPI>();
var lidar = host.Services.GetRequiredService<ILidarService>();

ultraborgAPI.Setup();
var ultraborg = ultraborgAPI?.Ultraborg;
var ultraborgServo = new UltraborgServo(4, 0,loggerFactory);
ultraborgServo.Init(ultraborg!);
int currentPosition = ultraborgServo.GetCurrentPosition();
logger.LogInformation($"Current Position: {currentPosition}");
int servoMax= ultraborgServo.ServoMax;
int servoMin= ultraborgServo.ServoMin;
ultraborgServo.SetServoPosition(-0.95);
currentPosition = ultraborgServo.GetCurrentPosition();
logger.LogInformation($"Start Position: {currentPosition}");
await lidar.StartAsync();

double pos=-1;
while (pos <= 1)
{
    pos+=0.1;
    ultraborgServo.SetServoPosition(pos);
    Thread.Sleep(100);
    currentPosition = ultraborgServo.GetCurrentPosition();
    logger.LogInformation($"Current Position: {currentPosition}");
    logger.LogInformation(
    "Distance={Distance} cm  Strength={Strength}  Temp={Temperature:F1}°C",
    lidar.Distance,
    lidar.Strength,
    lidar.Temperature);
}

   


Console.ReadLine();