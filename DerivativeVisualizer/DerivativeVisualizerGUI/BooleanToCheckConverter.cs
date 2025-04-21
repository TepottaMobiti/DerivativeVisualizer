using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DerivativeVisualizerGUI
{
    // Entire class is AI generated.
    public class BooleanToCheckConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a checkmark or cross symbol. 
        /// Returns "✔" if the value is true, otherwise returns "✖".
        /// </summary>
        /// <param name="value">The input value expected to be a boolean.</param>
        /// <param name="targetType">The target type of the binding (unused).</param>
        /// <param name="parameter">An optional parameter (unused).</param>
        /// <param name="culture">The culture information (unused).</param>
        /// <returns>
        /// A string containing "✔" if the boolean is true; otherwise, "✖".
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool isValid && isValid) ? "✔" : "✖";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
