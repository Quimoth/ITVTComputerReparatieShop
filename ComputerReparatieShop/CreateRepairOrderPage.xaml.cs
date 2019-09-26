using ComputerReparatieShop.DAL;
using ComputerReparatieShop.Models;
using System.Collections.ObjectModel;
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
using System.Collections.Specialized;

namespace ComputerReparatieShop
{
    /// <summary>
    /// Interaction logic for CreateRepairOrderPage.xaml
    /// </summary>
    public partial class CreateRepairOrderPage : Page
    {
        Data data = MainWindow.data;
        MyContext db = MyContext.Create();
        public CreateRepairOrderPage()
        {
            InitializeComponent();
            CustomerBox.ItemsSource = db.Customers.ToList();
            EmployeeBox.ItemsSource = db.Employees.ToList();

            StartDateBox.BlackoutDates.AddDatesInPast();
            EndDateBox.BlackoutDates.AddDatesInPast();
            //Makes sure the endDate is at least tomorrow
            EndDateBox.BlackoutDates.Add(new CalendarDateRange(DateTime.Now, DateTime.Now.AddDays(1)));
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

            #region Check required fields for nulls
            bool noNulls = true;
            if (selectedCustomer == null)
            {
                SelectCustomerLabel.Foreground = Brushes.Red;
                if (!SelectCustomerLabel.Content.ToString().Contains('*'))
                {
                    SelectCustomerLabel.Content += "*";
                }
                Required();
                noNulls = false;
            }
            if (startDate == null)
            {
                SelectStartDateLabel.Foreground = Brushes.Red;
                if (!SelectStartDateLabel.Content.ToString().Contains('*'))
                {
                    SelectStartDateLabel.Content += "*";
                }
                Required();
                noNulls = false;
            }
            if (endDate == null)
            {
                SelectEndDateLabel.Foreground = Brushes.Red;
                if (!SelectEndDateLabel.Content.ToString().Contains('*'))
                {
                    SelectEndDateLabel.Content += "*";
                }
                Required();
                noNulls = false;
            }
            #endregion

            if (noNulls)
            {
                RepairOrderModel repair = new RepairOrderModel
                {
                    Customer = selectedCustomer,
                    StartDate = startDate,
                    EndDate = endDate,
                    Employee = selectedEmployee,
                    HoursWorked = 0,
                    Status = "Inactive",
                    Description = string.Empty,
                };
  
                db.Repairs.Add(repair);
                db.SaveChanges();
                data.Repairs = new ObservableCollection<RepairOrderModel>(data.Repairs.Append(repair));
                CustomerBox.SelectedItem = null;
                EmployeeBox.SelectedItem = null;
                StartDateBox.SelectedDate = null;
                EndDateBox.SelectedDate = null;
                ResultLabel.Visibility = Visibility.Visible;
                ResultLabel.Foreground = Brushes.Green;
                ResultLabel.Content = "Repair added";
            }
        }

        private void Required()
        {
            ResultLabel.Content = "* = required";
            ResultLabel.Foreground = Brushes.Red;
            ResultLabel.Visibility = Visibility.Visible;
        }

        private void StartDateBox_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!(StartDateBox.SelectedDate == null))
            {
                DateTime selectedDate = (DateTime)StartDateBox.SelectedDate;
                EndDateBox.BlackoutDates.Clear();
                EndDateBox.BlackoutDates.AddDatesInPast();
                EndDateBox.BlackoutDates.Add(new CalendarDateRange(DateTime.Now, selectedDate));
                EndDateBox.SelectedDate = null;
                SelectStartDateLabel.Foreground = Brushes.Black;
                SelectStartDateLabel.Content = "Start date:";
            }

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string name = comboBox.Name.Remove(comboBox.Name.Length - 3);

            switch (name)
            {
                case "Customer":
                    SelectCustomerLabel.Content = $"{name}:";
                    SelectCustomerLabel.Foreground = Brushes.Black;  
                    break;
                case "Employee":
                    SelectEmployeeLabel.Content = $"{name}:";
                    SelectEmployeeLabel.Foreground = Brushes.Black;
                    break;
            }
        }

        private void EndDateBox_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(EndDateBox.SelectedDate == null))
            {
                SelectEndDateLabel.Foreground = Brushes.Black;
                SelectEndDateLabel.Content = "End date:";
            }
            
        }
    }
}
