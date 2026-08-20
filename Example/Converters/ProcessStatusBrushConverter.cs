using Example.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Example.Converters
{
    public class ProcessStatusBrushConverter :IValueConverter
    {
        public object Convert (object value, Type targetType,object parameter, CultureInfo culture)
        {
            if (value is not ProcessStatus status)
                return Brushes.Gray;

            return status switch
            {
                ProcessStatus.Idle => Brushes.Orange,
                ProcessStatus.OnProcess => Brushes.DodgerBlue,
                ProcessStatus.Sucess => Brushes.Green,
                ProcessStatus.Failed => Brushes.Red,

                _ => Brushes.Gray
            };
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
}
