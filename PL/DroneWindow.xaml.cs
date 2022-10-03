using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BlApi;
using BO;


namespace PL
{
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public partial class DroneWindow : Window
    {
        IBL bl;
        Drone drone;
        int stationId;
        public event Action Update = delegate { };

        // add drone ctor
        public DroneWindow(IBL bl)
        {
            this.bl = bl;
            drone = new Drone();
            InitializeComponent();

            // initialize source of the choose weight combobox
            comboWeightSelcetor.ItemsSource = Enum.GetValues(typeof(WeightCategories));

            // initialize xource of the choose station combobox
            List<int> listOfStationIds = new List<int>();
            foreach (var s in bl.GetListOfStationsWithAvailableChargeSlots())
                listOfStationIds.Add(s.Id);
            comboStationSelector.ItemsSource = listOfStationIds;

            // hide action mode grid
            ActionsGrid.Visibility = Visibility.Hidden;
        }

        // actions with drone ctor
        public DroneWindow(IBL bl, DroneToList d)
        {
            InitializeComponent();
            this.bl = bl;
          
            drone = bl.GetDrone(d.Id);
            DataContext = drone;

            txtParcelInDeliveryData.Text = Convert.ToInt32(drone.ParcelInDelivery.Id) == 0 ? "none" : drone.ParcelInDelivery.ToString();

            // hide add drone mode grid
            AddDroneGrid.Visibility = Visibility.Hidden;


            // find the drone status
            int status;
            if (d.ParcelInDeliveryId == 0)
            //if the drone doesn't carry a parcel
            {
                if (d.DroneStatus == DroneStatus.Available)
                    status = 1;         // the drone is available 
                else status = 2;        // the drone is in maintenance
            }
            else
            // the drone is carrying a parcel
            {
                Parcel tmpParcel = bl.GetParcel(d.ParcelInDeliveryId);
                // check status of the parcel that is being carried
                if (tmpParcel.PickedUp == null)
                    // if the parcel wasn't picked up
                    status = 3;         // the drone is picking up parcel
                else status = 4;        // the drone is in delivery
            }

            switch (status)
            // hide the irrelevant buttons according to the drone status 
            {
                case 1:
                    btnFreeDroneFromCharging.Visibility = Visibility.Hidden;
                    btnPickUpParcel.Visibility = Visibility.Hidden;
                    btnDeliverParcel.Visibility = Visibility.Hidden;
                    break;

                case 3:
                    btnSendToCharge.Visibility = Visibility.Hidden;
                    btnSendToDelivery.Visibility = Visibility.Hidden;
                    btnFreeDroneFromCharging.Visibility = Visibility.Hidden;
                    btnDeliverParcel.Visibility = Visibility.Hidden;
                    break;

                case 2:
                    btnSendToCharge.Visibility = Visibility.Hidden;
                    btnSendToDelivery.Visibility = Visibility.Hidden;
                    btnPickUpParcel.Visibility = Visibility.Hidden;
                    btnDeliverParcel.Visibility = Visibility.Hidden;
                    break;

                case 4:
                    btnSendToCharge.Visibility = Visibility.Hidden;
                    btnSendToDelivery.Visibility = Visibility.Hidden;
                    btnFreeDroneFromCharging.Visibility = Visibility.Hidden;
                    btnPickUpParcel.Visibility = Visibility.Hidden;
                    break;
            }

        }



        // ADD DRONE WINDOW FUNCTIONS 

        /// <summary>
        /// the event saves id the user input in a drone
        /// </summary>
        private void TextBox_TextChanged_DroneId(object sender, TextChangedEventArgs e)
        {
            drone.Id = Convert.ToInt32(txtDroneId.Text);
        }

        /// <summary>
        /// function will only allow user to enter numbers
        /// other chars won't e shown in the textbosx
        /// </summary>
        private void TextBox_OnlyNumbers_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //allow get out of the text box
            if (e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Tab)
                return;

            //allow list of system keys (add other key here if you want to allow)
            if (e.Key == Key.Escape || e.Key == Key.Back || e.Key == Key.Delete ||
                e.Key == Key.CapsLock || e.Key == Key.LeftShift || e.Key == Key.Home
             || e.Key == Key.End || e.Key == Key.Insert || e.Key == Key.Down || e.Key == Key.Right)
                return;

            char c = (char)KeyInterop.VirtualKeyFromKey(e.Key);

            //allow control system keys
            if (Char.IsControl(c)) return;

            //allow digits (without Shift or Alt)
            if (Char.IsDigit(c))
                if (!(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightAlt)))
                    return; //let this key be written inside the textbox

            //forbid letters and signs (#,$, %, ...)
            e.Handled = true; //ignore this key. mark event as handled, will not be routed to other controls

            return;
        }

        /// <summary>
        /// the event saves the weight the user chose in drone
        /// </summary>
        private void comboWeightSelcetor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WeightCategories weight = (WeightCategories)Convert.ToInt32(comboWeightSelcetor.SelectedItem);
            drone.MaxWeight = weight;
        }

        /// <summary>
        /// the event saves the model user input in a drone
        /// </summary>
        private void TextBox_TextChanged_DroneModel(object sender, TextChangedEventArgs e)
        {
            drone.Model = Convert.ToString(txtDroneModel.Text);
        }

        /// <summary>
        /// the event saves the station the user chose in drone
        /// </summary>
        private void comboStationSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            drone.DroneStatus = DroneStatus.Maintenance;
            stationId = Convert.ToInt32(comboStationSelector.SelectedItem);
        }

        /// <summary>
        /// the event sends the new drone to the addDrone method in BL
        /// </summary>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtDroneId.Text) || string.IsNullOrEmpty(txtDroneModel.Text) || string.IsNullOrEmpty(comboWeightSelcetor.Text) || string.IsNullOrEmpty(comboStationSelector.Text))
                    throw new EmptyInputException("Insert all details of the drone!");
                bl.AddDrone(drone, stationId);
                MessageBox.Show("Drone was added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                Close();
            }
            catch (IdAlreadyExistsException ex)
            {
                MessageBox.Show(ex.Message, "Error Occurred",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (NoMatchingIdException ex)
            {
                MessageBox.Show(ex.Message, "Error Occurred",
                   MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (EmptyInputException ex)
            {
                MessageBox.Show(ex.Message, "Error Occurred",
                   MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// close window
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        // ACTIONS WITH DRONE FUNCTIONS

        /// <summary>
        /// send the updated drone to update function in bl
        /// </summary>
        private void btnFinalUpdate_Click(object sender, RoutedEventArgs e)
        {
            Drone tmpDrone = drone;
            tmpDrone.Model = txtModelData.Text;
            try
            {
                bl.UpdateDrone(tmpDrone);
                drone = tmpDrone;
                MessageBox.Show($"Drone {drone.Id} model was updated successfully \npress OK to continue", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (NoUpdateException)
            {
                MessageBox.Show("No update was made \npress OK to continue", "Error Occurred",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("Error \npress OK to continue", "Error Occurred",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Close();
            new DroneWindow(bl, bl.GetDroneToList(drone.Id)).Show();
        }


        /// <summary>
        /// send available drone to charge 
        /// </summary>
        private void btnSendToCharge_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.RechargeDrone(drone.Id);
                MessageBox.Show($"Drone {drone.Id} was sent to charge successfully\npress OK to continue", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (ImpossibleOprationException)
            {
                MessageBox.Show("Couldn't send drone to charge \npress OK to continue", "Error Occurred",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("Error \npress OK to continue", "Error Occurred",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Close();
            new DroneWindow(bl, bl.GetDroneToList(drone.Id)).Show();
        }

        /// <summary>
        /// send available drone on a delivery and match it to a parcel
        /// </summary>
        private void btnSendToDelivery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.DroneToParcel(drone.Id);
                MessageBox.Show($"Drone {drone.Id} was sent on delivery successfully\npress OK to continue", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (ImpossibleOprationException)
            {
                MessageBox.Show("Couldn't pick up parcel \npress OK to continue, else press Cancel", "Error Occurred",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch(EmptyListException)
            {
                MessageBox.Show("No parcels to assign\npress OK to continue, else press Cancel", "Error Occurred",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("Error \npress OK to continue, else press Cancel", "Error Occurred",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Close();
            new DroneWindow(bl, bl.GetDroneToList(drone.Id)).Show();
        }

        /// <summary>
        /// free drone in maintenance from charging
        /// </summary>
        private void btnFreeDroneFromCharging_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.FreeDrone(drone.Id);
                MessageBox.Show($"Drone {drone.Id} was freed from station successfully\npress OK to continue", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (ImpossibleOprationException)
            {
                MessageBox.Show("Couldn't free drone from charging \npress OK to continue, else press Cancel", "Error Occurred",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("Error \npress OK to continue, else press Cancel", "Error Occurred",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Close();
            new DroneWindow(bl, bl.GetDroneToList(drone.Id)).Show();
        }

        /// <summary>
        /// send drone that was assigned to a parcel tp pick up that parcel
        /// </summary>
        private void btnPickUpParcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.PickUpParcel(drone);
                MessageBox.Show($"Drone {drone.Id} picked-up parcel {drone.ParcelInDelivery.Id} successfully\npress OK to continue", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (NoMatchingIdException)
            {
                MessageBox.Show("No parcel could be picked-up \npress OK to continue, else press Cancel", "Error Occurred",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch(ImpossibleOprationException ex)
            {
                MessageBox.Show(ex + "\npress OK to continue, else press Cancel", "Error Occurred",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("Error \npress OK to continue, else press Cancel", "Error Occurred",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Close();
            new DroneWindow(bl, bl.GetDroneToList(drone.Id)).Show();
        }

        /// <summary>
        /// complete a delivery and make the drone available again 
        /// </summary>
        private void btnDeliverParcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.deliveryPackage(drone);
                MessageBox.Show($"Drone {drone.Id} completed delivery successfully\npress OK to continue", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (ImpossibleOprationException)
            {
                MessageBox.Show("Parcel can't be delivered \npress OK to continue, else press Cancel", "Error Occurred",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("Error \npress OK to continue, else press Cancel", "Error Occurred",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Close();
            new DroneWindow(bl, bl.GetDroneToList(drone.Id)).Show();
        }

        /// <summary>
        /// close window
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// send drone to delete in bl
        /// </summary>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.DeleteDrone(drone.Id);
                btnDelete.Visibility = Visibility.Hidden;
                btnDeliverParcel.Visibility = Visibility.Hidden;
                btnFinalUpdate.Visibility = Visibility.Hidden;
                btnFreeDroneFromCharging.Visibility = Visibility.Hidden;
                btnPickUpParcel.Visibility = Visibility.Hidden;
                btnSendToCharge.Visibility = Visibility.Hidden;
                btnSendToDelivery.Visibility = Visibility.Hidden;
            }
            catch (NoMatchingIdException ex)
            {
                MessageBox.Show(ex.Message + "\npress OK to continue, else press Cancel", "Error Occurred", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //private void btnSimulator_Click(object sender, RoutedEventArgs e)
        //{
        //    //Auto = true;
        //    worker = new() { WorkerReportsProgress = true, WorkerSupportsCancellation = true, };
        //    worker.DoWork += (sender, args) => bl.StartDroneSimulator((int)args.Argument, updateDrone, checkStop);
        //    worker.RunWorkerCompleted += (sender, args) =>
        //    {
        //        //Auto = false;
        //        worker = null;
        //        if (closing) Close();
        //    };
        //    worker.ProgressChanged += (sender, args) => updateDroneView();
        //    worker.RunWorkerAsync(drone.Id);
        //}


        //BackgroundWorker worker;
        //private void updateDrone() => worker.ReportProgress(0);
        //private bool checkStop() => worker.CancellationPending;

        //private void Auto_Click(object sender, RoutedEventArgs e)
        //{
            
        //}

        //private void Manual_Click(object sender, RoutedEventArgs e) => worker?.CancelAsync();

        //bool closing = false;
        //private void Window_Closing(object sender, CancelEventArgs e)
        //{
        //    if (worker != null)
        //    {
        //        closing = true;
        //        e.Cancel = true;
        //    }
        //}

    }
}
