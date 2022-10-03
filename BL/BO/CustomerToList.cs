using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class CustomerToList
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Phone { get; init; }
        public int SentAndDelivered { get; init; }
        public int SentAndNotDeliverd { get; init; }
        public int Recieved { get; init; }
        public int InDeliveryToCustomer { get; init; }
        public bool isActive { get; set; }


        public override string ToString()
        {
            return string.Format(
                "Id is: {0}\n" +
                "Name of customer: {1}\n" +
                "Phone number: {2}\n" +
                "number of parcels that were sent and delivered by the customer: {3}\n" +
                "number of parcels that were sent and are not yet delivered: {4}\n" +
                "number of parcels the customer received: {5}\n" +
                "number of parcels that are on the way to the customer: {6}\n",
                Id, Name, Phone, SentAndDelivered, SentAndNotDeliverd, Recieved, InDeliveryToCustomer);
        }
    }



}
