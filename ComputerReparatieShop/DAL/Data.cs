using ComputerReparatieShop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerReparatieShop.DAL
{

    public class Data : INotifyPropertyChanged
    {
        public MyContext db = MyContext.Create();

        private ObservableCollection<RepairOrderModel> repairs;

        public Data()
        {
            repairs = new ObservableCollection<RepairOrderModel>(db.Repairs.Include("Customer").Include("Employee").Include("PartsUsed"));
            repairs.CollectionChanged += NotifyFrontEnd;
            Repairs.CollectionChanged += NotifyFrontEnd;
        }

        private void NotifyFrontEnd(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Repairs");
        }

        public ObservableCollection<PartModel> Parts
        {
            get
            {
                return new ObservableCollection<PartModel>(db.Parts);
            }
            set { }
        }
        public ObservableCollection<CustomerModel> Customers
        {
            get
            {
                return new ObservableCollection<CustomerModel>(db.Customers);
            }
            set { }
        }
        public ObservableCollection<EmployeeModel> Employees
        {
            get
            {
                return new ObservableCollection<EmployeeModel>(db.Employees);
            }
            set { }
        }
        public ObservableCollection<RepairOrderModel> Repairs
        {
            get
            {
                repairs.Clear();
                repairs = new ObservableCollection<RepairOrderModel>(db.Repairs.Include("Customer").Include("Employee").Include("PartsUsed"));
                return repairs;
            }
            set
            {
                OnPropertyChanged("Repairs");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
