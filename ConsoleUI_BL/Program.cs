using System.Collections.Generic;
using System;
using System.Linq;
using BlApi;

namespace ConsoleUI_BL
{
    public class Program
    {
        enum MenuOptions { Exit, Add, Update, Show_One, Show_List }
        enum EntityOptions { Exit, Parcel, Drone, BaseStation, Customer }
        enum ListOptions { Exit, BaseStations, Drones, Customers, Parcels, ParcelsWithoutDrone, AvailableChargingStation }
        enum UpdateOptions { Exit, Drone, Station, Customer, Recharge, FreeDrone, DroneToParcel, PickUpParcel, DeliveryPackageByDrone }

        static IBL mybl = BlFactory.GetBl();        
        static void Main(string[] args)
        {
            Console.WriteLine("press 1 to add an item");
            Console.WriteLine("press 2 to update an item");
            Console.WriteLine("press 3 to view details of specific item");
            Console.WriteLine("press 4 to view a list of specific item");
            Console.WriteLine("press 0 to stop");

            MenuOptions menuOptions;
            EntityOptions entityOptions;
            ListOptions listOptions;

            menuOptions = (MenuOptions)int.Parse(Console.ReadLine());

            while (menuOptions != MenuOptions.Exit)
            {
                try
                {
                    switch (menuOptions)
                    {
                        case MenuOptions.Add:
                            {
                                Console.WriteLine("Press 1 to add parcel");
                                Console.WriteLine("Press 2 to add drone");
                                Console.WriteLine("Press 3 to add station");
                                Console.WriteLine("Press 4 to add customer");
                                Console.WriteLine("press 0 to stop");
                                entityOptions = (EntityOptions)int.Parse(Console.ReadLine());
                                //try
                                //{
                                switch (entityOptions)
                                {
                                    case EntityOptions.Parcel:
                                        addParcel();
                                        break;
                                    case EntityOptions.Drone:
                                        addDrone();
                                        break;
                                    case EntityOptions.BaseStation:
                                        addStation();
                                        break;
                                    case EntityOptions.Customer:
                                        addCustomer();
                                        break;
                                    case EntityOptions.Exit:
                                        break;
                                    default:
                                        break;
                                }
                                // }
                                //catch(Exception ex)
                                //{
                                //    Console.WriteLine(ex.Message);
                                //}
                                break;

                            }
                        case MenuOptions.Update:
                            Console.WriteLine("Press 1 to update a drone");
                            Console.WriteLine("Press 2 to update a station");
                            Console.WriteLine("Press 3 to update a customer");
                            Console.WriteLine("Press 4 to send drone to charge");
                            Console.WriteLine("Press 5 to free drone from charging");
                            Console.WriteLine("press 6 to assign a parcel to drone");
                            Console.WriteLine("press 7 to pick up a parcel by drone");
                            Console.WriteLine("press 8 to deliver a parcel by drone");
                            Console.WriteLine("press 0 to stop");
                            UpdateOptions updateChoice = (UpdateOptions)Enum.Parse(typeof(UpdateOptions), Console.ReadLine());
                            #region Update switch
                            try
                            {
                                switch (updateChoice)
                                {
                                    case UpdateOptions.Drone:
                                        updateDrone();
                                        break;
                                    case UpdateOptions.Station:
                                        updateStation();
                                        break;
                                    case UpdateOptions.Customer:
                                        updateCustomer();
                                        break;
                                    case UpdateOptions.Recharge:
                                        rechargeDrone();
                                        break;
                                    case UpdateOptions.FreeDrone:
                                        FreeDrone();
                                        break;
                                    case UpdateOptions.DroneToParcel:
                                        DroneToParcel();
                                        break;
                                    case UpdateOptions.PickUpParcel:
                                        pickUpParcel();
                                        break;
                                    case UpdateOptions.DeliveryPackageByDrone:
                                        deliveryPackage();
                                        break;
                                    case UpdateOptions.Exit:
                                        break;
                                    default:
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            #endregion
                            break;
                        case MenuOptions.Show_One:
                            Console.WriteLine("press 1 to view details of a specific parcel");
                            Console.WriteLine("press 2 to view details of a specific drone");
                            Console.WriteLine("press 3 to view details of a specific base station");
                            Console.WriteLine("press 4 to view details of a specific customer");
                            entityOptions = (EntityOptions)int.Parse(Console.ReadLine());

                            switch (entityOptions)
                            {
                                case EntityOptions.Parcel:
                                    printParcel();
                                    break;
                                case EntityOptions.Drone:
                                    printDrone();
                                    break;
                                case EntityOptions.BaseStation:
                                    printStation();
                                    break;
                                case EntityOptions.Customer:
                                    printCustomer();
                                    break;
                                case EntityOptions.Exit:
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case MenuOptions.Show_List:
                            Console.WriteLine("press 1 to view the list of base stations");
                            Console.WriteLine("press 2 to view the list of the drones");
                            Console.WriteLine("press 3 to view the list of the cutomers");
                            Console.WriteLine("press 4 to view the list of the parcels");
                            Console.WriteLine("press 5 to view the list of the parcels without drones");
                            Console.WriteLine("press 6 to view the list of the stations with available charge slots");
                            listOptions = (ListOptions)int.Parse(Console.ReadLine());
                            #region get list switch
                            switch (listOptions)
                            {
                                case ListOptions.BaseStations:
                                    ShowList(mybl.GetListOfStations());
                                    break;
                                case ListOptions.Drones:
                                    ShowList(mybl.GetListOfDrones());

                                    break;
                                case ListOptions.Customers:
                                    ShowList(mybl.GetListOfCustomers());
                                    break;
                                case ListOptions.Parcels:
                                    ShowList(mybl.GetListofParcels());
                                    break;
                                case ListOptions.ParcelsWithoutDrone:
                                    PrintParcelsWithoutDroneList();
                                    break;
                                case ListOptions.AvailableChargingStation:
                                    PrintAvailableChargeSlotsList();
                                    break;
                                case ListOptions.Exit:
                                    break;
                                default:
                                    break;
                            }
                            #endregion
                            break;
                        case MenuOptions.Exit:
                            break;
                        default:
                            break;
                    }
                }
                catch (BO.IdAlreadyExistsException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (BO.ImpossibleOprationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (BO.EmptyListException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (BO.NoMatchingIdException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (BO.NoUpdateException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (WrongInputFormatException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                Console.WriteLine("press 1 to add an item");
                Console.WriteLine("press 2 to update an item");
                Console.WriteLine("press 3 to view details of specific item");
                Console.WriteLine("press 4 to view a list of specific item");
                Console.WriteLine("press 0 to stop");
                menuOptions = (MenuOptions)int.Parse(Console.ReadLine());

            }
        }


        //-----------------Add-----------------//

        private static void addDrone()
        {
            int droneId, stationId;
            string model;
            BO.WeightCategories weight;
            Console.WriteLine("Enter drone ID: ");
            if (!int.TryParse(Console.ReadLine(), out droneId))
                throw new WrongInputFormatException("input must be a number/n");
            Console.WriteLine("Enter drone model: ");
            model = Console.ReadLine();
            Console.WriteLine("Maximum weight of the parcel: press 1 for heavy, 2 for medium and 3 for light: ");
            int tmp;
            int.TryParse(Console.ReadLine(), out tmp);
            weight = (BO.WeightCategories)(tmp - 1);
            Console.WriteLine("Enter ID of station to charge: ");
            if (!int.TryParse(Console.ReadLine(), out stationId))
                throw new WrongInputFormatException("input must be a number/n");

            BO.Drone newDrone = new BO.Drone
            {
                Id = droneId,
                Model = model,
                MaxWeight = weight
            };
            mybl.AddDrone(newDrone, stationId);
            Console.WriteLine("drone was added successfully!");

        }

        private static void addStation()
        {
            int id, slots;
            string name;
            double latitude, longitude;
            Console.WriteLine("Enter station ID: ");
            if (!int.TryParse(Console.ReadLine(), out id))
                throw new WrongInputFormatException("input must be a number/n");
            Console.WriteLine("Enter station name: ");
            name = Console.ReadLine();
            Console.WriteLine("Enter latitude");
            double.TryParse(Console.ReadLine(), out latitude);
            Console.WriteLine("Enter longitude");
            double.TryParse(Console.ReadLine(), out longitude);
            Console.WriteLine("Enter number of charging slots: ");
            int.TryParse(Console.ReadLine(), out slots);
            BO.Station newStation = new BO.Station
            {
                Id = id,
                Name = name,
                AvailableChargeSlots = slots,
                Location = new BO.Location { Latitude = latitude, Longitude = longitude },
                DronesCharging = new List<BO.DroneInCharging>()
            };
            mybl.AddStation(newStation);
            Console.WriteLine("station was added successfully!");
        }

        private static void addCustomer()
        {
            int id;
            string name, phone;
            double longitude, latitude;
            Console.WriteLine("Enter customer ID: ");
            if (!int.TryParse(Console.ReadLine(), out id))
                throw new WrongInputFormatException("input must be a number/n");
            Console.WriteLine("Enter customer name: ");
            name = Console.ReadLine();
            Console.WriteLine("Enter phone number: ");
            phone = Console.ReadLine();
            Console.WriteLine("Enter customer longitude: ");
            double.TryParse(Console.ReadLine(), out longitude);
            Console.WriteLine("Enter customer latitude: ");
            double.TryParse(Console.ReadLine(), out latitude);

            BO.Location location = new BO.Location
            {
                Longitude = longitude,
                Latitude = latitude
            };

            BO.Customer newCustomer = new BO.Customer
            {
                Id = id,
                Name = name,
                Phone = phone,
                Location = location
            };
            mybl.AddCustomer(newCustomer);
            Console.WriteLine("customer was added successfully!");

        }

        private static void addParcel()
        {
            int senderId, targetId;
            Console.WriteLine("Enter sender ID: ");
            if (!int.TryParse(Console.ReadLine(), out senderId))
                throw new WrongInputFormatException("input must be a number");
            Console.WriteLine("Enter target ID: ");
            if (!int.TryParse(Console.ReadLine(), out targetId))
                throw new WrongInputFormatException("input was not int");
            Console.WriteLine("Weight of the parcel: press 1 for heavy, 2 for medium and 3 for light: ");
            BO.WeightCategories weight = (BO.WeightCategories)Enum.Parse(typeof(BO.WeightCategories), Console.ReadLine());
            Console.WriteLine("Priorities of the parcel: press 1 for regular, 2 for rapid and 3 for emergency: ");
            BO.Priorities priority = (BO.Priorities)Enum.Parse(typeof(BO.Priorities), Console.ReadLine());
            BO.Parcel parcel = new BO.Parcel
            {
                Sender = new BO.CustomerInParcel { Id = senderId },
                Target = new BO.CustomerInParcel { Id = targetId },
                Weight = weight,
                Priority = priority
            };
            mybl.AddParcel(parcel);
            Console.WriteLine("parcel was added successfully!");

        }

        //-----------------Update-----------------//

        private static void updateDrone()
        {
            int id;
            string model;
            Console.WriteLine("Enter ID of the drone");
            if (!int.TryParse(Console.ReadLine(), out id))
                throw new WrongInputFormatException("input must be a number");
            Console.WriteLine("Enter model of the drone");
            model = Console.ReadLine();
            BO.Drone newDrone = new BO.Drone()
            {
                Id = id,
                Model = model,
            };
            mybl.UpdateDrone(newDrone);
            Console.WriteLine("Drone was added successpully!");

        }


        private static void rechargeDrone()
        {
            int id;
            Console.WriteLine("Enter ID of drone");
            int.TryParse(Console.ReadLine(), out id);
            //BO.Drone newDrone = new BO.Drone()
            //{
            //    Id = id
            //};
            mybl.RechargeDrone(id);
            Console.WriteLine("Recharge drone action was done successfully!");

        }

        private static void pickUpParcel()
        {
            int id;
            Console.WriteLine("Enter drone ID");
            if (!int.TryParse(Console.ReadLine(), out id))
                throw new WrongInputFormatException("input must be a number");
            BO.Drone newDrone = new BO.Drone
            {
                Id = id
            };
            mybl.PickUpParcel(newDrone);
            Console.WriteLine("Pick up parcel action was done successfully!");

        }

        private static void deliveryPackage()
        {
            int id;
            Console.WriteLine("Enter drone ID");
            if (!int.TryParse(Console.ReadLine(), out id))
                throw new WrongInputFormatException("input must be a number");
            BO.Drone newDrone = new BO.Drone { Id = id };
            mybl.deliveryPackage(newDrone);
            Console.WriteLine("Delivery parcel action was done succesfpully!");

        }

        private static void DroneToParcel()
        {
            int id;
            Console.WriteLine("Enter drone ID: ");
            if (!int.TryParse(Console.ReadLine(), out id))
                throw new WrongInputFormatException("input must be a number\n");
            mybl.DroneToParcel(id);
            Console.WriteLine("Assign drone action was done successfully!");

        }

        private static void FreeDrone()
        {
            int id;
            Console.WriteLine("Enter drone ID: ");
            if (!int.TryParse(Console.ReadLine(), out id))
                throw new WrongInputFormatException("input must be a number\n");
            mybl.FreeDrone(id);
            Console.WriteLine("Free drone action was done successfully!");

        }

        private static void updateStation()
        {
            int id, chargeSlots;
            string name;
            Console.WriteLine("Enter station ID: ");
            if (!int.TryParse(Console.ReadLine(), out id))
                throw new WrongInputFormatException("input must be a number\n");
            Console.WriteLine("Enter station name: ");
            name = Console.ReadLine();
            Console.WriteLine("Enter station number of charge slots: ");
            if (!int.TryParse(Console.ReadLine(), out chargeSlots))
                throw new WrongInputFormatException("input must be a number\n");
            mybl.UpdateStation(id,name,chargeSlots);
            Console.WriteLine("Station was updated successpully!");
        }

        private static void updateCustomer()
        {
            int id;
            string name, phone;
            Console.WriteLine("Enter customer ID: ");
            if (!int.TryParse(Console.ReadLine(), out id))
                throw new WrongInputFormatException("input must be a number\n");
            Console.WriteLine("Enter customer name: ");
            name = Console.ReadLine();
            Console.WriteLine("Enter phone number: ");
            phone = Console.ReadLine();

            BO.Customer newCustomer = new BO.Customer
            {
                Id = id,
                Name = name,
                Phone = phone,
            };
            mybl.UpdateCustomer(newCustomer);
            Console.WriteLine("Customer was updated successpully!");

        }

        //-----------------View-----------------//

        private static void printParcel()
        {
            int id;
            Console.WriteLine("enter ID of the parcel");
            id = int.Parse(Console.ReadLine());
            if (id.ToString() == "")
                throw new WrongInputFormatException("input must be a number\n");
            BO.Parcel parcelForView = mybl.GetParcel(id);
            Console.WriteLine(parcelForView);
        }

        private static void printDrone()
        {
            int id;
            Console.WriteLine("enter ID of the drone");
            id = int.Parse(Console.ReadLine());
            if (id.ToString() == "")
                throw new WrongInputFormatException("input must be a number\n");
            Console.WriteLine(mybl.GetDrone(id));
        }

        private static void printStation()
        {
            int id;
            Console.WriteLine("enter ID of the station");
            id = int.Parse(Console.ReadLine());
            if (id.ToString() == "")
                throw new WrongInputFormatException("input must be a number\n");
            BO.Station stationForView = mybl.GetStation(id);
            Console.WriteLine(stationForView);
        }

        private static void printCustomer()
        {
            int id;
            Console.WriteLine("enter ID of the customer");
            id = int.Parse(Console.ReadLine());
            if (id.ToString() == "")
                throw new WrongInputFormatException("input must be a number\n");
            BO.Customer customerForView = mybl.GetCustomer(id);
            Console.WriteLine(customerForView);
        }

      
        private static void PrintParcelsWithoutDroneList()
        {
            foreach (var parcelWithoutDrone in mybl.GetListofParcelsWithoutDrone())
            {
                Console.WriteLine(parcelWithoutDrone);
            }
        }

        private static void PrintAvailableChargeSlotsList()
        {
            foreach (var station in mybl.GetListOfStationsWithAvailableChargeSlots())
            {
                Console.WriteLine(station);
            }
        }
        private static void ShowList<T>(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                Console.WriteLine(item);
            }
        }




    }

}

