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
using BO;
using BlApi;

namespace PL
{
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        IBL bl;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="bl">instance of bl</param>
        public SignUpWindow(IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
        }

        /// <summary>
        /// enables user to drag the window arounf the screen
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /// <summary>
        /// close window
        /// </summary>
        private void txtboxClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// create a new customer with the data that was inserted
        /// </summary>
        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Customer tmpCustomer = new Customer()
                {
                    Id = Convert.ToInt32(txtEnterId.Text),
                    Name = txtEnterName.Text,
                    Location = new Location() { Longitude = Convert.ToDouble(txtEnterLongitude.Text), Latitude = Convert.ToDouble(txtEnterLatitude.Text) },
                    Phone = txtEnterPhone.Text
                };
                bl.AddCustomer(tmpCustomer);
                new CustomerWindow(bl, tmpCustomer).Show();
                this.Close();
            }
            catch (BO.IdAlreadyExistsException ex)
            {
                MessageBox.Show(ex.Message, "Error Occurred",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("Not all the fields were filled or one of them is wrong", "Error Occurred",
                   MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
