using System.Runtime.CompilerServices;
using DO;
using System;
using System.Collections.Generic;

namespace DalApi
{
    public interface IDal
    {
        #region Create part of C.R.U.D

        //All the Add functions get an object and add it to the list 

        void AddCustomer(Customer c);
        void AddDrone(Drone d);
        int AddParcel(Parcel p);
        void AddStation(Station s);
        #endregion

        #region Request part of C.R.U.D

        //All the Get functions get an ID and return the object with this id

        Customer GetCustomer(int idNumber);
        Drone GetDrone(int idNumber);
        Station GetStation(int idNumber);
        Parcel GetParcel(int idNumber);
        DroneCharge GetDroneCharge(int idNumber);

        //All the GetList functions return all the list of the entity

        IEnumerable<Drone> GetDrones(Func<Drone, bool> predicate = null);
        IEnumerable<Customer> GetCustomers(Func<Customer, bool> predicate = null);
        IEnumerable<Station> GetStations(Func<Station, bool> predicate = null);
        IEnumerable<Parcel> GetParcels(Func<Parcel, bool> predicate = null);
        IEnumerable<DroneCharge> GetDroneCharges(Func<DroneCharge, bool> predicate = null);
        #endregion

        #region Update part of C.R.U.D

        /// <summary>
        /// Get a parcel and drone and assign the parcel to the drone - update the scheduled time
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        void MatchDroneToParcel(Parcel p, Drone d);
       
        /// <summary>
        /// get a parcel and deliver it to the target - update the delivere time
        /// </summary>
        /// <param name="p"></param>
        void ParcelDelivered(Parcel p);
        
        /// <summary>
        /// get parcel and pick it up by drone - update the pick-up time 
        /// </summary>
        /// <param name="p"></param>
        void PickUpParcel(Parcel p);
        
        /// <summary>
        /// get station and drone and release the drone from the charge station 
        /// by updating the num of available charge slots +1
        /// </summary>
        /// <param name="s"></param>
        /// <param name="d"></param>
        void ReleaseDroneFromCharge(Station s, Drone d);

        /// <summary>
        ///  get station and drone and insert the drone into the charge station 
        ///  by updating the available charge slots -1
        /// </summary>
        /// <param name="s"></param>
        /// <param name="d"></param>
        void SendDroneToCharge( Station s, Drone d);

        //All the update functions get the object for update and change his fields according to the params
        
        void UpdateDrone(Drone d);
        void UpdateCustomer(Customer c);
        void UpdateStation(Station s);
        #endregion
      

        #region Delete part of C.R.U.d

        //The delete functions get an object for delete and update it to be not active
       
        void DeleteParcel(Parcel p);
        void DeleteCustomer(Customer c);
        void DeleteDrone(Drone d);
        void DeleteStation(Station s);
        #endregion

        double[] GetElectricity();
    }
}