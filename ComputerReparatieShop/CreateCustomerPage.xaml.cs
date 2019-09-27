using ComputerReparatieShop.DAL;
using ComputerReparatieShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for CreateCustomerPage.xaml
    /// </summary>
    public partial class CreateCustomerPage : Page
    {
        MyContext db = MyContext.Create();
        public CreateCustomerPage()
        {
            InitializeComponent();
        }

        private void CreateCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            AddCustomer();
        }

        private void AddCustomer()
        {
            List<string> fields = new List<string>{"FirstName","LastName","City","Country","ZipCode","Adress","Preposition" };
            List<TextBox> inputFields = new List<TextBox>();
            //List<Label> inputLabels = new List<Label>();

            bool requiredFilled = true;

            foreach(string field in fields)
            {
                TextBox textBox = FindName($"{field}Box") as TextBox;
                Label label = FindName($"{field}Label") as Label;
                //Adds the textBox to the inputFields list for future access.
                inputFields.Add(textBox);
                //inputLabels.Add(label);

                //If the TextBox to the given field is empty, do something.
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    //Preposition isn't required
                    if (field != "Preposition")
                    {
                        requiredFilled = false;
                        if (!label.Content.ToString().Contains('*'))
                        { 
                            label.Content += "*";
                        }
                        label.Foreground = Brushes.Red;
                    }
                }
            }

            //If all required fields have been filled in (all except preposition are required)
            if (requiredFilled)
            {
                db.Customers.Add(new CustomerModel
                {
                    FirstName = inputFields[0].Text,
                    Preposition = inputFields[6].Text,
                    LastName = inputFields[1].Text,
                    Adress = inputFields[5].Text,
                    City = inputFields[2].Text,
                    Country = inputFields[3].Text,
                    ZipCode = inputFields[4].Text,
                });

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Should make sure that Name fields start with a capital and do not contain numbers. e.g. City, Country, FirstName, LastName and Preposition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Name_Text_Changed(object sender, TextChangedEventArgs e)
        {     
            TextBox textBox = sender as TextBox;
            string text = textBox.Text;

            if (!string.IsNullOrEmpty(text))
            {
                text = text.ToLower();
                char capital = text[0];
                capital -= (char)32;
                text = text.Remove(0, 1);
                text = text.Insert(0, $"{capital}");
                int selectionStart = textBox.SelectionStart;
                textBox.Text = text;
                textBox.SelectionStart = selectionStart;
            }
        }

        private void NameBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[a-zA-Z]");

            e.Handled = !regex.IsMatch(e.Text);
        }

        private void NameBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if(e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste || e.Command == ApplicationCommands.Copy)
            {
                e.Handled = true;
            }
        }
    }
}
