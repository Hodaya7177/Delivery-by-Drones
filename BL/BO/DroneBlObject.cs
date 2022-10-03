using BlApi;
using BO;
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
        /// the function receives a new drone to add and a station id to put the drone to charge in that station
        /// </summary>
        /// <param name="newDrone">drone to add</param>
        /// <param name="stationId">id of station to put the drone in</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddDrone(Drone newDrone, int stationId)
        {
            try
            {
                lock (dal)
                {
                    Station station = GetStation(stationId);
                    DroneToList drone = new DroneToList
                    {
                        Id = newDrone.Id,
                        Model = newDrone.Model,
                        MaxWeight = newDrone.MaxWeight,
                        Battery = r.Next(20, 41),
                        DroneStatus = DroneStatus.Maintenance,
                        Location = station.Location,
                        ParcelInDeliveryId = 0,
                        isActive = true
                    };
                    dronesToList.Add(drone);

                    DO.Drone dalDrone = new DO.Drone
                    {
                        Id = newDrone.Id,
                        Model = newDrone.Model,
                        MaxWeight = (DO.WeightCategories)newDrone.MaxWeight,
                        isActive = true
                    };
                    dal.AddDrone(dalDrone);
                    DO.Station dalStation = dal.GetStation(stationId);
                    dal.SendDroneToCharge(dalStation, dalDrone);
                }
            }
            catch (DO.IdAlreadyExistsException)
            {
                throw new IdAlreadyExistsException($"drone with id {newDrone.Id} already exists !!");
            }
            catch (DO.NoMatchingIdException ex)
            {
                throw new NoMatchingIdException(ex.Message);
            }
        }

        /// <summary>
        /// the function receives a drone with a possible change in the model and updates the old one
        /// </summary>
        /// <param name="newDrone"> the updated drone </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDrone(Drone newDrone)
        {
            try
            {
                DO.Drone dalDrone = dal.GetDrone(newDrone.Id);
                DroneToList blDrone = GetDroneToList(newDrone.Id);
                if (dalDrone.isActive)
                {
                    if (newDrone.Id.ToString() != "" && newDrone.Model != "")
                    {
                        dalDrone.Model = newDrone.Model;
                        dal.UpdateDrone(dalDrone);
                        blDrone.Model = newDrone.Model;
                        UpdateBlDrone(blDrone);
                    }
                    else throw new NoUpdateException("no update was received\n");
                }
            }
            catch (DO.NoMatchingIdException)
            {
                throw new NoMatchingIdException($"drone with id {newDrone.Id} doesn't exist !!");
            }
        }

        /// <summary>
        ///  the function receives an updated drone, deletes the old drone with the same id and adds the new one
        /// </summary>
        /// <param name="newDrone"> the updated drone </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateBlDrone(DroneToList newDrone)
        {
            dronesToList.Remove(GetDroneToList(newDrone.Id));
            dronesToList.Add(newDrone);
        }

        /// <summary>
        /// the function receives a drone id and sends it to charge in the closest station 
        /// </summary>
        /// <param name="id">id of drone to charge</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RechargeDrone(int id)
        {
            try
            {
                Drone drone = GetDrone(id);
                if (!drone.isActive)
                    throw new NoMatchingIdException($"drone with id {id} in not active !!");
                if (drone.DroneStatus != DroneStatus.Available)
                    throw new ImpossibleOprationException("Drone can't be sent to recharge");

                DO.Station tempStation = getClosestStation(drone.CurrentLocation.Latitude, drone.CurrentLocation.Longitude);
                double distance = Tools.Utils.DistanceCalculation(drone.CurrentLocation.Latitude, drone.CurrentLocation.Longitude, tempStation.Latitude, tempStation.Longitude);

                if (tempStation.AvailableChargeSlots > 0 && distance * whenAvailable < drone.Battery)
                {
                    DO.Drone dalDrone = ConvertDroneToDal(drone);
                    dal.SendDroneToCharge(tempStation, dalDrone);

                    DroneToList newDrone = new DroneToList
                    {
                        Id = drone.Id,
                        MaxWeight = drone.MaxWeight,
                        Model = drone.Model,
                        Battery =  Math.Max(0.0,drone.Battery - distance * whenAvailable),
                        Location = new Location { Latitude = tempStation.Latitude, Longitude = tempStation.Longitude },
                        DroneStatus = DroneStatus.Maintenance,
                        isActive = true
                    };
                    DroneToList oldDrone = GetDroneToList(drone.Id);
                    dronesToList.Remove(oldDrone);
                    dronesToList.Add(newDrone);
                }
                else throw new ImpossibleOprationException("Drone can't be sent to recharge");
            }
            catch (DO.NoMatchingIdException ex)
            {
                throw new NoMatchingIdException(ex.Message);
            }
            catch (BO.ImpossibleOprationException ex)
            {
                throw new ImpossibleOprationException(ex.Message);
            }
            catch (BO.NoMatchingIdException ex)
            {
                throw new BO.NoMatchingIdException(ex.Message);
            }
        }

        /// <summary>
        /// Release drone from charging and update the fields accordingly
        /// </summary>
        /// <param name="droneId"></param>
        /// <param name="timeInCharging"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void FreeDrone(int droneId)
        {
            try
            {
                DroneToList blDrone = GetDroneToList(droneId);
                if (blDrone.DroneStatus == DroneStatus.Maintenance)
                {
                    lock (dal)
                    {
                        dronesToList.Remove(blDrone);
                        DO.DroneCharge dc = dal.GetDroneCharge(blDrone.Id);

                        TimeSpan timeOfRelease = DateTime.Now - dc.ChargingTime; //calculate the time of charging
                        blDrone.Battery += Math.Min(timeOfRelease.TotalMinutes * chargeRate, 100);
                        blDrone.DroneStatus = DroneStatus.Available;
                        dronesToList.Add(blDrone);

                        DO.Drone dalDrone = dal.GetDrone(droneId);
                        DO.Station station = dal.GetStation(dc.StationId);
                        dal.ReleaseDroneFromCharge(station, dalDrone);
                    }
                }
                else throw new ImpossibleOprationException("drone can't be free from chraging\n");
            }
            catch (DO.NoMatchingIdException)
            {
                throw new NoMatchingIdException($"drone with id {droneId} doesn't exist !!");
            }
            catch (BO.ImpossibleOprationException ex)
            {
                throw new BO.ImpossibleOprationException(ex.Message);
            }
        }

        /// <summary>
        /// the function receives an id and returns the bl droneToList with the same id
        /// </summary>
        /// <param name="id"> id of the drone to search </param>
        /// <returns> a bl droneToList with the id that was received </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DroneToList GetDroneToList(int id)
        {         
            DroneToList tmp = new DroneToList();
            if (dronesToList.Exists(d => d.Id == id))
                return dronesToList.Find(d => d.Id == id);
             else throw new NoMatchingIdException($"drone with id {id} doesn't exist !!");
        }


        /// <summary>
        /// the function receives an id and returns the bl drone with the same id
        /// </summary>
        /// <param name="id"> id of the drone to search </param>
        /// <returns> a bl drone with the id that was received </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone GetDrone(int id)
        {
            Drone d = ConvertDroneToListToDrone(GetDroneToList(id));
            return d;
        }

        /// <summary>
        /// the function receives a bl droneToList and returns the equal dal drone
        /// </summary>
        /// <param name="d"> the bl droneToList </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone ConvertDroneToListToDrone(DroneToList d)
        {
            Drone newDrone = new Drone();
            try
            {
                lock (dal)
                {
                    ParcelInDelivey parcelInDrone;
                    if (d.ParcelInDeliveryId == 0)
                    {
                        parcelInDrone = new ParcelInDelivey();
                    }
                    else
                    {

                        DO.Parcel parcel = dal.GetParcel(d.ParcelInDeliveryId);
                        parcelInDrone = new ParcelInDelivey
                        {
                            Id = parcel.Id,
                            PickUpStatus = getParcelStatus(parcel) == ParcelStatus.PickedUp || getParcelStatus(parcel) == ParcelStatus.Delivered ? true : false,
                            Weight = (WeightCategories)parcel.Weight,
                            Priority = (Priorities)parcel.Priority,
                            Sender = new CustomerInParcel { Id = dal.GetCustomer(parcel.SenderId).Id, Name = dal.GetCustomer(parcel.SenderId).Name },
                            Target = new CustomerInParcel { Id = dal.GetCustomer(parcel.TargetId).Id, Name = dal.GetCustomer(parcel.TargetId).Name },
                            PickUpLocation = new Location { Latitude = dal.GetCustomer(parcel.SenderId).Latitude, Longitude = dal.GetCustomer(parcel.SenderId).Longitude },
                            TargetLocation = new Location { Latitude = dal.GetCustomer(parcel.TargetId).Latitude, Longitude = dal.GetCustomer(parcel.TargetId).Longitude },
                            //calculate the distance between the sender and the target:
                            Distance = Tools.Utils.DistanceCalculation(dal.GetCustomer(parcel.SenderId).Latitude, dal.GetCustomer(parcel.SenderId).Longitude, dal.GetCustomer(parcel.TargetId).Latitude, dal.GetCustomer(parcel.TargetId).Longitude)
                        };

                    }

                    newDrone = new Drone()
                    {
                        Id = d.Id,
                        Model = d.Model,
                        MaxWeight = d.MaxWeight,
                        Battery = d.Battery,
                        DroneStatus = d.DroneStatus,
                        ParcelInDelivery = parcelInDrone,
                        CurrentLocation = d.Location,
                        isActive = d.isActive
                    };
                }
            }
            catch (DO.NoMatchingIdException ex)
            {
                throw new NoMatchingIdException(ex.Message);
            }

            return newDrone;
        }


        /// <summary>
        /// the function receives a drone id and finds a parcel that can be assigned to it
        /// </summary>
        /// <param name="id"> the id of the drone </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DroneToParcel(int id)
        {
            try
            {
                lock (dal)
                { 
                    DroneToList blDrone = GetDroneToList(id);
                    if (!blDrone.isActive)
                        throw new NoMatchingIdException($"drone with ID {id} in not active");
                    if (blDrone.DroneStatus == DroneStatus.Available)
                    {
                        // delete from the list parcels that are too heavy for the drone
                        List<DO.Parcel> parcels = dal.GetParcels().Where(parcel => (int)parcel.Weight >= (int)blDrone.MaxWeight && parcel.Scheduled == null).ToList();

                        // make list of parcels with highest priority possible
                        parcels = parcels.Where(parcel => parcel.Priority == DO.Priorities.Emergency && parcel.Scheduled==null).ToList();

                        if (!parcels.Any())
                            parcels = dal.GetParcels().Where(parcel => parcel.Priority == DO.Priorities.Rapid && parcel.Scheduled == null).ToList();
                        if (!parcels.Any())
                            parcels = dal.GetParcels().Where(parcel => parcel.Priority == DO.Priorities.Regular && parcel.Scheduled == null).ToList();
                        if (!parcels.Any())
                            throw new EmptyListException("no parcel was found\n");

                        // find a parcel in that is possible for the drone to take
                        DO.Parcel posibleDistanceParcel = parcels.First();
                        Parcel finalParcel = new Parcel();
                        bool parcelWasFound = false;
                        foreach (var p in parcels)
                        {
                            DO.Customer tempSender = dal.GetCustomer(p.SenderId);
                            DO.Customer tempTarget = dal.GetCustomer(p.TargetId);
                            DO.Station closestStation = getClosestStation(tempTarget.Latitude, tempTarget.Longitude);
                            double droneToSenderBatterty = Tools.Utils.DistanceCalculation(blDrone.Location.Latitude, blDrone.Location.Longitude, tempSender.Latitude, tempSender.Longitude) * whenAvailable;
                            double senderToTargetBattery = Tools.Utils.DistanceCalculation(tempSender.Latitude, tempSender.Longitude, tempTarget.Latitude, tempTarget.Longitude) * getBatteryConsumption(p.Weight);
                            double targetToStationBattery = Tools.Utils.DistanceCalculation(tempTarget.Latitude, tempTarget.Longitude, closestStation.Latitude, closestStation.Longitude) * whenAvailable;
                            
                            //after calculation the battery consumption check if there is enough in the battery
                            if (droneToSenderBatterty + senderToTargetBattery + targetToStationBattery <= blDrone.Battery)
                            {
                                parcelWasFound = true;
                                finalParcel = GetParcel(p.Id);
                                break;
                            }
                        }

                        // make the necessary updates in the parcel and drone or throw exception if no parcel matched the conditions 
                        if (!parcelWasFound)
                            throw new ImpossibleOprationException("there is no parcel the drone can carry\n");

                        dal.MatchDroneToParcel(dal.GetParcel(finalParcel.Id), dal.GetDrone(id)); // make the update in dal

                        blDrone.ParcelInDeliveryId = finalParcel.Id;
                        blDrone.DroneStatus = DroneStatus.Delivery;
                        UpdateBlDrone(blDrone);
                    }
                    else throw new ImpossibleOprationException("the drone is not available\n");
            }
            }
            catch (DO.NoMatchingIdException ex)
            {
                throw new NoMatchingIdException(ex.Message);
            }
            catch (DO.IdAlreadyExistsException ex)
            {
                throw new IdAlreadyExistsException(ex.Message);
            }
            catch (NoMatchingIdException ex)
            {
                throw new NoMatchingIdException(ex.Message);
            }
            catch (ImpossibleOprationException ex)
            {
                throw new ImpossibleOprationException(ex.Message);
            }
        }


        /// <summary>
        /// pick up parcel by drone - update the drone and the parcel in accordance
        /// </summary>
        /// <param name="drone"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PickUpParcel(Drone drone)
        {
            try
            {
                lock (dal)
                {
                    List<DO.Parcel> parcels = dal.GetParcels().ToList();
                    DO.Parcel oldDalParcel;
                    int parcelIndex = parcels.FindIndex(p => p.DroneId == drone.Id);
                    if (parcelIndex != -1) //this parcel is assigned
                        oldDalParcel = parcels[parcelIndex];
                    else throw new NoMatchingIdException($"no parcel can be picked up by drone with id {drone.Id}");

                    if (oldDalParcel.PickedUp == null) //only drone that assigned to parcel but still didnt pick up can pick up this parcel
                    {
                        dal.PickUpParcel(oldDalParcel);
                        DO.Customer sender = dal.GetCustomer(oldDalParcel.SenderId);
                        double distance = Tools.Utils.DistanceCalculation(drone.CurrentLocation.Latitude, drone.CurrentLocation.Longitude, sender.Latitude, sender.Longitude);
                        double batteryConsumption = getBatteryConsumption(oldDalParcel.Weight);
                        double batteryWaste = distance * batteryConsumption;

                        DroneToList newDrone = new DroneToList
                        {
                            Id = drone.Id,
                            Model = drone.Model,
                            MaxWeight = drone.MaxWeight,
                            Battery = Math.Max (drone.Battery - batteryWaste,0),
                            DroneStatus = GetDroneToList(drone.Id).DroneStatus,
                            Location = new Location { Latitude = sender.Latitude, Longitude = sender.Longitude },
                            ParcelInDeliveryId = oldDalParcel.Id,
                            isActive = drone.isActive
                        };

                        DroneToList oldDrone = GetDroneToList(drone.Id);
                        dronesToList.Remove(oldDrone);
                        dronesToList.Add(newDrone);
                    }
                    else throw new ImpossibleOprationException("Parcel can't be picked up");
                }
            }
            catch (DO.NoMatchingIdException ex)
            {
                throw new NoMatchingIdException(ex.Message);
            }
            catch (DO.IdAlreadyExistsException ex)
            {
                throw new IdAlreadyExistsException(ex.Message);
            }
        }

        /// <summary>
        /// complete the delivery action
        /// </summary>
        /// <param name="drone"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void deliveryPackage(Drone drone)
        {
            try
            {
                lock (dal)
                {
                    DroneToList droneToList = GetDroneToList(drone.Id);
                    if (droneToList.ParcelInDeliveryId != 0) //the drone carries a parcel
                    {
                        DO.Parcel dalParcel = dal.GetParcel(droneToList.ParcelInDeliveryId);
                        if (dalParcel.PickedUp != null && dalParcel.Delivered == null)
                        {
                            DO.Customer target = dal.GetCustomer(dalParcel.TargetId);
                            double distance = Tools.Utils.DistanceCalculation(droneToList.Location.Latitude, droneToList.Location.Longitude, target.Latitude, target.Longitude);
                            double batteryConsumption = getBatteryConsumption(dalParcel.Weight);
                            double battery = distance * batteryConsumption;

                            DroneToList newDrone = new DroneToList
                            {
                                Id = droneToList.Id,
                                Model = droneToList.Model,
                                MaxWeight = droneToList.MaxWeight,
                                Battery = Math.Max( droneToList.Battery - battery,0), 
                                Location = new Location { Latitude = target.Latitude, Longitude = target.Longitude },
                                DroneStatus = DroneStatus.Available,
                                isActive = droneToList.isActive
                            };
                            dronesToList.Remove(droneToList);
                            dronesToList.Add(newDrone);

                            dal.ParcelDelivered(dalParcel);
                        }
                        else throw new ImpossibleOprationException("parcel can't be delivered");
                    }
                    else throw new ImpossibleOprationException("drone is not assigned to any parcel");
                }
            }
            catch (DO.NoMatchingIdException ex)
            {
                throw new NoMatchingIdException(ex.Message);
            }
            catch (DO.IdAlreadyExistsException ex)
            {
                throw new IdAlreadyExistsException(ex.Message);
            }
        }

        /// <summary>
        /// returns a copy of the drones list
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneToList> GetListOfDrones()
        {
            IEnumerable<DroneToList> droneToList1 = dronesToList;
            return droneToList1;
        }

        /// <summary>
        /// the function receives a BL drone and return dal drone
        /// </summary>
        /// <param name="dalDrone"> BO drone to convert to DO </param>
        /// <returns></returns>
        private DO.Drone ConvertDroneToDal(Drone dalDrone)
        {
            DO.Drone newDrone = new DO.Drone()
            {
                Id = dalDrone.Id,
                Model = dalDrone.Model,
                MaxWeight = (DO.WeightCategories)dalDrone.MaxWeight,
                isActive = dalDrone.isActive
            };
            return newDrone;
        }

        /// <summary>
        /// the function receives an id and deletes the drone with that id from the lists in dal and bl
        /// </summary>
        /// <param name="id">id of drone to delete</param>
        public void DeleteDrone(int id)
        {
            try
            {
                lock (dal)
                {
                    dal.DeleteDrone(dal.GetDrone(id));
                    DroneToList deletedDrone = GetDroneToList(id);
                    deletedDrone.isActive = false;
                    UpdateBlDrone(deletedDrone);
                }
            }
            catch (DO.NoMatchingIdException ex)
            {
                throw new NoMatchingIdException(ex.Message);
            }
        }


        /// <summary>
        /// the function calculates all the required battery for the drone according to the distance
        /// first, it calculates battery consumption by the distance between the sender to the target
        /// then, it adds the battery consumption according to the distance between the target to the closest station for charging
        /// and finally, if the parcel wasn't still picked up, it adds the battery consumption according to the distance between the drone to the sender
        /// </summary>
        /// <param name="drone"></param>
        /// <param name="bl"></param>
        /// <param name="parcelId"></param>
        internal double RequiredBattery(DroneToList drone , int parcelId)
        {
            DO.Parcel parcel = dal.GetParcel(parcelId);
            Customer sender = GetCustomer(parcel.SenderId);
            Customer target = GetCustomer(parcel.TargetId);
            double senderLongitude = sender.Location.Longitude;
            double senderLatitude = sender.Location.Latitude;
            double targetLongitude = target.Location.Longitude;
            double targetLatitude = target.Location.Latitude;
            double battery = getBatteryConsumption(parcel.Weight) * Tools.Utils.DistanceCalculation(senderLatitude, senderLongitude, targetLatitude, targetLongitude);
            DO.Station station = getClosestStation(targetLatitude, targetLongitude);
            battery = Math.Min(100,battery + electricity[0] * Tools.Utils.DistanceCalculation(targetLatitude, targetLongitude, station.Latitude, station.Longitude));
            if (parcel.PickedUp is null)
                battery += electricity[0] * Tools.Utils.DistanceCalculation(drone.Location.Latitude, drone.Location.Longitude, senderLatitude, senderLongitude);    
            return battery;
        }

    }
}
