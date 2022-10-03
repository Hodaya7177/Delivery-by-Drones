using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Parcel
    {
        public int Id { get; set; }
        public CustomerInParcel Sender { get; set; }
        public CustomerInParcel Target { get; set; }
        public WeightCategories Weight { get; set; }
        public Priorities Priority { get; set; }
        public DroneInParcel AssignedDrone { get; set; }
        public DateTime? Requested { get; set; } // creating parcel time
        public DateTime? Scheduled { get; set; } // assigning drone to parcel time
        public DateTime? PickedUp { get; set; }  // pick up time
        public DateTime? Delivered { get; set; } // delivery time

        public override string ToString()
        {
            return string.Format(
                "Id is: {0}\n" +
                "Id of the sender: {1}\n" +
                "Id of the target: {2}\n" +
                "Weight is: {3}\n" +
                "priority: {4}\n" +
                "requested date: {5}\n" + 
                "scheduled date: {6}\n" +
                "pickedUp date: {7}\n" +
                "delivered date: {8}\n",
                Id, Sender.Id, Target.Id, Weight, Priority, Requested, Scheduled, PickedUp, Delivered);
        }
    }
}
