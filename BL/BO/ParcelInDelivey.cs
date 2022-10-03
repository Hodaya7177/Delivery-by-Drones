using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class ParcelInDelivey
    {
        public int Id { get; set; }
        public bool PickUpStatus { get; set; }
        public WeightCategories Weight { get; set; }
        public Priorities Priority { get; set; }
        public CustomerInParcel Sender { get; set; }
        public CustomerInParcel Target { get; set; }
        public Location PickUpLocation { get; set; }
        public Location TargetLocation { get; set; }
        public double Distance { get; set; }


        public override string ToString()
        {
            return string.Format("Id is: {0}\n" +
                "Pick-up ststus: {1}\n" +
                "Weight is: {2}\n" +
                "priority: {3}\n" +
                "Sender: \n{4}\n" +
                "Target: \n{5}\n" +
                "Pick-up locatoin: {6}\n" +
                "Target location: {7}\n" +
                "Distance from the sender to the target: {8}KM\n",
                Id, PickUpStatus == true? "was picked up" : "wasn't picked up", Weight, Priority, Sender.ToString(), Target.ToString(), PickUpLocation, TargetLocation, (float)Distance);
        }
    }


}
