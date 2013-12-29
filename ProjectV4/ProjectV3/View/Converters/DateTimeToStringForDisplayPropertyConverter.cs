using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ProjectV3.View.Converters
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    class DateTimeToStringForDisplayPropertyConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Console.WriteLine("Start converteren DatetimeToString");
            //month/day/years
            DateTime d = (DateTime)value;
            int day = d.Day;
            int month = d.Month;
            int year = d.Year;
            string sday = month + "/" + day + "/" + year;

            return sday;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
