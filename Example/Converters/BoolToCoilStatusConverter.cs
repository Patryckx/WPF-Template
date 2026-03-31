using Example.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Example.Converters
{
    public class BoolToCoilStatusConverter :IValueConverter
    {
        public object Convert(object value,Type targetType, object parameter, System.Globalization.CultureInfo culture)

        {
            if(value is bool BoolValue)
                return BoolValue ? CoilStatus.Enabled : CoilStatus.Disabled;

            return CoilStatus.Enabled;
        
    }

    public object ConvertBack(object value,Type targetType,object parameter,
        System.Globalization.CultureInfo culture)
            =>
            throw new NotImplementedException(); 
    }


}

