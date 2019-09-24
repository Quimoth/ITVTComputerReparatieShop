using ComputerReparatieShop.DAL;
using ComputerReparatieShop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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

        public static Data data = new Data();

        MyContext db = MyContext.Create();

        #endregion
        public MainWindow()
        {
            InitializeComponent();

            ObservableCollection<RepairOrderModel> repairs = data.Repairs;

            //Displays the amount of repairs for each status category. Note to self: does not refresh (yet)?
            UpdateStatusBar(repairs);

            //Handles the binding and styles of the cells in the orderlist.
            FillOrderList(repairs);

            OrderListGrid.CellEditEnding += UpdateEntry;
            data.PropertyChanged += Refresh;
        }

        private void UpdateEntry(object sender, DataGridCellEditEndingEventArgs e)
        {
            RepairOrderModel editRepair = e.Row.DataContext as RepairOrderModel;
            RepairOrderModel repairToEdit = db.Repairs.Where(x => x.RepairOrderId == editRepair.RepairOrderId).SingleOrDefault();

            switch (e.Column.DisplayIndex)
            {
                case 0:
                    repairToEdit.Customer = editRepair.Customer;
                    break;
            }
            
            db.Entry(repairToEdit).State = EntityState.Modified;
            db.SaveChanges();
        }

        private void Refresh(object sender, PropertyChangedEventArgs e)
        {
            Update();
        }

        private void Update()
        {
            ObservableCollection<RepairOrderModel> repairs = data.Repairs;
            OrderListGrid.Items.Refresh();
            OrderListGrid.ItemsSource = repairs;
            UpdateStatusBar(repairs);
        }

        /// <summary>
        /// Binds the repair order data to the datagrid in the app and handles which columns are shown/hidden and adds the visual styles and templates.
        /// </summary>
        private void FillOrderList(ObservableCollection<RepairOrderModel> repairs)
        {
            OrderListGrid.ItemsSource = repairs;
            OrderListGrid.AutoGenerateColumns = false;

            //The list of collumns we wish to display. The order in this list will be the column order the user sees.
            string[] columns = { "Customer", "Status", "Employee", "StartDate", "EndDate", "HoursWorked", "Description" };

            //TODO: parts used and total cost column? latter will need special binding
            foreach (string columnName in columns)
            {
                DataGridTextColumn dataGridColumn = new DataGridTextColumn();

                // Date columns
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
                        
                        DatePicker datePicker = template.LoadContent() as DatePicker;

                        object temp = TryFindResource($"{type}DateStyle");

                        //Set the textblock style to the correct Style in the xaml file.
                        datePicker.Style = temp as Style;
                        datePicker.SelectedDateChanged += UpdateDate;
                        datePicker.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Now));
                    }

                    DataGridTemplateColumn dataGridTemplateColumn = new DataGridTemplateColumn
                    {
                        Header = columnName,
                        CanUserSort = true,
                        SortMemberPath = $"{type}Date",
                        CellTemplate = template,
                    };
                    OrderListGrid.Columns.Add(dataGridTemplateColumn);
                    continue;
                }
                else if (columnName == "Status")
                {
                    Binding binding = new Binding(columnName);

                    DataTemplate template = new DataTemplate();

                    template = TryFindResource("StatusTemplate") as DataTemplate;
                        
                    TextBlock textBlock = template.LoadContent() as TextBlock;

                    object temp = TryFindResource("StatusBlockStyle");

                    //The styles handle the colors.
                    textBlock.Style = temp as Style;

                    DataGridTemplateColumn dataGridTemplateColumn = new DataGridTemplateColumn
                    {
                        Header = columnName,
                        CellTemplate = template,
                    };

                    OrderListGrid.Columns.Add(dataGridTemplateColumn);
                    continue;
                }
                else if(columnName == "Employee")
                {
                    Binding binding = new Binding(columnName);

                    DataGridComboBoxColumn dataGridComboBoxColumn = new DataGridComboBoxColumn
                    {
                        Header = columnName,
                        ItemsSource = data.Employees,
                        SelectedItemBinding = binding,
                    };

                    Style addEvent = new Style(typeof(ComboBox));
                    addEvent.Setters.Add(new EventSetter(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler(EditEmployee)));

                    dataGridComboBoxColumn.EditingElementStyle = addEvent;

                    OrderListGrid.Columns.Add(dataGridComboBoxColumn);
                    continue;
                }
                else if(columnName == "Customer")
                {
                    Binding binding = new Binding(columnName);

                    DataGridComboBoxColumn dataGridComboBoxColumn = new DataGridComboBoxColumn
                    {
                        Header = columnName,
                        ItemsSource = data.Customers,
                        SelectedItemBinding = binding,
                        SelectedValueBinding = binding,
                    };

                    Style addEvent = new Style(typeof(ComboBox));
                    addEvent.Setters.Add(new EventSetter(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler(EditCustomer)));

                    dataGridComboBoxColumn.EditingElementStyle = addEvent;

                    OrderListGrid.Columns.Add(dataGridComboBoxColumn);
                    continue;
                }
                else
                {
                    dataGridColumn = new DataGridTextColumn
                    {
                        Header = columnName,
                        Binding = new Binding(columnName),
                    };
                }


                OrderListGrid.Columns.Add(dataGridColumn);
            }
        }

        private void EditEmployee(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            RepairOrderModel repair = comboBox.DataContext as RepairOrderModel;

            repair.Employee = comboBox.SelectedItem as EmployeeModel;

            db.Entry(repair).State = EntityState.Modified;
            db.SaveChanges();
        }

        private void EditCustomer(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            RepairOrderModel repair = comboBox.DataContext as RepairOrderModel;

            repair.Customer = comboBox.SelectedItem as CustomerModel;

            db.Entry(repair).State = EntityState.Modified;
            db.SaveChanges();
        }

        #region StatusBar
        /// <summary>
        /// Updates the content of the status bar.
        /// </summary>
        public void UpdateStatusBar(ObservableCollection<RepairOrderModel> repairs)
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

                

                List<RepairOrderModel> repairsOfType = repairs.Where(x => x.Status == s).ToList();

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

        #region DatePicker eventhandlers
        private void DatePicker_Open(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            RepairOrderModel repair = datePicker.DataContext as RepairOrderModel;
            //Makes sure the start date can't be changed to anything earlier than now or the last start date.
            DateTime currentStartDate = ((DateTime)repair.StartDate).AddDays(-1);
            DateTime maxTime = (currentStartDate > DateTime.Now.AddDays(-1)) ? DateTime.Now.AddDays(-1) : currentStartDate;
            datePicker.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, maxTime));
            if (datePicker.Name.Contains("End"))
            {
                datePicker.BlackoutDates.AddDatesInPast();
            }
        }

        private void UpdateDate(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            if (datePicker.IsDropDownOpen)
            {
                RepairOrderModel selectedOrder = datePicker.DataContext as RepairOrderModel;
                RepairOrderModel order = db.Repairs.Where(x => x.RepairOrderId == selectedOrder.RepairOrderId).SingleOrDefault();

                DateTime? orderStart = order.StartDate;
                DateTime? today = DateTime.Now;
                DateTime? selected = datePicker.SelectedDate;

                string name = datePicker.Name;
                datePicker.IsDropDownOpen = false;

                if (name.Contains("End"))
                {
                    if (selected > orderStart && selected > today)
                    {
                        order.EndDate = selected;
                        db.Entry(order).State = EntityState.Modified;
                        db.SaveChanges();

                        Update();
                    }
                }
                else
                {
                    if(selected > today)
                    {
                        order.StartDate = selected;
                        db.Entry(order).State = EntityState.Modified;
                        db.SaveChanges();

                        Update();
                    }
                }
            }
        }

        #endregion

        #region Validation

        /// <summary>
        /// Prevents false input from being typed into name fields and automatically adds a Capital letter at the begin.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Name_Text_Changed(object sender, TextChangedEventArgs e)
        {
            string changes = e.Changes.ToString();

            TextBox textBox = sender as TextBox;
            string text = textBox.Text;

            char capital = text[0];
            capital = (capital > 96) ? (char)(capital - 32) : capital;
            text = text.Remove(0, 1);
            textBox.Text = text.Insert(0, $"{capital}");
        }

        #endregion
    }
}
