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
        /// th function receives a parcel and adds it to the parcels list in dal
        /// </summary>
        /// <param name="newParcel"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddParcel(BO.Parcel newParcel)
        {
            try
            {
                lock (dal)
                {
                    //check if the ID of sender and receiver exist. if not, throw an exception

                    if (!GetListOfCustomers().ToList().Exists(c => c.Id == newParcel.Sender.Id))
                    {
                        throw new BO.NoMatchingIdException($"ID of sender {newParcel.Sender.Id} doesn't exist!");
                    }
                    if (!GetListOfCustomers().ToList().Exists(c => c.Id == newParcel.Target.Id))
                    {
                        throw new BO.NoMatchingIdException($"ID of receiver {newParcel.Target.Id} doesn't exist!");
                    }

                    DO.Parcel parcel = new DO.Parcel
                    {
                        SenderId = newParcel.Sender.Id,
                        TargetId = newParcel.Target.Id,
                        Weight = (DO.WeightCategories)newParcel.Weight,
                        Priority = (DO.Priorities)newParcel.Priority,
                        Requested = DateTime.Now,
                        Scheduled = null,
                        PickedUp = null,
                        Delivered = null,
                        DroneId = 0
                    };
                    dal.AddParcel(parcel);
                }
            }
            catch (DO.IdAlreadyExistsException ex)
            {
                throw new BO.IdAlreadyExistsException(ex.Message);
            }
            catch(DO.NoMatchingIdException ex)
            {
                throw new BO.NoMatchingIdException(ex.Message);
            }
        }

        /// <summary>
        /// the function receives an id and rerturns the parcel eith that id
        /// </summary>
        /// <param name="id">id of parcel to search</param>
        /// <returns>the parcel with the id that was received</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public BO.Parcel GetParcel(int id)
        {
            try
            {
                lock (dal)
                {
                    DO.Parcel dalParcel = dal.GetParcel(id);
                    BO.Parcel blParcel = new BO.Parcel
                    {
                        Id = dalParcel.Id,
                        Sender = new BO.CustomerInParcel { Id = dalParcel.SenderId, Name = dal.GetCustomer(dalParcel.SenderId).Name },
                        Target = new BO.CustomerInParcel { Id = dalParcel.TargetId, Name = dal.GetCustomer(dalParcel.TargetId).Name },
                        Weight = (BO.WeightCategories)dalParcel.Weight,
                        Priority = (BO.Priorities)dalParcel.Priority,
                        Requested = dalParcel.Requested,
                        Scheduled = dalParcel.Scheduled,
                        PickedUp = dalParcel.PickedUp,
                        Delivered = dalParcel.Delivered,
                        AssignedDrone = dalParcel.DroneId != 0 ? new BO.DroneInParcel { Id = dalParcel.DroneId, Battery = dronesToList.Find(d => d.Id == dalParcel.DroneId).Battery, CurrentLocation = dronesToList.Find(d => d.Id == dalParcel.DroneId).Location } : new BO.DroneInParcel { Id = 0, Battery = 0, CurrentLocation = new BO.Location { Latitude = 0, Longitude = 0 } }
                    };
                    return blParcel;
                }
            }
            catch (DO.NoMatchingIdException ex)
            {
                throw new BO.NoMatchingIdException(ex.Message);
            }
            catch (DO.IdAlreadyExistsException ex)
            {
                throw new BO.IdAlreadyExistsException(ex.Message);
            }
        }

        /// <summary>
        /// create a list of parcelsToList and returns it
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<BO.ParcelToList> GetListofParcels()
        {
            lock (dal)
            {
                List<BO.ParcelToList> blParcels = new List<BO.ParcelToList>();
                foreach (var dalParcel in dal.GetParcels())
                {
                    blParcels.Add(convertParcelToParcelToList(dalParcel));
                }
                return blParcels;
            }
        }

        /// <summary>
        /// the function receives a DO parcel and returns the parcel converted to BO
        /// </summary>
        /// <param name="dalParcel">dal parcel to convert</param>
        /// <returns>the converted BO parcel</returns>
        private BO.ParcelToList convertParcelToParcelToList(DO.Parcel dalParcel)
        {
            BO.ParcelToList blParcel = new BO.ParcelToList
            {
                Id = dalParcel.Id,
                SenderName = GetCustomer(dalParcel.SenderId).Name,
                TargetName = GetCustomer(dalParcel.TargetId).Name,
                Weight = (BO.WeightCategories)dalParcel.Weight,
                Priority = (BO.Priorities)dalParcel.Priority,
                ParcelStatus = getParcelStatus(dalParcel)
            };
            return blParcel;
        }

        /// <summary>
        /// the function receives a parcel and returns the parcel's status
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns>status of the parcel</returns>
        private BO.ParcelStatus getParcelStatus(DO.Parcel parcel)
        {
            if (parcel.Scheduled == null)
                return BO.ParcelStatus.Requested;
            else if (parcel.PickedUp == null)
                return BO.ParcelStatus.Assigned;
            else if (parcel.Delivered == null)
                return BO.ParcelStatus.PickedUp;
            else return BO.ParcelStatus.Delivered;
        }

        // <summary>
        // create a list of parcels that are not assigned to drone and returns this list
        // </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<BO.ParcelToList> GetListofParcelsWithoutDrone()
        {
            lock (dal)
            {
                List<BO.ParcelToList> parcelsWithoutDrones = new List<BO.ParcelToList>();
                foreach (var dalParcel in dal.GetParcels(p => p.DroneId == 0)) //send a predicate to DAL
                {
                    parcelsWithoutDrones.Add(convertParcelToParcelToList(dalParcel));
                }
                return parcelsWithoutDrones;
            }
        }

        /// <summary>
        /// get ID of parcel and delete it
        /// </summary>
        /// <param name="id"></param>
        public void DeleteParcel(int id)
        {
            try
            {
                dal.DeleteParcel(dal.GetParcel(id));
            }
            catch(DO.NoMatchingIdException ex)
            {
                throw new BO.NoMatchingIdException(ex.Message);
            }

        }
    }
}
