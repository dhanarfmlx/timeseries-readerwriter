using System;
using System.Collections.Generic;
using System.Text;

namespace TimeSeries.ReaderWriter
{
    class SensorData
    {
        public string datetime { get; set; }
        public double[] sensorData { get; set; }
    }
}
