using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ComputerReparatieShop
{
    /// <summary>
    /// A simple class that allows to check wether a certain DateTime is in the future or not.
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(bool))]
    public class IsEqualOrGreaterThanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            DateTime curDate = DateTime.Now;

            TimeSpan span = curDate.Subtract(date);

            int param = int.Parse(parameter.ToString());

            return span.TotalDays > 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
