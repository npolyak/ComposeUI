using Avalonia.Data.Converters;
using Sebastion.Core.SvcUtils;
//using Sebastion.Core.SvcUtils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sebastion.Visuals.Converters
{
    public class AppStatusConverter<T> : IValueConverter
    {
        public T GoodVal { get; set; } = default!;
        public T WarningVal { get; set; } = default!;
        public T ErrorVal { get; set; } = default!;

        public T UnavailableVal { get; set; } = default!;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is AppStatus val)
            {
                return val switch
                {
                    AppStatus.Good => GoodVal,
                    AppStatus.Warning => WarningVal,
                    AppStatus.Error => ErrorVal,
                    _ => UnavailableVal
                };
            }

            return UnavailableVal;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
