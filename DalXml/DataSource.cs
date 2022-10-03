using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DO;

namespace Dal
{
    internal class DataSource
    {
        internal static List<Drone> Drones = new List<Drone>();
        internal static List<Station> Stations = new List<Station>();
        internal static List<Customer> Customers = new List<Customer>();
        internal static List<Parcel> Parcels = new List<Parcel>();
        internal static List<DroneCharge> DroneCharges = new List<DroneCharge>();

        private const string DataDirectory = @"data";

        private static string configPath = Path.Combine(DataDirectory, @"ConfigXML.xml");
        private static string baseStationsPath = Path.Combine(DataDirectory, @"BaseStations.xml");
        private static string dronesPath = Path.Combine(DataDirectory, @"Drones.xml");
        private static string parcelsPath = Path.Combine(DataDirectory, @"Parcels.xml");
        private static string customersPath = Path.Combine(DataDirectory, @"Customers.xml");
        private static string droneChargesPath = Path.Combine(DataDirectory, @"DroneCharges.xml");
        internal static Random r = new Random();

        internal class Config
        {
            internal static int ParcelId = 1000000;
            internal static double WhenAvailable { get { return 0.1; } }
            internal static double WhenLightWeight { get { return 1; } }
            internal static double WhenMediumWeight { get { return 2; } }
            internal static double WhenHeavyWeight { get { return 4; } }
            internal static double ChargingRate { get { return 5; } }
        }
        /// <summary>
        /// initialize the data lists 
        /// </summary>
        public static void Initialize()
        {
            //initialize 2 stations:
            createStation();
            //initialize 10 customers:
            createCustomer(10);
            //initialize 10 drones:
            createDrone(10);
            //initialize 10 parcels:
            createParcel();
            SaveListToXmlSerializer(Drones, dronesPath);
            SaveListToXmlSerializer(Stations, baseStationsPath);
            SaveListToXmlSerializer(Customers, customersPath);
            SaveListToXmlSerializer(Parcels, parcelsPath);
        }

        private static void createStation()
        {
            Station s = new Station
            {
                Id = r.Next(10000, 100000),
                Name = "Romema",
                Longitude = 35.20553,
                Latitude = 31.79160,
                AvailableChargeSlots = r.Next(5, 11),
                isActive = true
            };
            Stations.Add(s);

            s = new Station()
            {
                Id = r.Next(10000, 100000),
                Name = "Givat Shaul",
                Longitude = 35.195144,
                Latitude = 31.790835,
                AvailableChargeSlots = r.Next(5, 11),
                isActive = true
            };
            Stations.Add(s);
        }
        private static void createCustomer(int numOfCustomers)
        {
            // minLat, minLon, maxLat,maxLon are variables for making sure the latitude and longitude are in the range
            double minLat = 31.79;
            double minLon = 35.1;
            double maxLat = 31.81;
            double maxLon = 35.21;
            string[] namesArray = { "Avraham Cohen", "Yitshak Levi", "Yaakov Israeli", "Sarah Shalom", "Rivka Silver", "Rahel Shushan", "Leah Yosefi", "David Dayan", "Moshe Biton", "Aharon Uzan" };
            string[] firstDigits = { "050-", "052-", "054-" };
            for (int i = 0; i < numOfCustomers; i++)
            {
                Customer c = new Customer()
                {
                    Id = r.Next(100000000, 300000000),
                    Name = namesArray[i],
                    Phone = firstDigits[r.Next(3)] + r.Next(1000000, 10000000).ToString(),
                    Latitude = minLat + (maxLat - minLat) * r.NextDouble(),
                    Longitude = minLon + (maxLon - minLon) * r.NextDouble(),
                    isActive = true
                };
                Customers.Add(c);
            }

        }

        private static void createDrone(int numOfDrones)
        {
            for (int i = 0; i < numOfDrones; i++)
            {
                Drone d = new Drone()
                {
                    Id = r.Next(1001, 10000),
                    Model = (char)r.Next(65, 91) + ((char)r.Next(65, 91) + r.Next(111, 999).ToString()), // for example: SE503
                    MaxWeight = (WeightCategories)r.Next(3),
                    isActive = true
                };
                Drones.Add(d);
            }
        }

        private static void createParcel()
        {
            Parcel p = new Parcel()
            {
                Id = Config.ParcelId++,
                SenderId = Customers[0].Id,
                TargetId = Customers[1].Id,
                Weight = (WeightCategories)r.Next(3),
                Priority = (Priorities)r.Next(3),
                Requested = new DateTime(2020, 05, 09, 8, 10, 00),
                Scheduled = new DateTime(2020, 05, 09, 8, 30, 00),
                PickedUp = null,
                Delivered = null,
                DroneId = Drones[0].Id
            };
            Parcels.Add(p);

            p = new Parcel()
            {
                Id = Config.ParcelId++,
                SenderId = Customers[1].Id,
                TargetId = Customers[4].Id,
                Weight = (WeightCategories)r.Next(3),
                Priority = (Priorities)r.Next(3),
                Requested = new DateTime(2020, 09, 09, 20, 00, 00),
                Scheduled = new DateTime(2020, 09, 09, 21, 10, 00),
                PickedUp = new DateTime(2020, 09, 09, 21, 30, 00),
                Delivered = null,
                DroneId = Drones[1].Id
            };
            Parcels.Add(p);

            p = new Parcel()
            {
                Id = Config.ParcelId++,
                SenderId = Customers[6].Id,
                TargetId = Customers[3].Id,
                Weight = (WeightCategories)r.Next(3),
                Priority = (Priorities)r.Next(3),
                Requested = new DateTime(2020, 09, 12, 20, 00, 00),
                Scheduled = new DateTime(2020, 09, 12, 21, 10, 00),
                PickedUp = new DateTime(2020, 09, 12, 21, 30, 00),
                Delivered = new DateTime(2020, 09, 12, 22, 00, 00),
                DroneId = Drones[2].Id
            };
            Parcels.Add(p);

            p = new Parcel()
            {
                Id = Config.ParcelId++,
                SenderId = Customers[9].Id,
                TargetId = Customers[0].Id,
                Weight = (WeightCategories)r.Next(3),
                Priority = (Priorities)r.Next(3),
                Requested = new DateTime(2020, 11, 10, 20, 00, 00),
                Scheduled = new DateTime(2020, 11, 10, 21, 10, 00),
                PickedUp = new DateTime(2020, 11, 10, 21, 30, 00),
                Delivered = null,
                DroneId = Drones[3].Id
            };
            Parcels.Add(p);

            p = new Parcel
            {
                Id = Config.ParcelId++,
                SenderId = Customers[2].Id,
                TargetId = Customers[3].Id,
                Weight = (WeightCategories)r.Next(3),
                Priority = (Priorities)r.Next(3),
                Requested = new DateTime(2021, 3, 12, 15, 25, 30),
                Scheduled = null,
                PickedUp = null,
                Delivered = null,
                DroneId = 0
            };
            Parcels.Add(p);

            p = new Parcel
            {
                Id = Config.ParcelId++,
                SenderId = Customers[3].Id,
                TargetId = Customers[7].Id,
                Weight = (WeightCategories)r.Next(3),
                Priority = (Priorities)r.Next(3),
                Requested = new DateTime(2021, 4, 4, 12, 25, 0),
                Scheduled = null,
                PickedUp = null,
                Delivered = null,
                DroneId = 0
            };
            Parcels.Add(p);

            p = new Parcel
            {
                Id = Config.ParcelId++,
                SenderId = Customers[8].Id,
                TargetId = Customers[2].Id,
                Weight = (WeightCategories)r.Next(3),
                Priority = (Priorities)r.Next(3),
                Requested = new DateTime(2021, 8, 1, 1, 12, 56),
                Scheduled = new DateTime(2021, 8, 2, 12, 0, 0),
                PickedUp = new DateTime(2021, 8, 2, 16, 15, 00),
                Delivered = null,
                DroneId = Drones[4].Id
            };
            Parcels.Add(p);

            p = new Parcel
            {
                Id = Config.ParcelId++,
                SenderId = Customers[9].Id,
                TargetId = Customers[1].Id,
                Weight = (WeightCategories)r.Next(3),
                Priority = (Priorities)r.Next(3),
                Requested = new DateTime(2020, 10, 05, 12, 00, 00),
                Scheduled = new DateTime(2020, 10, 05, 12, 10, 00),
                PickedUp = new DateTime(2020, 10, 05, 12, 30, 00),
                Delivered = new DateTime(2020, 10, 05, 13, 00, 00),
                DroneId = Drones[5].Id
            };
            Parcels.Add(p);

            p = new Parcel()
            {
                Id = Config.ParcelId++,
                SenderId = Customers[7].Id,
                TargetId = Customers[4].Id,
                Weight = (WeightCategories)r.Next(3),
                Priority = (Priorities)r.Next(3),
                Requested = new DateTime(2021, 01, 05, 11, 00, 00),
                Scheduled = new DateTime(2021, 01, 05, 12, 00, 00),
                PickedUp = new DateTime(2021, 01, 05, 12, 30, 00),
                Delivered = new DateTime(2021, 01, 05, 13, 00, 00),
                DroneId = Drones[6].Id
            };
            Parcels.Add(p);

            p = new Parcel
            {
                Id = Config.ParcelId++,
                SenderId = Customers[5].Id,
                TargetId = Customers[6].Id,
                Weight = (WeightCategories)r.Next(3),
                Priority = (Priorities)r.Next(3),
                Requested = new DateTime(2021, 10, 13, 0, 8, 0),
                Scheduled = new DateTime(2021, 10, 2, 12, 0, 0),
                PickedUp = new DateTime(2021, 10, 2, 16, 15, 0),
                Delivered = new DateTime(2021, 10, 2, 17, 0, 0),
                DroneId = Drones[7].Id
            };
            Parcels.Add(p);
        }

        static void SaveListToXmlSerializer<T>(IEnumerable<T> list, string filePath)
        {
            //string dirPath = @"xml\";
            try
            {
                FileStream file = new FileStream(filePath, FileMode.Create);
                XmlSerializer x = new XmlSerializer(list.GetType());

                x.Serialize(file, list);
                file.Close();
            }
            catch (Exception ex)
            {
                throw new XmlFailedToLoadCreatException(filePath, $"fail to create xml file: {filePath}", ex);
            }
        }
    }
}


