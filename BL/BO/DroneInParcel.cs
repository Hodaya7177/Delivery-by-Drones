using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class DroneInParcel
    {
        public int Id { get; set; }
        public double Battery { get; set; }
        public Location CurrentLocation { get; set; }


        public override string ToString()
        {
            return string.Format("Id: {0}\nBattery: {1}\nLocation: {2}", Id, (int)Battery, CurrentLocation);
        }
    }
}
