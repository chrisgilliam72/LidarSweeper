using Iot.Device.Board;
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

var ultraborgAPI= host.Services.GetService<IUltraborgAPI>();
if (ultraborgAPI != null)
{
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
    double pos=-1;
    while (pos <= 1)
    {
        pos+=0.1;
        ultraborgServo.SetServoPosition(pos);
        Thread.Sleep(100);
        currentPosition = ultraborgServo.GetCurrentPosition();
        logger.LogInformation($"Current Position: {currentPosition}");
    }
}
   


//var raspberryPibrd = new RaspberryPiBoard();
//var controller = raspberryPibrd.CreateGpioController();
//int busNo = raspberryPibrd.GetDefaultI2cBusNumber();

//using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
//{
//    builder
//        .SetMinimumLevel(LogLevel.Information)
//        .AddConsole();
//});

//ILogger logger = loggerFactory.CreateLogger<Program>();

//logger.LogInformation("Bus No: {BusNo}", busNo);

//var ultraborg = new Ultraborg.Library.Ultraborg(logger);
//Console.WriteLine("Scanning for Ultraborg address...");
//int address = ultraborg.GetUltraBorgAdress();
//if (address != -1)
//{
//    Console.WriteLine($"UltraBorg found on address {address}");
//    ultraborg.Init(busNo, address);

//    while (true)
//    {
//        var keyInfo = Console.ReadKey();
//        switch (keyInfo.KeyChar)
//        {

//            case 'd': var distance = ultraborg.GetFilteredDistance(1); Console.WriteLine($"Filtered Distance: {distance}"); break;
//            case 'u': var unfiltDistance = ultraborg.GetDistance(1); ; Console.WriteLine($"Unfiltered Distance: {unfiltDistance}"); break;
//            case 'x': Environment.Exit(0); break;

//        }
//    }
//}

Console.ReadLine();