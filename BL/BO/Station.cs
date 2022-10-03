using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Station
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Location Location { get; set; }
        public int AvailableChargeSlots { get; set; }
        public List<DroneInCharging> DronesCharging { get; set; }
        public bool isActive { get; set; }

        public override string ToString()
        {
            string droneInCharging = string.Join(" , ", DronesCharging);
            return string.Format(
                "Id is: {0}\n" +
                "Name of station: {1}\n" +
                "Location:\n{2}\n" +
                "Number of open charge slots: {3}\n"+
                "Drone in charging: {4}\n",
                Id, Name, Location, AvailableChargeSlots, droneInCharging);
        }
    }
}
