using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public Location Location { get; set; }
        public List<ParcelInCustomer> Send { get; set; } //list of all the parcels that this customer sent
        public List<ParcelInCustomer> Receive { get; set; } //list of all the parcels that this customer received
        public bool isActive { get; set; } //Bonus - a boolean field that marks if the customer is active and wasn't deleted



        public override string ToString()
        {
            string senderParcels = string.Join(" , ", Send);
            string recieverParcels = string.Join(" , ", Receive);
            return string.Format(
                "Id is: {0}\n" +
                "Name of customer: {1}\n" +
                "Phone number: {2}\n" +
                "Location: {3}\nParcels from customer: {4}\nparcels to customer: {5}", Id, Name, Phone, Location, senderParcels, recieverParcels);
        }


    }

}
