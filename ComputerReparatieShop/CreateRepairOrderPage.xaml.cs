using ComputerReparatieShop.DAL;
using ComputerReparatieShop.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ComputerReparatieShop
{
    /// <summary>
    /// Interaction logic for CreateRepairOrderPage.xaml
    /// </summary>
    public partial class CreateRepairOrderPage : Page
    {
        MyContext db = new MyContext();
        public CreateRepairOrderPage()
        {   
            InitializeComponent();
            CustomerBox.ItemsSource = db.Customers.ToList();
            EmployeeBox.ItemsSource = db.Employees.ToList();
        }

        private void CreateRepairOrderButton_Click(object sender, RoutedEventArgs e)
        {
            AddRepairOrder();
        }

        public void AddRepairOrder()
        {
            CustomerModel selectedCustomer = CustomerBox.SelectedItem as CustomerModel;
            EmployeeModel selectedEmployee = EmployeeBox.SelectedItem as EmployeeModel;
            DateTime? startDate = StartDateBox.SelectedDate;
            DateTime? endDate = EndDateBox.SelectedDate;

            if(selectedCustomer == null)
            {
                SelectCustomerLabel.Foreground = Brushes.Red;
                SelectCustomerLabel.Content += " *";
                ErrorLabel.Visibility = Visibility.Visible;
            }

            if(startDate == null)
            {
                SelectStartDateLabel.Foreground = Brushes.Red;
                SelectStartDateLabel.Content += " *";
                ErrorLabel.Visibility = Visibility.Visible;
            }

            if(endDate == null)
            {
                SelectEndDateLabel.Foreground = Brushes.Red;
                SelectEndDateLabel.Content += " *";
                ErrorLabel.Visibility = Visibility.Visible;
            }
        }
    }
}
