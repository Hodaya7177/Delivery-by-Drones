using BlApi;
using BO;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        IBL bl;
        Customer customer;

        /// <summary>
        /// add customer window constructor
        /// </summary>
        /// <param name="bl">instance of bl</param>
        public CustomerWindow(IBL bl)
        {
            this.bl = bl;
            customer = new Customer();
            InitializeComponent();

            // hide action mode grid
            ActionsGrid.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// customer details constructor
        /// </summary>
        /// <param name="bl">instance of bl</param>
        /// <param name="c">customer to display</param>
        public CustomerWindow(IBL bl, Customer c)
        {
            InitializeComponent();
            this.bl = bl;
            customer = c;
            DataContext = customer;
            ParcelsSentListView.ItemsSource = customer.Send;
            ParcelsReceivedListView.ItemsSource = customer.Receive;
            AddCustomerGrid.Visibility = Visibility.Hidden;

            if (!customer.isActive) //if this customer is deleted, don't enable the user to delete or update this customer
            {
                btnDelete.Visibility = Visibility.Hidden;
                btnFinalUpdate.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// send the updated customer to update function in bl
        /// </summary>
        private void btnFinalUpdate_Click(object sender, RoutedEventArgs e)
        {
            Customer tmpCustomer = customer;
            tmpCustomer.Name = txtNameData.Text;
            tmpCustomer.Phone = txtPhoneData.Text;
            try
            {
                bl.UpdateCustomer(tmpCustomer);
                customer = tmpCustomer;
                MessageBox.Show($"Customer {customer.Id} was updated successfully", "Success",
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
            Close();
            new CustomerWindow(bl, bl.GetCustomer(customer.Id)).Show();
        }

        /// <summary>
        /// close window
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// create a new customer with the data and send to add in bl
        /// </summary>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtCustomerId.Text) || string.IsNullOrEmpty(txtCustomerName.Text) || 
                    string.IsNullOrEmpty(txtCustomerPhone.Text) || string.IsNullOrEmpty(txtCustomerLat.Text) || string.IsNullOrEmpty(txtCustomerLong.Text))
                    throw new EmptyInputException("Insert all details of the customer!");
                if (Math.Abs(Convert.ToDouble(txtCustomerLong.Text)) > 180 || Math.Abs(Convert.ToDouble(txtCustomerLat.Text)) > 180)
                    throw new InvalidInputException("The longitude or latitude are no valid");
                if (Convert.ToDouble(txtCustomerLat.Text) < 31.79 || Convert.ToDouble(txtCustomerLat.Text) > 31.81
                        || Convert.ToDouble(txtCustomerLong.Text) < 35.1 || Convert.ToDouble(txtCustomerLong.Text) > 35.21)
                    throw new InvalidInputException("We operate our delivery services in Jerusalem only");

                Customer c = new Customer
                {
                    Id = Convert.ToInt32(txtCustomerId.Text),
                    Name = Convert.ToString(txtCustomerName.Text),
                    Phone = Convert.ToString(txtCustomerPhone.Text),
                    Location = new Location
                    {
                        Longitude = Convert.ToDouble(txtCustomerLong.Text),
                        Latitude = Convert.ToDouble(txtCustomerLat.Text)
                    },
                    isActive = true
                };
                bl.AddCustomer(c);
                customer = c;
                MessageBox.Show("Customer was added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
            catch (InvalidInputException ex)
            {
                MessageBox.Show(ex.Message, "Error Occurred",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("Error Occured", "Error Occurred",
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

        /// <summary>
        /// validation of input - only allow numbers
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
        /// open parcel details window of the parcel that was double clicked
        /// </summary>
        private void ReceivedlistView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ParcelInCustomer tmpParcel = (ParcelInCustomer)ParcelsReceivedListView.SelectedItem;
                Parcel p = bl.GetParcel(tmpParcel.Id);
                new ParcelWindow(bl, p).Show();
            }
            catch(BO.NoMatchingIdException ex)
            {
                MessageBox.Show(ex.Message, "Error Occurred", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// open parcel details window of the parcel that was double clicked
        /// </summary>
        private void DeliveredlistView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ParcelInCustomer tmpParcel = (ParcelInCustomer)ParcelsSentListView.SelectedItem;
                Parcel p = bl.GetParcel(tmpParcel.Id);
                new ParcelWindow(bl, p).Show();
            }
            catch (BO.NoMatchingIdException ex)
            {
                MessageBox.Show(ex.Message, "Error Occurred", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// allow customer to add a new parcel and opan add parcel window
        /// </summary>
        private void btnNewParcel_Click(object sender, RoutedEventArgs e)
        {
            new ParcelWindow(bl).Show();
        }

        /// <summary>
        /// delete current customer 
        /// </summary>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.DeleteCustomer(customer.Id);
                MessageBox.Show($"Customer {customer.Id} was deleted successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch(BO.NoMatchingIdException ex)
            {
                MessageBox.Show(ex.Message + "\npress OK to continue", "Error Occurred", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
