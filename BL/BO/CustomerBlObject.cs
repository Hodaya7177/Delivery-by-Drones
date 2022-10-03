using BlApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace BL
{
    public partial class BlObject : IBL
    {
        /// <summary>
        /// the fucntion receives an updated existing customer and updates the old one 
        /// </summary>
        /// <param name="newCustomer"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateCustomer(BO.Customer newCustomer)
        {
            try
            {
                lock (dal)
                {
                    DO.Customer dalCustomer = dal.GetCustomer(newCustomer.Id);
                    if (dalCustomer.isActive == false)
                        throw new BO.NoMatchingIdException($"customer with id {newCustomer.Id} doesn't exist !!");
                    if (newCustomer.Name == "" && newCustomer.Phone == "")
                        throw new BO.NoUpdateException("no update to customer was received\n");
                    if (newCustomer.Name != "")
                        dalCustomer.Name = newCustomer.Name;
                    if (newCustomer.Phone != "")
                        dalCustomer.Phone = newCustomer.Phone;
                    dal.UpdateCustomer(dalCustomer);
                }
            }
            catch (DO.NoMatchingIdException)
            {
                throw new BO.NoMatchingIdException($"customer with id {newCustomer.Id} doesn't exist !!");
            }
        }

        /// <summary>
        /// the function receives a new customer and adds if to the customer list in dal
        /// </summary>
        /// <param name="newCustomer"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddCustomer(BO.Customer newCustomer)
        {
            try
            {
                DO.Customer dalCustomer = new DO.Customer
                {
                    Id = newCustomer.Id,
                    Name = newCustomer.Name,
                    Phone = newCustomer.Phone,
                    Longitude = newCustomer.Location.Longitude,
                    Latitude = newCustomer.Location.Latitude,
                    isActive = true
                };
                dal.AddCustomer(dalCustomer);
            }
            catch (DO.IdAlreadyExistsException)
            {
                throw new BO.IdAlreadyExistsException($"drone with id {newCustomer.Id} already exists !!");
            }
        }

        /// <summary>
        /// the function receives a DO customer and converts it to BO custoerToList
        /// </summary>
        /// <param name="dalCustomer">dal customer to convert</param>
        /// <returns>the converted bo customer to list</returns>
        private BO.CustomerToList convertCustomerToCustomerToList(DO.Customer dalCustomer)
        {
            int sentAndDelivered = 0, sentAndNotDelivered = 0, received = 0, inDeliveryToCustomer = 0;
            foreach (var p in dal.GetParcels())
            {
                if (p.SenderId == dalCustomer.Id)
                {
                    if (getParcelStatus(p) == BO.ParcelStatus.Delivered)
                        sentAndDelivered++;
                    else sentAndNotDelivered++;
                }
                else if (p.TargetId == dalCustomer.Id)
                {
                    if (getParcelStatus(p) == BO.ParcelStatus.Delivered)
                        received++;
                    else
                        inDeliveryToCustomer++;
                }
            }
            BO.CustomerToList blCustomer = new BO.CustomerToList
            {
                Id = dalCustomer.Id,
                Name = dalCustomer.Name,
                Phone = dalCustomer.Phone,
                SentAndDelivered = sentAndDelivered,
                SentAndNotDeliverd = sentAndNotDelivered,
                Recieved = received,
                InDeliveryToCustomer = inDeliveryToCustomer,
                isActive = dalCustomer.isActive
            };
            return blCustomer;
        }

        /// <summary>
        /// the function return a customer by an id
        /// </summary>
        /// <param name="id">an id of acustomer to find</param>
        /// <returns>the customer the id thar was received</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public BO.Customer GetCustomer(int id)
        {
            try
            {
                lock (dal)
                {
                    DO.Customer dalCustomer = dal.GetCustomer(id);
                    List<DO.Parcel> parcelsOfsender = dal.GetParcels().ToList().FindAll(p => p.SenderId == id);
                    List<DO.Parcel> parcelsOfreceiver = dal.GetParcels().ToList().FindAll(p => p.TargetId == id);
                    BO.Customer customer = new BO.Customer
                    {
                        Id = dalCustomer.Id,
                        Name = dalCustomer.Name,
                        Phone = dalCustomer.Phone,
                        Location = new BO.Location { Latitude = dalCustomer.Latitude, Longitude = dalCustomer.Longitude },
                        Send = convertParcelsToParcelsCustomer(parcelsOfsender, id).ToList(),
                        Receive = convertParcelsToParcelsCustomer(parcelsOfreceiver, id).ToList(),
                        isActive = dalCustomer.isActive
                    };
                    return customer;
                }
            }
            catch (DO.NoMatchingIdException ex)
            {
                throw new BO.NoMatchingIdException(ex.Message);
            }
        }

        /// <summary>
        /// the functon receives a list of parcels and an id of customer and returns the list of parcels that belong to that cusstomer
        /// </summary>
        /// <param name="parcelsOfSender">list of parcels</param>
        /// <param name="id">id of customer to dinf his parcels</param>
        /// <returns>list of the customer's parecls</returns>
        private IEnumerable<BO.ParcelInCustomer> convertParcelsToParcelsCustomer(List<DO.Parcel> parcelsOfSender, int id)
        {
            List<BO.ParcelInCustomer> parcels = new List<BO.ParcelInCustomer>();
            foreach (var item in parcelsOfSender)
            {
                parcels.Add(convertParcelToParcelInCustomer(item, id));
            }
            return parcels;
        }

        /// <summary>
        /// the function receives a DO parcel and an id of a customer and converts the parcel to o parcalInCustomer of the customer 
        /// </summary>
        /// <param name="parcel">parcel to convert</param>
        /// <param name="id">id of the parcel owner</param>
        /// <returns>the parcel converted to parcelInCustomer</returns>
        private BO.ParcelInCustomer convertParcelToParcelInCustomer(DO.Parcel parcel, int id)
        {
            BO.ParcelInCustomer parcelInCustomer = new BO.ParcelInCustomer
            {
                Id = parcel.Id,
                Weight = (BO.WeightCategories)parcel.Weight,
                Priority = (BO.Priorities)parcel.Priority,
                ParcelStatus = getParcelStatus(parcel),
                TargetOrSender = new BO.CustomerInParcel
                {
                    Id = id,
                    Name = dal.GetCustomer(id).Name
                }
            };
            return parcelInCustomer;
        }

        /// <summary>
        /// the function return the the list of customers converted to BO customerTo/kist
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<BO.CustomerToList> GetListOfCustomers()
        {
            List<BO.CustomerToList> customers = new List<BO.CustomerToList>();
            foreach (var dalCustomer in dal.GetCustomers())
            {
                customers.Add(convertCustomerToCustomerToList(dalCustomer));
            }
            return customers;
        }

        /// <summary>
        /// help function that returns a list of customers that have parcels which still weren't delivered
        /// </summary>
        private IEnumerable<DO.Customer> GetCustomersWithParcels()
        {
            lock (dal)
            {
                List<DO.Customer> clients = new List<DO.Customer>();
                foreach (var customer in dal.GetCustomers())
                {
                    if (customer.isActive)
                    {
                        foreach (var parcel in dal.GetParcels())
                        {
                            if (customer.Id == parcel.TargetId && parcel.Delivered != null)
                            {
                                DO.Customer client = dal.GetCustomer(parcel.TargetId);
                                clients.Add(client);
                            }
                        }
                    }
                }
                return clients;
            }
        }

        public void DeleteCustomer(int id)
        {
            try
            {
                dal.DeleteCustomer(dal.GetCustomer(id));
            }
            catch(DO.NoMatchingIdException ex)
            {
                throw new BO.NoMatchingIdException(ex.Message);
            }
        }
       
    }
}
