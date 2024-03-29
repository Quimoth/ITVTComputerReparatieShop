﻿using ComputerReparatieShop.DAL;
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
    /// Interaction logic for CreatePartPage.xaml
    /// </summary>
    public partial class CreatePartPage : Page
    {
        MyContext db = MyContext.Create();
        public CreatePartPage()
        {
            InitializeComponent();
        }

        private void CreatePartButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO add a validation method
            AddPart();
        }

        public void AddPart()
        {
            string partName = PartNameBox.Text;
            string manufacturer = PartManufacturerBox.Text;
            double price = double.NaN;
            if (double.TryParse(PartPriceBox.Text, out double result))
            {
                price = double.Parse(PartPriceBox.Text); ;
            }
            if(!string.IsNullOrEmpty(partName) && !string.IsNullOrEmpty(manufacturer) && !double.IsNaN(price))
            {
                PartModel part = db.Parts.Where(x => x.Name == partName).FirstOrDefault();
                if(part != null)
                {
                    ResultLabel.Foreground = Brushes.Red;
                    ResultLabel.Content = "Dit onderdeel bestaat al!";
                    return;
                }

                db.Parts.Add(new PartModel { Name = partName, Price = price, Manufacturer = manufacturer });
                db.SaveChanges();
                ResultLabel.Foreground = Brushes.Black;
                ResultLabel.Content = "Onderdeel toegevoegd";
            }
            else
            {

            }
        }

        /// <summary>
        /// returns wether a given string is an Integer (of any size)
        /// </summary>
        /// <param name="text">the string to parse</param>
        /// <returns></returns>
        private bool IsDouble(string text)
        {
            Regex regex = new Regex("^\\d{0,6}(\\.\\d{0,1})?$");
            return regex.IsMatch(text);
        }

        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            e.Handled = !IsDouble(textBox.Text);
        }
    }
}
