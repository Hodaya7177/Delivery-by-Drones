//using System;
//using BO;
//using System.Threading;
//using static BL.BlObject;
//using System.Linq;
//using static System.Math;

//namespace BL
//{
//    class Simulator
//    {
//        enum Maintenance { step1, step2, step3 }

//        internal const int DELAY = 500;
//        internal const double DRONE_SPEED = 1.0;
//        private const double TIME_STEP = DELAY / 1000.0;
//        private const double STEP = DRONE_SPEED / TIME_STEP;
//        public Simulator(BlObject bl, int droneId, Action update, Func<bool> checkStop)
//        {
//            var AccessIbl = bl;
//            var dal = bl.dal;
//            DroneToList drone = bl.GetDroneToList(droneId);
//            DO.Station station = new DO.Station(); //currently null
//            double electricity = 0.0;
//            double distance = 0.0;
//            bool pickedUp = false;
//            Customer customer = null;
//            Maintenance maintenance = Maintenance.step1;
//            DO.Parcel parcel = new DO.Parcel();
//            int parcelId = 0;

//            void delivery(int id)
//            {
//                parcel = dal.GetParcel(id);
//                electricity = AccessIbl.getBatteryConsumption(parcel.Weight);
//                pickedUp = parcel.PickedUp == null ? false : true;
//                customer = bl.GetCustomer(parcel.PickedUp == null ? parcel.TargetId : parcel.SenderId);
//            }



//            while (!checkStop())
//            {
//                switch (drone.DroneStatus)
//                {
//                    case DroneStatus.Available:
//                        if (!sleepSimulator()) break;

//                        lock (bl) lock (dal)
//                            {
//                                parcelId = bl.dal.GetParcels(p => p.Scheduled == null
//                                                                 && (WeightCategories)(p.Weight) <= drone.MaxWeight
//                                                                 && AccessIbl.RequiredBattery(drone, (int)p.Id) < drone.Battery)
//                                                .OrderByDescending(p => p.Priority)
//                                                .ThenByDescending(p => p.Weight)
//                                                .FirstOrDefault().Id;
//                                switch (drone.ParcelInDeliveryId, drone.Battery)
//                                {
//                                    case (0, 100):
//                                        break;
//                                    case (0, _):
//                                        {
//                                            station = AccessIbl.getClosestStation(drone.Location.Latitude, drone.Location.Longitude); //fint the closest ataion for charging the drone
//                                            if (station.Id != 0) //there is close station to charge the drone
//                                            {
//                                                drone.DroneStatus = DroneStatus.Maintenance;
//                                                maintenance = Maintenance.step1;
//                                                DO.Drone dalDrone = dal.GetDrone(drone.Id);
//                                                dal.SendDroneToCharge(station, dalDrone);
//                                            }
//                                            break;
//                                        }
//                                    case (_, _):
//                                        {
//                                            dal.MatchDroneToParcel(dal.GetParcel(parcelId), dal.GetDrone(drone.Id));
//                                            drone.ParcelInDeliveryId = parcelId;
//                                            delivery(parcelId);
//                                            drone.DroneStatus = DroneStatus.Delivery;
//                                            break;
//                                        }
//                                }
//                                break;
//                            }

//                    case DroneStatus.Maintenance:
//                        lock (bl) lock (dal)
//                            {
//                                switch (maintenance)
//                                {
//                                    //get closest station and calculate the distance and update the drone and the station in PL
//                                    case Maintenance.step1:
//                                        station = AccessIbl.getClosestStation(drone.Location.Latitude, drone.Location.Longitude); //find the closest station for charging the drone
//                                        distance = Tools.Utils.DistanceCalculation(drone.Location.Latitude, drone.Location.Longitude, station.Latitude, station.Longitude);
//                                        maintenance = Maintenance.step2;
//                                        break;

//                                    case Maintenance.step2:
//                                        if (distance < 0.01)
//                                            lock (bl)
//                                            {
//                                                drone.Location = new Location { Latitude = station.Latitude, Longitude = station.Longitude };
//                                                maintenance = Maintenance.step3;
//                                            }
//                                        else
//                                        {
//                                            /////להשלים
//                                        }
//                                        break;
//                                    case Maintenance.step3:
//                                        lock (bl) lock (dal)
//                                            {
//                                                if (drone.Battery == 100)
//                                                {
//                                                    dal.ReleaseDroneFromCharge(station, dal.GetDrone(drone.Id));
//                                                    drone.DroneStatus = DroneStatus.Available;
//                                                }
//                                                else
//                                                {
//                                                    if (!sleepSimulator()) break;
//                                                    lock (bl) drone.Battery = Min(1.0, drone.Battery + bl.chargeRate * TIME_STEP);
//                                                }
//                                            }
//                                        break;
//                                }
//                                break;

//                            }


//                    case DroneStatus.Delivery:

//                        lock (bl) lock (dal)
//                            {
//                                try { if (drone.ParcelInDeliveryId != 0) delivery(drone.ParcelInDeliveryId); }
//                                catch (DO.NoMatchingIdException ex) { throw new NoMatchingIdException(ex.Message); }
//                                distance = Tools.Utils.DistanceCalculation(drone.Location.Latitude, drone.Location.Longitude, customer.Location.Latitude, customer.Location.Longitude);
//                            }

//                        if (distance < 0.01 || drone.Battery == 0.0)
//                        {
//                            lock (bl) lock (dal)
//                                {
//                                    drone.Location = customer.Location;
//                                    if (pickedUp)
//                                    {
//                                        dal.ParcelDelivered(parcel);
//                                        drone.DroneStatus = DroneStatus.Available;

//                                    }
//                                    else
//                                    {
//                                        dal.PickUpParcel(parcel);
//                                        customer = bl.GetCustomer(parcel.TargetId);
//                                        pickedUp = true;
//                                    }
//                                }

//                        }
//                        else
//                        {
//                            if (!sleepSimulator()) break;
//                            lock (bl)
//                            {
//                                double delta = distance < STEP ? distance : STEP;
//                                double proportion = delta / distance;
//                                drone.Battery = Max(0.0, drone.Battery - delta * (pickedUp ? bl.getBatteryConsumption(parcel.Weight) : bl.whenAvailable));
//                                double lat = drone.Location.Latitude + (customer.Location.Latitude - drone.Location.Latitude) * proportion;
//                                double lon = drone.Location.Longitude + (customer.Location.Longitude - drone.Location.Longitude) * proportion;
//                                drone.Location = new() { Latitude = lat, Longitude = lon };
//                            }
//                        }

//                        break;

//                    default:
//                        break;
//                }

//                update();
//            }

//        }

//        private static bool sleepSimulator()
//        {
//            try { Thread.Sleep(DELAY); } catch (ThreadInterruptedException) { return false; }
//            return true;
//        }
//    }
//}


