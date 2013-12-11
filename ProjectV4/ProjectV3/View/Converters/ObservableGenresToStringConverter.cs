using ProjectV3.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ProjectV3.View.Converters
{
    [ValueConversion(typeof(Object), typeof(string))]
    class ObservableGenresToStringConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string genres = "";
            ObservableCollection<Genre> ocG = value as ObservableCollection<Genre>;
            foreach(Genre g in ocG)
            {
                genres += g.Name + ", ";
            }
            return genres;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
