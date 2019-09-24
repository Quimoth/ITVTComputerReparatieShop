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
    /// Interaction logic for CreateEmployeePage.xaml
    /// </summary>
    public partial class CreateEmployeePage : Page
    {
        MyContext db = MyContext.Create();
        public CreateEmployeePage()
        {
            InitializeComponent();
        }
        private void CreateEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            AddEmployee();
        }

        private void AddEmployee()
        {
            string firstName = FirstNameBox.Text;
            string preposition = PrepositionBox.Text;
            string lastName = LastNameBox.Text;
            string adress = AdressBox.Text;
            string city = CityBox.Text;
            string country = CountryBox.Text;
            string zipCode = ZipCodeBox.Text;
            //TODO validation and check.
            double fee = double.NaN;
            if (double.TryParse(FeeBox.Text, out double result))
            {
                fee = double.Parse(FeeBox.Text);
            }
            

            db.Employees.Add(new EmployeeModel {
                FirstName = firstName,
                Preposition = preposition,
                LastName = lastName,
                Adress = adress,
                City = city,
                Country = country,
                ZipCode = zipCode,
                Fee = fee,
            });

            db.SaveChanges();
        }
    }
}
