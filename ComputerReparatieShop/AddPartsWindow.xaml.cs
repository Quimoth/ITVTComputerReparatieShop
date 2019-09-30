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
using System.Windows.Shapes;

namespace ComputerReparatieShop
{
    /// <summary>
    /// Interaction logic for AddPartsWindow.xaml
    /// </summary>
    public partial class AddPartsWindow : Window
    {
        public static Data data = new Data();

        MyContext db = MyContext.Create();

        private static RepairOrderModel currentRepair;
        public AddPartsWindow(RepairOrderModel repair)
        {
            InitializeComponent();
            currentRepair = repair;

            FillRepairDetails();
            FillPartList();
        }

        private void FillRepairDetails()
        {
            string[] Rows = { "Customer", "Employee", "Description", "Status","StartDate","EndDate","HoursWorked","PartsUsed" };
            RepairItemsControl.DataContext = currentRepair;

            foreach(string rowName in Rows)
            {
                Binding binding = new Binding(rowName);

                Label label = new Label()
                {
                    Content = rowName,
                    BorderThickness = new Thickness(1, 1, 1, 1),
                    BorderBrush = Brushes.Black,
                    MinHeight = 30,
                };
                LabelItemsControl.Items.Add(label);

                if (rowName == "PartsUsed")
                {
                    ItemsControl itemsControl = new ItemsControl()
                    {
                        ItemsSource = currentRepair.PartsUsed,
                        BorderThickness = new Thickness(1, 1, 1, 1),
                        BorderBrush = Brushes.Black,
                        MinHeight = 30,
                        MaxHeight = 200,
                    };
                    label.Height = itemsControl.Height;
                    RepairItemsControl.Items.Add(itemsControl);

                }
                else
                {
                    Label info = new Label()
                    {
                        BorderThickness = new Thickness(1, 1, 1, 1),
                        BorderBrush = Brushes.Black,
                        MinHeight = 30,
                    };
                    info.SetBinding(Label.ContentProperty, binding);
                    RepairItemsControl.Items.Add(info);
                    
                }
            }
        }

        private void FillPartList()
        {
            PartGrid.ItemsSource = data.Parts;

            string[] columnNames = { "Name", "Price", "InStock", "Manufacturer", "NextDelivery","Ordered" };

            foreach (string columnName in columnNames)
            {
                Binding binding = new Binding(columnName);
                DataGridTextColumn dataGridTextColumn = new DataGridTextColumn()
                {
                    Header = columnName,
                    Binding = binding,
                };
                PartGrid.Columns.Add(dataGridTextColumn);
            }
        }

        private void AddPartsButton_Click(object sender, RoutedEventArgs e)
        {
            List<PartModel> selectedParts = PartGrid.SelectedItems.Cast<PartModel>().ToList();

            currentRepair.PartsUsed.AddRange(selectedParts);
            UpdateRepairList();
        }

        private void UpdateRepairList()
        {
            RepairItemsControl.Items.Clear();
            LabelItemsControl.Items.Clear();
            FillRepairDetails();
        }
    }
}
