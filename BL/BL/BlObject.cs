using System;
using System.Linq;
using System.Collections.Generic;
using BlApi;
using DalApi;
using BO;
using System.Runtime.CompilerServices;

namespace BL
{
    //Blobject is a singelton - restricts the instantiation of the class to one "single" instance
    sealed partial class BlObject : IBL
    {
        public static IBL instance = new BlObject();
        public static IBL Instance
        {
            get
            {
                return instance;
            }
        }
        internal readonly IDal dal; 
        static BlObject() { }


        public List<DO.Drone> drones = new List<DO.Drone>();
        internal double[] electricity;
        internal double chargeRate, whenAvailable, whenHeavy, whenMedium, whenLight;
        private List<DroneToList> dronesToList;
        internal Random r = new Random();

        private BlObject()
        {
            dal = DalFactory.GetDal();
            dronesToList = new List<DroneToList>();
            drones = dal.GetDrones().ToList();

            //get the electricity rate from DAL:
            electricity = dal.GetElectricity();
            whenAvailable = electricity[0];
            whenLight = electricity[1];
            whenMedium = electricity[2];
            whenHeavy = electricity[3];
            chargeRate = electricity[4];

            initializeDrones();
        }

        /// <summary>
        /// initialize all the fields of all the drones according to their status
        /// </summary>
        void initializeDrones()
        {
            List<DO.Parcel> parcels = dal.GetParcels().ToList();
            DroneToList droneBl;
            foreach (var droneDal in drones)
            {
                droneBl = new DroneToList()
                {
                    Id = droneDal.Id,
                    Model = droneDal.Model,
                    MaxWeight = (WeightCategories)droneDal.MaxWeight,
                    isActive = true
                };

                int parcelIndex = parcels.FindIndex(p => p.DroneId == droneDal.Id);
                if (parcelIndex != -1) //there is a parcel that assigned to this drone
                {
                    DO.Parcel parcel = parcels[parcelIndex];
                    droneBl.ParcelInDeliveryId = parcel.Id;
                    
                    //assigned but wasn't delivered:
                    if (parcel.Delivered == null)
                    {
                        droneBl.DroneStatus = DroneStatus.Delivery;

                        //assigned but wasn't picked up:
                        if (parcel.PickedUp == null)
                        {
                            //the location will be in the closest station to the sender
                            double senderLatitude = dal.GetCustomer(parcel.SenderId).Latitude;
                            double senderLongitude = dal.GetCustomer(parcel.SenderId).Longitude;
                            DO.Station st = getClosestStation(senderLatitude, senderLongitude);
                            droneBl.Location = new Location { Latitude = st.Latitude, Longitude = st.Longitude };
                        }

                        //picked up but wasn't delivered:
                        else
                        {
                            //the location of the drone will be in the sender's location
                            double senderLatitude = dal.GetCustomer(parcel.SenderId).Latitude;
                            double senderLongitude = dal.GetCustomer(parcel.SenderId).Longitude;
                            droneBl.Location = new Location
                            {
                                Latitude = senderLatitude,
                                Longitude = senderLongitude
                            };
                        }
                        //Battery status will be raffled between a minimal charge that will allow the drone to make the shipment and arrive at the station closest to the shipment destination and a full charge
                        Customer customer = GetCustomer(parcel.TargetId);
                        DO.Station closestStation = getClosestStation(customer.Location.Latitude, customer.Location.Longitude);
                        double distance1 = Tools.Utils.DistanceCalculation(droneBl.Location.Latitude, droneBl.Location.Longitude, customer.Location.Latitude, customer.Location.Longitude);
                        double distance2 = Tools.Utils.DistanceCalculation(customer.Location.Latitude, customer.Location.Longitude, closestStation.Latitude, closestStation.Longitude);
                        double battery = getBatteryConsumption(parcel.Weight);

                        double minBattery = (distance1 + distance2) * battery;
                        droneBl.Battery = minBattery + r.NextDouble() * (100 - minBattery);

                    }
                    else // if the parcel already delivered
                    {
                        droneBl.DroneStatus = (DroneStatus)r.Next(2); //Maintenance or Available
                        droneBl.ParcelInDeliveryId = 0; //there is no parcel that assigned to this drone so the parcel id is null(0).                    }

                    }
                }

                else //the drone is not assigned
                {
                    droneBl.DroneStatus = (DroneStatus)r.Next(2); //Maintenance or Available
                    droneBl.ParcelInDeliveryId = 0; //there is no parcel that assigned to this drone so the parcel id is null(0).
                }

                if (droneBl.DroneStatus == DroneStatus.Maintenance)
                {
                    //battery status will be random between 0% - 20%
                    droneBl.Battery = r.Next(0, 21);

                    //the location is a random station from the stations list:
                    int index = r.Next(dal.GetStations().ToList().Count());
                    Location location = new Location()
                    {
                        Latitude = dal.GetStations().ToList()[index].Latitude,
                        Longitude = dal.GetStations().ToList()[index].Longitude
                    };
                    droneBl.Location = location;
                    dal.SendDroneToCharge(dal.GetStations().ToList()[index], droneDal); //send this drone to charge
                }
                else if (droneBl.DroneStatus == DroneStatus.Available)
                {
                    //the location is a random betwwen customers that get a parcel
                    List<DO.Customer> customers = GetCustomersWithParcels().ToList();
                    int rand = r.Next(customers.Count());
                    Location location = new Location { Latitude = customers[rand].Latitude, Longitude = customers[rand].Longitude };
                    droneBl.Location = location;

                    //Battery status will be raffled off between minimal charging that will allow it to reach the station closest to charging and full charging
                    DO.Station closestStation = getClosestStation(droneBl.Location.Latitude, droneBl.Location.Longitude);
                    double distance = Tools.Utils.DistanceCalculation(droneBl.Location.Latitude, droneBl.Location.Longitude, closestStation.Latitude, closestStation.Longitude);
                    double minBattery = distance * whenAvailable;
                    droneBl.Battery = r.Next((int)minBattery, 100);  // minBattery + r.NextDouble() * (100 - minBattery);
                }
                dronesToList.Add(droneBl);
            }
        }

        /// <summary>
        /// get a weight and returns the appropriate battery consumption
        /// </summary>
        internal double getBatteryConsumption(DO.WeightCategories weight)
        {
            switch (weight)
            {
                case DO.WeightCategories.Light:
                    return whenLight;
                case DO.WeightCategories.Medium:
                    return whenMedium;
                case DO.WeightCategories.Heavy:
                    return whenHeavy;
                default:
                    return whenAvailable;
            }
        }
       // public void StartDroneSimulator(int id, Action update, Func<bool> checkStop) => new Simulator(this, id, update, checkStop);
    }
}



