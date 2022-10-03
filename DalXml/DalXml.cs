using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DalApi;
using DO;

namespace Dal
{
    sealed class DalXml : IDal
    {
        private static string DataDirectory = @"data";

        private static string configPath = Path.Combine(DataDirectory, @"ConfigXML.xml");
        private static string baseStationsPath = Path.Combine(DataDirectory, @"BaseStations.xml");
        private static string dronesPath = Path.Combine(DataDirectory, @"Drones.xml");
        private static string parcelsPath = Path.Combine(DataDirectory, @"Parcels.xml");
        private static string customersPath = Path.Combine(DataDirectory, @"Customers.xml");
        private static string droneChargesPath = Path.Combine(DataDirectory, @"DroneCharges.xml");

        public static readonly DalXml instance = new DalXml();
        public static DalXml Instance { get => instance; }
        static DalXml() { } // static ctor to ensure instance init is done just before first usage


        //private static string configPath = Path.Combine(solutionDirectory, "DalXML", "data");
        [MethodImpl(MethodImplOptions.Synchronized)]
        private DalXml()
        {
            DataSource.Initialize();

        //    List<DroneCharge> droneCharge = XMLTools.LoadListFromXmlSerializer<DroneCharge>(droneChargesPath);
        //    foreach (var item in droneCharge)
        //    {
        //        UpdatePluseChargeSlots(item.StationId);
        //    }
        //    droneCharge.Clear();
        //    XMLTools.SaveListToXmlSerializer(droneCharge, droneChargesPath);
        }

        //private void UpdatePluseChargeSlots(int stationId)
        //{
        //    Station s = GetStation(stationId);
        //    s.AvailableChargeSlots++;
        //    UpdateStation(s);
        //}

        [MethodImpl(MethodImplOptions.Synchronized)]
        public double[] GetElectricity()
        {
            //var temp1 = XMLTools.LoadListFromXmlElement(configPath);
            //var temp2 = temp1.Element("electricityRates").Elements();
            //var temp3 = temp2.Select(e => Convert.ToDouble(e.Value)).ToArray();
            //return temp3;

            return XMLTools.LoadListFromXmlElement(configPath).Element("electricityRates").Elements()
            .Select(e => Convert.ToDouble(e.Value)).ToArray();
        }

        #region Get Entity

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Station GetStation(int requestedId)
        {
            XElement baseStationXml = XMLTools
                .LoadListFromXmlElement(baseStationsPath)
                .Elements().FirstOrDefault(bs => bs.Element("Id").Value == $"{requestedId}");
            return XMLTools.LoadListFromXmlSerializer<Station>(baseStationsPath).Find(bs => bs.Id == requestedId);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone GetDrone(int requestedId)
        {
            return XMLTools.LoadListFromXmlSerializer<Drone>(dronesPath).Find(drone => drone.Id == requestedId);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Customer GetCustomer(int requestedId)
        {
            return XMLTools.LoadListFromXmlSerializer<Customer>(customersPath).Find(customer => customer.Id == requestedId);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Parcel GetParcel(int requestedId)
        {
            Parcel p = XMLTools.LoadListFromXmlSerializer<Parcel>(parcelsPath).Find(parcel => parcel.Id == requestedId);
            if (p.Id == 0)
                throw new NoMatchingIdException($"parcel with ID {requestedId} was not found");
            return p;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public DroneCharge GetDroneCharge(int droneId)
        {
            XElement droneChargeXml = XMLTools
                .LoadListFromXmlElement(droneChargesPath)
                .Elements().FirstOrDefault(dc => dc.Element("DroneId").Value == $"{droneId}");
            return XMLTools.LoadListFromXmlSerializer<DroneCharge>(droneChargesPath).Find(dc => dc.DroneId == droneId);
        }
        #endregion

        #region Get collections

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Station> GetBaseStations()
        {
            XElement baseStationsXML = XMLTools.LoadListFromXmlElement(baseStationsPath);

            List<Station> baseStations = new();
            foreach (var baseStationElement in baseStationsXML.Elements())
            {
                baseStations.Add(
                    new Station()
                    {
                        Id = int.Parse(baseStationElement.Element("Id").Value),
                        AvailableChargeSlots = int.Parse(baseStationElement.Element("AvailableChargeSlots").Value),
                        Name = baseStationElement.Element("Name").Value,
                        Latitude = double.Parse(baseStationElement.Element("Latitude").Value),
                        Longitude = double.Parse(baseStationElement.Element("Longitude").Value),
                    });
            }

            return baseStations;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Parcel> GetParcels(Func<Parcel, bool> predicate = null) =>
            predicate == null ?
               XMLTools.LoadListFromXmlSerializer<Parcel>(parcelsPath) :
            XMLTools.LoadListFromXmlSerializer<Parcel>(parcelsPath).Where(predicate);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Station> GetStations(Func<Station, bool> predicate = null) =>
            predicate == null ?
                XMLTools.LoadListFromXmlSerializer<Station>(baseStationsPath) :
                XMLTools.LoadListFromXmlSerializer<Station>(baseStationsPath).Where(predicate);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Drone> GetDrones(Func<Drone, bool> predicate = null) =>
            predicate == null ?
                XMLTools.LoadListFromXmlSerializer<Drone>(dronesPath) :
                XMLTools.LoadListFromXmlSerializer<Drone>(dronesPath).Where(predicate);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Customer> GetCustomers(Func<Customer, bool> predicate = null) =>
            predicate == null ?
                XMLTools.LoadListFromXmlSerializer<Customer>(customersPath) :
                XMLTools.LoadListFromXmlSerializer<Customer>(customersPath).Where(predicate);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneCharge> GetDroneCharges(Func<DroneCharge, bool> predicate = null) =>
            predicate == null ?
                XMLTools.LoadListFromXmlSerializer<DroneCharge>(droneChargesPath) :
                XMLTools.LoadListFromXmlSerializer<DroneCharge>(droneChargesPath).Where(predicate);
        #endregion

        #region Add

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddStation(Station addBaseStation)
        {            
            List<Station> stations = XMLTools.LoadListFromXmlSerializer<Station>(baseStationsPath);
            if (stations.FindIndex(x => x.Id == addBaseStation.Id) != -1) //this station exists
                throw new IdAlreadyExistsException("This station already exists");
            stations.Add(addBaseStation);
            XMLTools.SaveListToXmlSerializer(stations, baseStationsPath);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddDrone(Drone droneDO)
        {
            XElement drones = XMLTools.LoadListFromXmlElement(dronesPath);
            drones.Add(
                new XElement("Drone",
                    new XElement("Id", droneDO.Id),
                    new XElement("MaxWeight", droneDO.MaxWeight),
                    new XElement("Model", droneDO.Model),
                    new XElement("isActive", droneDO.isActive)
                )
            );
            XMLTools.SaveListToXmlElement(drones, dronesPath);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddCustomer(Customer customerDo)
        {
            List<Customer> customers = XMLTools.LoadListFromXmlSerializer<Customer>(customersPath);
            if (customers.FindIndex(x => x.Id == customerDo.Id) != -1) //this customer exists
                throw new IdAlreadyExistsException("This customer already exists");
            customers.Add(customerDo);
            XMLTools.SaveListToXmlSerializer(customers, customersPath);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int AddParcel(Parcel parcel)
        {
            List<Parcel> parcels = XMLTools.LoadListFromXmlSerializer<Parcel>(parcelsPath);
            if (parcels.FindIndex(x => x.Id == parcel.Id) != -1) //this parcel exists
                throw new IdAlreadyExistsException("This parcel already exists");
            parcel.Id = DataSource.Config.ParcelId;
            parcels.Add(parcel);
            XMLTools.SaveListToXmlSerializer(parcels, parcelsPath);
            return ++DataSource.Config.ParcelId;
        }

        #endregion

        #region Update Functions

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDrone(Drone d)
        {
           
            List<Drone> drones = XMLTools.LoadListFromXmlSerializer<Drone>(dronesPath);
            int index = drones.FindIndex(dr => dr.Id == dr.Id);
            if (index != -1)
            {
                drones.RemoveAt(index);
                drones.Insert(index,d);
            }
            else
                throw new NoMatchingIdException($"drone with id {d.Id} doesn't exist");
            XMLTools.SaveListToXmlSerializer(drones, dronesPath);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateCustomer(Customer c)
        {
            List<Customer> customers = XMLTools.LoadListFromXmlSerializer<Customer>(customersPath);
            int clientIndex = customers.FindIndex(p => p.Id == c.Id);
            if (clientIndex != -1)
            {
                customers.RemoveAt(clientIndex);
                customers.Insert(clientIndex,c);
            }
            else
                throw new NoMatchingIdException($"customer {c.Id} doesn't exist");
            XMLTools.SaveListToXmlSerializer(customers, customersPath);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateStation(Station s)
        {
            List<Station> stations = XMLTools.LoadListFromXmlSerializer<Station>(baseStationsPath);
            int st = stations.FindIndex(p => p.Id == s.Id);
            if (st != -1)
            {
                stations.RemoveAt(st);
                stations.Insert(st, s);
            }
            else
                throw new NoMatchingIdException($"station {s.Id} doesn't exist");
            XMLTools.SaveListToXmlSerializer(stations, baseStationsPath);
          
        }
        #endregion

        #region  Actions on Parcel 

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void MatchDroneToParcel(Parcel p, Drone d)
        {
          
            List<Parcel> parcels = XMLTools.LoadListFromXmlSerializer<Parcel>(parcelsPath);
            List<Drone> drones = XMLTools.LoadListFromXmlSerializer<Drone>(dronesPath);
            Parcel parcel = parcels.Find(prcl => prcl.Id == p.Id);
            if (!parcels.Exists(prcl => prcl.Id == p.Id))
                throw new NoMatchingIdException($"parcel with id {p.Id} doesn't exist");
            Drone drone = drones.Find(dr => dr.Id == d.Id);
            if (!drones.Exists(dr=>dr.Id==d.Id))
                throw new NoMatchingIdException($"Drone with id {d.Id} doesn't exist");

            parcel.DroneId = d.Id;
            parcel.Scheduled = DateTime.Now;

            int index = parcels.FindIndex(prcl => prcl.Id == p.Id);
            parcels.RemoveAt(index);
            parcels.Insert(index, parcel);
            XMLTools.SaveListToXmlSerializer<Parcel>(parcels, parcelsPath);

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PickUpParcel(Parcel p)
        {
            List<Parcel> parcels = XMLTools.LoadListFromXmlSerializer<Parcel>(parcelsPath);
            Parcel parcel = parcels.Find(prcl => prcl.Id == p.Id);
            if (!parcels.Exists(prcl => prcl.Id == p.Id))
                throw new NoMatchingIdException($"parcel with id {p.Id} doesn't exist");          
            parcel.PickedUp = DateTime.Now;

            int index = parcels.FindIndex(prcl => prcl.Id == p.Id);
            parcels.RemoveAt(index);
            parcels.Insert(index, parcel);
            XMLTools.SaveListToXmlSerializer<Parcel>(parcels, parcelsPath);
         
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ParcelDelivered(Parcel p)
        {
            List<Parcel> parcels = XMLTools.LoadListFromXmlSerializer<Parcel>(parcelsPath);
            Parcel parcel = parcels.Find(prcl => prcl.Id == p.Id);
            if (!parcels.Exists(prcl => prcl.Id == p.Id))
                throw new NoMatchingIdException($"parcel with id {p.Id} doesn't exist");
            parcel.Delivered = DateTime.Now;
            int index = parcels.FindIndex(prcl => prcl.Id == p.Id);
            parcels.RemoveAt(index);
            parcels.Insert(index, parcel);
            XMLTools.SaveListToXmlSerializer<Parcel>(parcels, parcelsPath);

        }
        #endregion

        #region  charging drone and release
        /// <summary>
        /// get the drone for charge and the station and update the available charge slots and the list of charge slots
        /// </summary>
        /// <param name="Station s"></param>
        /// <param name="Drone d"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SendDroneToCharge(Station s, Drone d)
        {
            s.AvailableChargeSlots--;
            DroneCharge dc = new DroneCharge()
            {
                DroneId = d.Id,
                StationId = s.Id,
                ChargingTime = DateTime.Now

            };
            List<DroneCharge> droneCharges = XMLTools.LoadListFromXmlSerializer<DroneCharge>(droneChargesPath);
            droneCharges.Add(dc);
            XMLTools.SaveListToXmlSerializer<DroneCharge>(droneCharges, droneChargesPath);




        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ReleaseDroneFromCharge(Station s, Drone d)
        {
            XElement baseStations = XMLTools.LoadListFromXmlElement(baseStationsPath);
            XElement baseStation = (from bs in baseStations.Elements()
                                    where bs.Element("Id").Value == $"{s.Id}"
                                    select bs).FirstOrDefault();
            int availableChargingPorts = Convert.ToInt32(baseStation.Element("AvailableChargeSlots").Value);
            ++availableChargingPorts;
            baseStation.Element("AvailableChargeSlots").Value = availableChargingPorts.ToString();

            XMLTools.SaveListToXmlElement(baseStations, baseStationsPath);

            XElement droneCharges = XMLTools.LoadListFromXmlElement(droneChargesPath);
            XElement removeElement = (from dc in droneCharges.Elements()
                                      where dc.Element("DroneId").Value == $"{d.Id}"
                                      select dc).FirstOrDefault();
            removeElement.Remove();
            XMLTools.SaveListToXmlElement(droneCharges, droneChargesPath);
        }
        #endregion

        #region Delete

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteParcel(Parcel p)
        {
            List<Parcel> parcels = XMLTools.LoadListFromXmlSerializer<Parcel>(parcelsPath);
            int parcelIndex = parcels.FindIndex(pr => p.Id == pr.Id);
            if (parcelIndex != -1)
            {
                parcels.RemoveAt(parcelIndex);
            }
            else
                throw new NoMatchingIdException($"parcel {p.Id} doesn't exist");
            XMLTools.SaveListToXmlSerializer(parcels, parcelsPath);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteCustomer(Customer c)
        {
            List<Customer> customers = XMLTools.LoadListFromXmlSerializer<Customer>(customersPath);
            int clientIndex = customers.FindIndex(p => p.Id == c.Id);
            if (clientIndex != -1)
            {
                c.isActive = false;
                customers.RemoveAt(clientIndex);
                customers.Insert(clientIndex, c);
            }
            else
                throw new NoMatchingIdException($"customer {c.Id} doesn't exist");
            XMLTools.SaveListToXmlSerializer(customers, customersPath);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteDrone(Drone d)
        {
            XElement drones = XMLTools.LoadListFromXmlElement(dronesPath);
            XElement removeElement = (from dr in drones.Elements()
                                      where dr.Element("Id").Value == $"{d.Id}" && dr.Element("isActive").Value == "true"
                                      select dr).FirstOrDefault();
            removeElement.Remove();
            drones.Add(
           new XElement("Drone",
               new XElement("Id", d.Id),
               new XElement("MaxWeight", d.MaxWeight),
               new XElement("Model", d.Model),
               new XElement("isActive", false)
           )
       );
            XMLTools.SaveListToXmlElement(drones, dronesPath);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteStation(Station s)
        {
            List<Station> stations = XMLTools.LoadListFromXmlSerializer<Station>(baseStationsPath);
            int idx = stations.FindIndex(p => p.Id == s.Id && p.isActive);
            if (idx != -1)
            {
                s.isActive = false;
                stations.RemoveAt(idx);
                stations.Insert(idx, s);
            }
            else
                throw new NoMatchingIdException($"station {s.Id} doesn't exist");
            XMLTools.SaveListToXmlSerializer(stations, baseStationsPath);

        }
        #endregion
    }
}