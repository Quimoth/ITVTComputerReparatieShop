using ComputerReparatieShop.DAL;
using ComputerReparatieShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables
        List<string> status = new List<string> { "Waiting for Parts", "In progress", "Inactive", "Fixed", "Waiting for employee" };
        List<Brush> statusColors = new List<Brush> { Brushes.SandyBrown, Brushes.NavajoWhite, Brushes.LightGray, Brushes.LightGreen, Brushes.LightCoral };

        MyContext db = new MyContext();

        List<CustomerModel> customers;

        List<PartModel> parts;

        List<EmployeeModel> employees;

        List<RepairOrderModel> repairOrders;
        #endregion
        public MainWindow()
        {
            GetAllData();
            InitializeComponent();

            //Displays the amount of repairs for each status category. Note to self: does not refresh (yet)?
            UpdateStatusBar();

            //Handles the binding and styles of the cells in the orderlist.
            FillOrderList();
        }

        /// <summary>
        /// Binds the repair order data to the datagrid in the app and handles which columns are shown/hidden and adds the visual styles and templates.
        /// </summary>
        private void FillOrderList()
        {
            OrderListGrid.ItemsSource = repairOrders;
            OrderListGrid.AutoGenerateColumns = false;

            //The list of collumns we wish to display. 
            //TODO: parts used and total cost column? latter will need special binding
            string[] columns = { "Customer", "Status", "Employee", "StartDate", "EndDate", "HoursWorked", "Description" };
            foreach (string columnName in columns)
            {
                DataGridTextColumn data = new DataGridTextColumn();

                if (columnName.Contains("Date"))
                {
                    Binding binding = new Binding(columnName);
                    binding.StringFormat = "dd/MM/yyyy";

                    string type = "End";
                    if (columnName.Contains("Start"))
                    {
                        type = "Start";
                    }
                    DataTemplate template = new DataTemplate();
                    if (TryFindResource($"{type}DateTemplate") != null)
                    {
                        object obj = TryFindResource($"{type}DateTemplate");
                        template = obj as DataTemplate;
                        TextBlock textBlock = template.LoadContent() as TextBlock;

                        object temp = TryFindResource($"{type}DateStyle");

                        //Set the textblock style to the correct Style in the xaml file.
                        textBlock.Style = temp as Style;
                    }

                    DataGridTemplateColumn dataTemplateColumn = new DataGridTemplateColumn
                    {
                        Header = columnName,
                        CellTemplate = (template != null) ? template : template,
                    };
                    OrderListGrid.Columns.Add(dataTemplateColumn);
                    continue;
                }
                else if (columnName == "Status")
                {
                    Binding binding = new Binding(columnName);

                    DataTemplate template = new DataTemplate();
                    if (TryFindResource("StatusTemplate") != null)
                    {
                        object obj = TryFindResource("StatusTemplate");
                        template = obj as DataTemplate;
                        TextBlock textBlock = template.LoadContent() as TextBlock;

                        object temp = TryFindResource("StatusBlockStyle");

                        //The styles handle the colors.
                        textBlock.Style = temp as Style;
                    }

                    DataGridTemplateColumn dataTemplateColumn = new DataGridTemplateColumn
                    {
                        Header = columnName,
                        CellTemplate = template,
                    };

                    OrderListGrid.Columns.Add(dataTemplateColumn);
                    continue;
                }
                else
                {
                    data = new DataGridTextColumn
                    {
                        Header = columnName,
                        Binding = new Binding(columnName),
                    };
                }

                OrderListGrid.Columns.Add(data);
            }
        }

        #region StatusBar
        /// <summary>
        /// Updates the content of the status bar.
        /// </summary>
        public void UpdateStatusBar()
        {
            int col = 0;
            for (int i = 0; i < status.Count; i++)
            {
                string s = status[i];
                Brush brush = statusColors[i];
                Label headerLabel = new Label()
                {
                    Content = s,
                    Background = brush,
                    BorderThickness = new Thickness(1, 0, 0, 0),
                    BorderBrush = Brushes.Black,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                };

                OrderStatusGrid.Children.Add(headerLabel);
                Grid.SetRow(headerLabel, 0);
                Grid.SetColumn(headerLabel, col);

                List<RepairOrderModel> repairsOfType = repairOrders.Where(x => x.Status == s).ToList();

                Label countLabel = new Label()
                {
                    Content = repairsOfType.Count,
                    Background = brush,
                    BorderThickness = new Thickness(1, 0, 0, 0),
                    BorderBrush = Brushes.Black,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                };
                Grid.SetRow(countLabel, 1);
                Grid.SetColumn(countLabel, col);
                OrderStatusGrid.Children.Add(countLabel);
                col++;
            }
        }
        #endregion

        public void Name_Text_Changed(object sender, TextChangedEventArgs e)
        {
            string changes = e.Changes.ToString();

            TextBox textBox = sender as TextBox;
            string text = textBox.Text;

            char capital = text[0];
            capital = (capital > 96) ? (char)(capital - 32) : capital;
            text = text.Remove(0, 1);
            text = text.Insert(0, $"{capital}");
        }

        #region Buttons
        private void Add_Customer(object sender, RoutedEventArgs e)
        {
            CreateFrame.Source = new Uri("CreateCustomerPage.xaml", UriKind.RelativeOrAbsolute);
        }

        private void Add_Employee(object sender, RoutedEventArgs e)
        {
            CreateFrame.Source = new Uri("CreateEmployeePage.xaml", UriKind.RelativeOrAbsolute);
        }

        private void Add_RepairOrder(object sender, RoutedEventArgs e)
        {
            CreateFrame.Source = new Uri("CreateRepairOrderPage.xaml", UriKind.RelativeOrAbsolute);
        }

        private void Add_Part(object sender, RoutedEventArgs e)
        {
            CreateFrame.Source = new Uri("CreatePartPage.xaml", UriKind.RelativeOrAbsolute);
        }
        #endregion

        private void GetAllData()
        {
            customers = db.Customers.ToList();
            parts = db.Parts.ToList();
            employees = db.Employees.ToList();
            repairOrders = db.Repairs.Include("Customer").Include("Employee").Include("PartsUsed").ToList();
        }
    }
}
