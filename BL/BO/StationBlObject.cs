using BlApi;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BL
{
    public partial class BlObject : IBL
    {
        /// <summary>
        /// get station of BL and add it into the stations list of DAL
        /// </summary>
        /// <param name="newStation"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddStation(BO.Station newStation)
        {
            try
            {
                lock (dal)
                {
                    DO.Station dalStation = new DO.Station
                    {
                        Id = newStation.Id,
                        Name = newStation.Name,
                        AvailableChargeSlots = newStation.AvailableChargeSlots,
                        Latitude = newStation.Location.Latitude,
                        Longitude = newStation.Location.Longitude,
                        isActive = true
                    };
                    dal.AddStation(dalStation);
                }
            }
            catch (DO.IdAlreadyExistsException ex)
            {
                throw new BO.IdAlreadyExistsException(ex.Message);
            }
        }

        /// <summary>
        /// the function receives id of station and two possible changes - name and number of charge slots
        /// the function checks if either/both of the changes was made and updates the station in the list accordinly
        /// </summary>
        /// <param name="id">id of station</param>
        /// <param name="name">possible change of name</param>
        /// <param name="chargeSlots">possible change of charge slots number</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateStation(int id, string name, int chargeSlots)
        {
            try
            {
                lock (dal)
                {
                    DO.Station dalStation = dal.GetStation(id);
                    if (!dalStation.isActive)
                        throw new BO.NoMatchingIdException($"station with ID {dalStation.Id} does not exist !!");
                    if (name == "" && chargeSlots.ToString() == "")
                        throw new BO.NoUpdateException("no update to station was received\n");
                    else
                    {
                        bool nameIsUpdated = false;
                        if (name != "")
                        {
                            dalStation.Name = name;
                            nameIsUpdated = true;
                            dal.UpdateStation(dalStation);
                        }

                        int unavailableChargeSlots = dal.GetDroneCharges(d => d.StationId == id).Count();
                        bool slotsIsUpdated = false;

                        if (chargeSlots.ToString() != "")
                        {
                            if (chargeSlots >= unavailableChargeSlots)
                            {
                                dalStation.AvailableChargeSlots = chargeSlots - unavailableChargeSlots;
                                slotsIsUpdated = true;
                                dal.UpdateStation(dalStation);
                            }
                            else throw new BO.ImpossibleOprationException("can't update the station with number smaller than the number of drones charging");
                        }

                        if (nameIsUpdated == false && slotsIsUpdated == false)
                            throw new BO.NoUpdateException("no update to station was received\n");
                    }
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
            catch (BO.NoUpdateException ex)
            {
                throw new BO.NoUpdateException(ex.Message);
            }
            catch (BO.ImpossibleOprationException ex)
            {
                throw new BO.ImpossibleOprationException(ex.Message);
            }
            catch (BO.NoMatchingIdException ex)
            {
                throw new BO.NoMatchingIdException(ex.Message);
            }
        }

        /// <summary>
        /// the function return the station by an id
        /// </summary>
        /// <param name="id">id of station to find</param>
        /// <returns>station</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public BO.Station GetStation(int id)
        {
            try
            {
                lock (dal)
                {
                    DO.Station dalStation = dal.GetStation(id);                
                    List<BO.DroneInCharging> dronesCharging = new List<BO.DroneInCharging>();
                    foreach (var droneCharge in dal.GetDroneCharges())
                    {
                        if (droneCharge.StationId == id)
                            dronesCharging.Add(new BO.DroneInCharging { Id = droneCharge.DroneId, Battery = GetDroneToList(droneCharge.DroneId).Battery });
                    }
                    BO.Station station = new BO.Station
                    {
                        Id = dalStation.Id,
                        Name = dalStation.Name,
                        Location = new BO.Location { Latitude = dalStation.Latitude, Longitude = dalStation.Longitude },
                        AvailableChargeSlots = dalStation.AvailableChargeSlots,
                        DronesCharging = dronesCharging,
                        isActive = true
                    };
                    return station;
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
            catch (BO.NoMatchingIdException ex)
            {
                throw new BO.NoMatchingIdException(ex.Message);
            }
        }


        /// <summary>
        /// the function receives an id and return the BO stationToLisist with that id
        /// </summary>
        /// <param name="id">id ofstation to list to look for</param>
        /// <returns>the stationToList with the id that was received</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public BO.StationToList GetStationToList(int id)
        {
            lock (dal)
            {
                return convertStationToStationToList(dal.GetStation(id));
            }
        }

        /// <summary>
        /// the function receives a DO station and converts it to BO stationToList
        /// </summary>
        /// <param name="dalStation">the DO station</param>
        /// <returns>the converted station</returns>
        private BO.StationToList convertStationToStationToList(DO.Station dalStation)
        {
            lock (dal)
            {
                // find how many drones are charging in the station using dal droneCharge type
                int OccupiedChargeSlots = 0;
                foreach (var droneCharge in dal.GetDroneCharges())
                {
                    if (droneCharge.StationId == dalStation.Id)
                        // once you find another droneCharge with the station ID add one to the number of occupied slots
                        OccupiedChargeSlots++;
                }
                BO.StationToList blStation = new BO.StationToList
                {
                    Id = dalStation.Id,
                    Name = dalStation.Name,
                    AvailableChargeSlots = dalStation.AvailableChargeSlots,
                    OccupiedChargeSlots = OccupiedChargeSlots,
                    isActive = dalStation.isActive
                };
                return blStation;
            }
        }

        /// <summary>
        /// the function returns a list of station that have availbale charge slots
        /// </summary>
        /// <returns>ienumerable of the list that was created</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<BO.StationToList> GetListOfStationsWithAvailableChargeSlots()
        {
            lock (dal)
            {
                List<BO.StationToList> stationsWithAvailableChargeSlots = new List<BO.StationToList>();
                foreach (var station in dal.GetStations(s => s.AvailableChargeSlots > 0 && s.isActive)) //send a predicate to DAL
                {
                    stationsWithAvailableChargeSlots.Add(convertStationToStationToList(station));
                }
                return stationsWithAvailableChargeSlots;
            }
        }

        /// <summary>
        /// the function converts the whole DO stations list in dal to a list of BO stationToLists
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<BO.StationToList> GetListOfStations()
        {
            lock (dal)
            {
                List<BO.StationToList> stations = new List<BO.StationToList>();
                foreach (var s in dal.GetStations())
                {
                    stations.Add(convertStationToStationToList(s));
                }
                return stations;
            }
        }

        /// <summary>
        /// help function that returns the closest station to a location
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        internal DO.Station getClosestStation(double latitude, double longitude)
        {
            lock (dal)
            {
                DO.Station result = default;
                double distance = double.MaxValue;

                foreach (var item in dal.GetStations())
                {
                    double dist = Tools.Utils.DistanceCalculation(latitude, longitude, item.Latitude, item.Longitude);
                    if (dist < distance && item.isActive)
                    {
                        distance = dist;
                        result = item;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// the function receives an id number and marks the station with that id as not active
        /// </summary>
        /// <param name="id">id of statoin to delete</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteStation(int id)
        {
            lock (dal)
            {
                try
                {
                    dal.DeleteStation(dal.GetStation(id));
                }
                catch (DO.NoMatchingIdException ex)
                {
                    throw new BO.NoMatchingIdException(ex.Message);
                }
            }
        }

    }
}

