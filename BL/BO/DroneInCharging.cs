using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class DroneInCharging
    {
        public int Id { get; set; }
        public double Battery { get; set; }

        public override string ToString()
        {
            return string.Format("Id is: {0}\n Battery is: {1}", Id, Battery);           
        }
    }
}
