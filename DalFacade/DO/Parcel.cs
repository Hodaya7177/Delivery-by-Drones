using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{

    public struct Parcel
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int TargetId { get; set; }
        public WeightCategories Weight { get; set; }
        public Priorities Priority { get; set; }
        public int DroneId { get; set; }
        public DateTime? Requested { get; set; }
        public DateTime? Scheduled { get; set; }
        public DateTime? PickedUp { get; set; }
        public DateTime? Delivered { get; set; }

        public override string ToString()
        {
            return string.Format(
                "Id is: {0}\n" +
                "Id of the sender: {1}\n" +
                "Id of the target: {2}\n" +
                "Weight is: {3}\n" +
                "priority: {4}\n" +
                "requested date: {5}\n" +
                "Drone id: {6}\n" +
                "scheduled date: {7}\n" +
                "pickedUp date: {8}\n" +
                "delivered date: {9}\n",
                Id, SenderId, TargetId, Weight, Priority, Requested, DroneId, Scheduled, PickedUp, Delivered);
        }
    }
}
