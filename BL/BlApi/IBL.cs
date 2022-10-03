using System;
using System.Collections.Generic;
using BO;

namespace BlApi
{
    public interface IBL
    {
        #region Add functions:

        //All the Add functions get an object and add it to the list 

        void AddStation(Station newStation);
        void AddCustomer(Customer customer);
        void AddParcel(Parcel parcel);
        void AddDrone(Drone newDrone, int stationId);
        #endregion

        #region Update functions:
        /// <summary>
        /// Get the customer for update and change his fields according the params
        /// </summary>
        /// <param name="newCustomer"></param>
        void UpdateCustomer(Customer newCustomer);
        
        /// <summary>
        /// Get the drone for update and change his fields according to the params
        /// </summary>
        /// <param name="d"></param>
        void UpdateDrone(Drone d);
        
        /// <summary>
        /// Get the fields for updating and change them according to the params
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="chargeSlots"></param>
        void UpdateStation(int id, string name, int chargeSlots);

        /// <summary>
        /// Releases the drone from charge
        /// </summary>
        /// <param name="droneId"></param>
        void FreeDrone(int droneId); 
        
        /// <summary>
        /// Allocate the drone to the most urgent parcel
        /// </summary>
        /// <param name="id"></param>
        void DroneToParcel(int id); 
        
        /// <summary>
        /// Send the drone to charge in the closest station
        /// </summary>
        /// <param name="id"></param>
        void RechargeDrone(int id); 
        
        /// <summary>
        /// The drone pick up the parcel for delivery
        /// </summary>
        /// <param name="drone"></param>
        void PickUpParcel(Drone drone); 
       
        /// <summary>
        /// complete the delivery action
        /// </summary>
        void deliveryPackage(Drone drone);
        #endregion

        #region Get functions:

        //All the Get functions get an ID and return the object with this id

        DroneToList GetDroneToList(int id);
        StationToList GetStationToList(int id);
        Drone GetDrone(int id);
        Station GetStation(int id);
        Customer GetCustomer(int id);
        Parcel GetParcel(int id);
        
        //All the GetList functions return all the list of the entity

        IEnumerable<StationToList> GetListOfStations();
        IEnumerable<ParcelToList> GetListofParcels();
        IEnumerable<DroneToList> GetListOfDrones();
        IEnumerable<ParcelToList> GetListofParcelsWithoutDrone();
        IEnumerable<StationToList> GetListOfStationsWithAvailableChargeSlots();
        IEnumerable<CustomerToList> GetListOfCustomers();
        #endregion

        #region Delete functions:

        //All the Delete functions get an ID and delete the object with this id

        void DeleteStation(int id);
        void DeleteDrone(int id);
        void DeleteCustomer(int id);
        void DeleteParcel(int id);
        #endregion

        // void StartDroneSimulator(int id, Action update, Func<bool> checkStop);

    }
}