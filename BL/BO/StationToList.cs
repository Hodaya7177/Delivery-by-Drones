using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class StationToList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AvailableChargeSlots { get; set; }
        public int OccupiedChargeSlots { get; set; }
        public bool isActive { get; set; }  //Bonus - a boolean field that marks if the station is active and wasn't deleted

        public override string ToString()
        {
            return string.Format(
                "Id is: {0}\n" +
                "Name of station: {1}\n" +
                "number of available charging slots: {2}\n" +
                "Number of occupied charging slots: {3}\n",
                Id, Name, AvailableChargeSlots, OccupiedChargeSlots);
        }
    }
}
