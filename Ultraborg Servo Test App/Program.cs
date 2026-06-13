using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ultraborg;
using Ultraborg.Library.Servo;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IUltraborgAPI, UltraborgAPI>();
    })
    .Build();

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(LogLevel.Information)
        .AddConsole();
});

ILogger logger = loggerFactory.CreateLogger<Program>();

var ultraborgAPI = host.Services.GetRequiredService<IUltraborgAPI>();

ultraborgAPI.Setup();
var ultraborg = ultraborgAPI?.Ultraborg;
var ultraborgServo = new UltraborgServo(4, 0, logger);
ultraborgServo.Init(ultraborg!);

int servoMax = ultraborgServo.ServoMax;
int servoMin = ultraborgServo.ServoMin;

logger.LogInformation($"Servo max : {servoMax} Servo min {servoMin}");
int currentPosition = ultraborgServo.GetCurrentPosition();
logger.LogInformation($"Servor Current Position: {currentPosition}");
logger.LogInformation($"Servo Position value {ultraborgServo.Postion}");

Thread.Sleep(100);
ultraborgServo!.SetServoPosition(-1);
logger.LogInformation($"Servor Current Position: {currentPosition}");
logger.LogInformation($"Servo Position value {ultraborgServo.Postion}");
var key = Console.ReadKey();
while (key.Key!= ConsoleKey.X)
{
    if (key.Key== ConsoleKey.RightArrow)
        ultraborgServo.RotateRight();
    if (key.Key== ConsoleKey.LeftArrow)
        ultraborgServo.RotateLeft();
    if (key.Key == ConsoleKey.Home)
        ultraborgServo.ServoTo0();
    if (key.Key == ConsoleKey.PageUp)
        ultraborgServo.ServoTo270();
    if (key.Key == ConsoleKey.PageDown)
        ultraborgServo.ServoTo90();

    currentPosition = ultraborgServo.GetCurrentPosition();
    logger.LogInformation($"Servor Current Position: {currentPosition}");
    logger.LogInformation($"Servo Position value {ultraborgServo.Postion}");
    key = Console.ReadKey();
}


