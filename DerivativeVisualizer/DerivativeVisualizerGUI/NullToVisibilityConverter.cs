using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace DerivativeVisualizerGUI
{
    // Entire class is AI generated.
    public class NullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a null value to <see cref="Visibility.Collapsed"/> and any non-null value to <see cref="Visibility.Visible"/>.
        /// </summary>
        /// <param name="value">The input value to check for null.</param>
        /// <param name="targetType">The target type of the binding (unused).</param>
        /// <param name="parameter">An optional parameter (unused).</param>
        /// <param name="culture">The culture information (unused).</param>
        /// <returns>
        /// <see cref="Visibility.Collapsed"/> if the input value is null; otherwise, <see cref="Visibility.Visible"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
