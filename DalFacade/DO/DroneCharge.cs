using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DO
{
    public struct DroneCharge
    {
        public int DroneId { get; set; }
        public int StationId { get; set; }
        public DateTime ChargingTime { get; set; }

        public override string ToString()
        {
            return string.Format("Id of drone: {0}\nId of station: {1}\n", DroneId, StationId);
        }
    }

}
