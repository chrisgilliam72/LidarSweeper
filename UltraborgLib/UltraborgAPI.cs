namespace Ultraborg
{
    using Iot.Device.Board;
    using Microsoft.Extensions.Logging;

    public interface IUltraborgAPI
    {
        public bool Setup();
        public double GetDistance(int sensorNo);

        public Ultraborg.Library.Ultraborg Ultraborg { get; }
    }

    public class UltraborgAPI : IUltraborgAPI
    {

        private readonly Ultraborg.Library.Ultraborg ultraborg;
        private readonly ILogger logger;
        public UltraborgAPI(ILoggerFactory loggerFactory)
        {
            logger=loggerFactory.CreateLogger("Ultraborg");
            ultraborg=new Ultraborg.Library.Ultraborg();
        }

        public Ultraborg.Library.Ultraborg Ultraborg => ultraborg;
        public bool Setup()
        {
            var raspberryPibrd = new RaspberryPiBoard();
            var controller = raspberryPibrd.CreateGpioController();

            int busNo = raspberryPibrd.GetDefaultI2cBusNumber();
            logger.LogInformation("Bus No:" + busNo);

            int ultraborgAddress = ultraborg.GetUltraBorgAdress();
            if (ultraborgAddress>0)
            {
                logger.LogInformation("Address: "+ultraborgAddress);
                ultraborg.Init(busNo,ultraborgAddress);
                return true;
            }

            logger.LogCritical("Ultraborg not detected");
            return false;
        }

        public double GetDistance(int sensorNo)
        {
            return ultraborg.GetFilteredDistance(sensorNo);
        }
    }
}