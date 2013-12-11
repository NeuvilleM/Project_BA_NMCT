using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ProjectV3.View.Converters
{
    [ValueConversion(typeof(string), typeof(Uri))]
    class StringToUriConverter: IValueConverter

    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string svalue = (string)value;
            if (svalue != null && svalue != "")
            {

                return new Uri(svalue, UriKind.RelativeOrAbsolute);
            }
            else
            {
                Uri NewUri = new Uri("Pictures/noImage", UriKind.Relative);
                return NewUri;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
