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
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a <see cref="Visibility"/> value. 
        /// Returns <see cref="Visibility.Visible"/> if the boolean is true, or <see cref="Visibility.Collapsed"/> if false.
        /// If the optional <paramref name="parameter"/> is the string "false" (case-insensitive), the result is inverted.
        /// </summary>
        /// <param name="value">The input value expected to be a boolean.</param>
        /// <param name="targetType">The target type of the binding (unused).</param>
        /// <param name="parameter">An optional parameter to invert the conversion logic when set to "false".</param>
        /// <param name="culture">The culture information (unused).</param>
        /// <returns>
        /// <see cref="Visibility.Visible"/> if the (optionally inverted) boolean is true; otherwise, <see cref="Visibility.Collapsed"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            bool boolValue = false;
            if (value is bool b)
            {
                boolValue = b;
            }

            bool invert = string.Equals(parameter?.ToString(), "false", StringComparison.OrdinalIgnoreCase);
            return (invert ? !boolValue : boolValue) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
