using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Ultraborg.Library.Sensor
{
    public class UltraborgSensor
    {
        private int SensorNo { get; set; }
        private Ultraborg? Ultraborg { get; set; }

        public UltraborgSensor(int sensorNo)
        {
            SensorNo = sensorNo;
        }

        public void Init(Ultraborg ultraborg)
        {
            Ultraborg = ultraborg;

        }

        public double GetDistance()
        {
            if (Ultraborg == null)
                return -100;
            return Ultraborg.GetDistance(SensorNo);
        }
    }
}
