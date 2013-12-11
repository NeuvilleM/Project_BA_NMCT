using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ProjectV3.View.Converters
{
    class MultiDateToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime start = (DateTime)values[0];
            DateTime end = (DateTime)values[1];
            string date = "";
            date += start.Day + "/" + start.Month + "/" + start.Year;
            date += " - ";
            date += end.Day + "/" + end.Month + "/" + end.Year;
            return date;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
