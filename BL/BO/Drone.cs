using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Drone
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public WeightCategories MaxWeight { get; set; }
        public double Battery { get; set; }
        public DroneStatus DroneStatus { get; set; }
        public ParcelInDelivey ParcelInDelivery { get; set; } 
        public Location CurrentLocation { get; set; }
        public bool isActive { get; set; }  //Bonus - a boolean field that marks if the drone is active and wasn't deleted

        public override string ToString()
        {
            return string.Format(
                "Id is: {0}\n" +
                "model: {1}\n" +
                "max weight is: {2}\n" +
                "battery: {3}%\n" +
                "drone status: {4}\n" +
                "parcel in delivery: \n{5}\n" +
                "current location: {6}\n",
                Id, Model, MaxWeight, (int)Battery, DroneStatus, ParcelInDelivery, CurrentLocation);
        }
    }
}
