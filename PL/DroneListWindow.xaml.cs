using System;
using System.Collections.Generic;
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
using System.Runtime.CompilerServices;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneListWindow.xaml
    /// </summary>
    public partial class DroneListWindow : Window
    {
        IBL bl;

        /// <summary>
        /// drone list window constructor
        /// </summary>
        /// <param name="bl">instance of bl</param>
        public DroneListWindow(IBL bl)
        {
            this.bl = bl;
            InitializeComponent();

            DronesListView.ItemsSource = bl.GetListOfDrones().ToList();
            comboStatusSelector.ItemsSource = Enum.GetValues(typeof(DroneStatus));
            comboMaxWeightSelector.ItemsSource = Enum.GetValues(typeof(WeightCategories));
        }

        /// <summary>
        /// combobox filters the list with a specific weight and/or priority
        /// </summary>
        private void comboCombineStatusAndWeight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            cboxStatusSort.IsChecked = false;
            if (comboMaxWeightSelector.SelectedItem != null && comboStatusSelector.SelectedItem != null)
                DronesListView.ItemsSource = bl.GetListOfDrones().Where(d => d.MaxWeight == (WeightCategories)comboMaxWeightSelector.SelectedItem && d.DroneStatus == (DroneStatus)comboStatusSelector.SelectedItem);
            else if (comboMaxWeightSelector.SelectedItem != null)
                DronesListView.ItemsSource = bl.GetListOfDrones().Where(d => d.MaxWeight == (WeightCategories)comboMaxWeightSelector.SelectedItem);
            else if (comboStatusSelector.SelectedItem != null)
                DronesListView.ItemsSource = bl.GetListOfDrones().Where(d => d.DroneStatus == (DroneStatus)comboStatusSelector.SelectedItem);
        }

        /// <summary>
        /// open add drone window
        /// </summary>
        private void btnAddDrone_Click(object sender, RoutedEventArgs e)
        {
            DroneToList tmpDrone = new DroneToList();
            tmpDrone = (DroneToList)DronesListView.SelectedItem;
            DroneWindow dw = new DroneWindow(bl);
            dw.Closed += Dw_Closed;
            dw.Show();
        }

        /// <summary>
        /// close window
        /// </summary>
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// open the details page of the drone that was double clicked
        /// </summary>
        private void DronesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if ((DroneToList)DronesListView.SelectedItem is DroneToList)
                {
                    DroneToList tmpDrone = new DroneToList();
                    tmpDrone = (DroneToList)DronesListView.SelectedItem;
                    if (!tmpDrone.isActive)
                        throw new NoMatchingIdException("The drone is not active !!");
                    DroneWindow dw = new DroneWindow(bl, bl.GetDroneToList(tmpDrone.Id));
                    dw.Closed += Dw_Closed;
                    dw.Show();
                }
            }
            catch(NoMatchingIdException ex)
            {
                MessageBox.Show(ex.Message + "\npress OK to continue, else press Cancel", "Error Occurred", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch(Exception)
            {
                MessageBox.Show("Error\npress OK to continue, else press Cancel", "Error Occurred", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// occurs before a drone details window closes and updates the list
        /// </summary>
        private void Dw_Closed(object sender, EventArgs e)
        {
            DronesListView.Items.Refresh();
            comboCombineStatusAndWeight_SelectionChanged(this, null);
        }
        
        /// <summary>
        /// clear grouping
        /// </summary>
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            cboxStatusSort.IsChecked = false;
            DronesListView.ItemsSource = bl.GetListOfDrones();
        }

        /// <summary>
        /// group drones list by status
        /// </summary>
        private void cboxStatusSort_Checked(object sender, RoutedEventArgs e)
        {
            
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(DronesListView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("DroneStatus");
            view.GroupDescriptions.Add(groupDescription);
        }

        /// <summary>
        /// un-group the drones list
        /// </summary>
        private void cboxStatusSort_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(DronesListView.ItemsSource);
            view.GroupDescriptions.Clear();
        }
    }
}
