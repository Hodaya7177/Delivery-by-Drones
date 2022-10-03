using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class ParcelInCustomer
    {
        public int Id { get; set; }
        public WeightCategories Weight { get; set; }
        public Priorities Priority { get; set; }
        public ParcelStatus ParcelStatus { get; set; }
        public CustomerInParcel TargetOrSender { get; set; }

        public override string ToString()
        {
            return string.Format(
                "Id is: {0}\n" +
                "Maximum Weight: {1}\n" +
                "Priority: {2}\n" +
                "Status of Parcel: {3}\n" +
                "Customer of this Parcel: {4}\n", Id, Weight, Priority, ParcelStatus, TargetOrSender.ToString());
        }

    }
}
