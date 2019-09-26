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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables
        readonly List<string> status = new List<string> { "Waiting for Parts", "In progress", "Inactive", "Fixed", "Waiting for employee" };
        readonly List<Brush> statusColors = new List<Brush> { Brushes.SandyBrown, Brushes.NavajoWhite, Brushes.LightGray, Brushes.LightGreen, Brushes.LightCoral };

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

            data.PropertyChanged += Refresh;
        }

        #region DataGrid Initialization
        /// <summary>
        /// Binds the repair order data to the datagrid in the app and handles which columns are shown/hidden and adds the visual styles and templates.
        /// </summary>
        private void FillOrderList(ObservableCollection<RepairOrderModel> repairs)
        {
            OrderListGrid.ItemsSource = repairs;
            OrderListGrid.AutoGenerateColumns = false;

            //The list of collumns we wish to display. The order in this list will be the column order the user sees.
            string[] columns = { "Customer", "Status", "Employee", "StartDate", "EndDate", "HoursWorked", "Description" };

            //TODO: parts used and total cost column?
            foreach (string columnName in columns)
            {
                // Start and End Date Columns
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

                    OrderListGrid.Columns.Add(new DataGridTemplateColumn
                    {
                        Header = columnName,
                        CanUserSort = true,
                        SortMemberPath = $"{type}Date",
                        CellTemplate = template,
                    });
                }
                else if (columnName == "Status")
                {
                    Binding binding = new Binding(columnName);

                    Style style = TryFindResource("StatusBlockStyle") as Style;
                    Style addEvent = new Style(typeof(ComboBox));
                    addEvent.Setters.Add(new EventSetter(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler(EditStatus)));

                    OrderListGrid.Columns.Add(new DataGridComboBoxColumn
                    {
                        Header = columnName,
                        ItemsSource = status,
                        SelectedItemBinding = binding,
                        SelectedValueBinding = binding,
                        EditingElementStyle = addEvent,
                        CellStyle = style,
                    });
                }
                else if (columnName == "Employee")
                {
                    Binding binding = new Binding(columnName);
                    Style addEvent = new Style(typeof(ComboBox));
                    addEvent.Setters.Add(new EventSetter(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler(EditEmployee)));

                    OrderListGrid.Columns.Add(new DataGridComboBoxColumn
                    {
                        Header = columnName,
                        ItemsSource = data.Employees,
                        SelectedItemBinding = binding,
                        SelectedValueBinding = binding,
                        EditingElementStyle = addEvent,
                    });
                }
                else if (columnName == "Customer")
                {
                    Binding binding = new Binding(columnName);

                    Style addEvent = new Style(typeof(ComboBox));
                    addEvent.Setters.Add(new EventSetter(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler(EditCustomer)));

                    OrderListGrid.Columns.Add(new DataGridComboBoxColumn
                    {
                        Header = columnName,
                        ItemsSource = data.Customers,
                        SelectedItemBinding = binding,
                        SelectedValueBinding = binding,
                        EditingElementStyle = addEvent,
                    });
                }
                else if (columnName == "HoursWorked")
                {
                    Binding binding = new Binding(columnName);

                    Style addEvent = new Style(typeof(TextBox));
                    addEvent.Setters.Add(new EventSetter(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(IntegerValidation)));

                    OrderListGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = columnName,
                        Binding = binding,
                        EditingElementStyle = addEvent,
                    });
                }
                else if (columnName == "Description")
                {
                    Binding binding = new Binding(columnName);

                    Style addEvent = new Style(typeof(DataGridCell));

                    addEvent.Setters.Add(new EventSetter(DataGridCell.LostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(DescriptionChanged)));

                    OrderListGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = columnName,
                        Binding = binding,
                        CellStyle = addEvent,
                    });
                }
                //The default column, simply binds the text to a datagridtextcolumn, redundant.
                else
                {
                    OrderListGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = columnName,
                        Binding = new Binding(columnName),
                    });
                }
            }
        }

        #endregion

        #region DataGrid edit and validation

        #region ComboBox Events
        /// <summary>
        /// Eventhandler for changes of the employee of a repair (ComboBox)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditEmployee(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            RepairOrderModel repair = comboBox.DataContext as RepairOrderModel;

            repair.Employee = comboBox.SelectedItem as EmployeeModel;

            db.Entry(repair).State = EntityState.Modified;
            db.SaveChanges();
        }

        /// <summary>
        /// Eventhandler for changes of the customer of a repair (ComboBox)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditCustomer(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            RepairOrderModel repair = comboBox.DataContext as RepairOrderModel;

            repair.Customer = comboBox.SelectedItem as CustomerModel;

            db.Entry(repair).State = EntityState.Modified;
            db.SaveChanges();
        }

        /// <summary>
        /// Eventhandler for changes of the status of a repair (ComboBox)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditStatus(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            RepairOrderModel repair = comboBox.DataContext as RepairOrderModel;
            string selectedStatus = (string)comboBox.SelectedItem;
            List<string> allowedStatus = new List<string>(status);

            if(repair.Employee == null)
            {
                allowedStatus.Remove("In progress");
            }
            else
            {
                allowedStatus.Remove("Inactive");
                allowedStatus.Remove("Waiting for employee");
            }

            if (allowedStatus.Contains(selectedStatus) && repair.Status != selectedStatus)
            {
                repair.Status = selectedStatus;

                db.Entry(repair).State = EntityState.Modified;
                db.SaveChanges();
                Update();
            }
        }

        /// <summary>
        /// Updates the description if it has been changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DescriptionChanged(object sender, RoutedEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;

            TextBox textBox = cell.Content as TextBox;

            if (textBox != null)
            {
                RepairOrderModel repair = cell.DataContext as RepairOrderModel;
                repair.Description = textBox.Text;

                db.Entry(repair).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        #endregion

        #region DatePicker eventhandlers
        /// <summary>
        /// Adds blackout dates to the calendar object of the datepicker.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Updates the date the use has changed and saves it on the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateDate(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            if (datePicker.IsDropDownOpen)
            {
                RepairOrderModel order = datePicker.DataContext as RepairOrderModel;

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
                    if (selected > today)
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

        #region TextBox Validation

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

        private void IntegerValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsInt(e.Text);
        }
        #endregion

        #region helper methods
        /// <summary>
        /// returns wether a given string is an Integer (of any size)
        /// </summary>
        /// <param name="text">the string to parse</param>
        /// <returns></returns>
        private bool IsInt(string text)
        {
            Regex regex = new Regex("[0-9]");
            return regex.IsMatch(text);
        }
        #endregion

        #endregion

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

        #region Create Buttons
        /* 
         * These buttons open the different create pages in the frame object of the main window grid.
         */
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

        #region update/refresh methods
        /// <summary>
        /// Calls the Update() method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Refresh(object sender, PropertyChangedEventArgs e)
        {
            Update();
        }

        /// <summary>
        /// Updates the DataGrid on the front end when a change has been made.
        /// </summary>
        private void Update()
        {
            ObservableCollection<RepairOrderModel> repairs = data.Repairs;
            OrderListGrid.Items.Refresh();
            OrderListGrid.ItemsSource = repairs;
            UpdateStatusBar(repairs);
        }
        #endregion
    }
}
