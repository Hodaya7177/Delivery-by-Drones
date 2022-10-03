using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    class DroneCharge
    {
        public int DroneId { get; set; }
        //public double Battery;
        public int StationId { get; set; }

        public override string ToString()
        {
            return string.Format("Id of drone: {0}\nStation: {1}\n", DroneId, StationId);
        }
    }
}
