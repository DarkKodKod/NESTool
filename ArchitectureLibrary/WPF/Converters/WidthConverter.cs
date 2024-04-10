using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace ArchitectureLibrary.WPF.Converters;

public class WidthConverter : IValueConverter
{
    public object Convert(object o, Type type, object parameter, CultureInfo culture)
    {
        ListView? l = o as ListView;

        if (l?.View is GridView g)
        {
            double total = 0;

            for (int i = 0; i < g.Columns.Count - 1; i++)
            {
                total += g.Columns[i].Width;
            }

            return (l.ActualWidth - total);
        }

        return 0;
    }

    public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
