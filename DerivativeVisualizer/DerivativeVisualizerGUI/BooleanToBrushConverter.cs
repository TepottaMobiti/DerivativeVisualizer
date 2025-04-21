using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DerivativeVisualizerGUI
{
    // Entire class is AI generated.
    public class BooleanToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a <see cref="Brush"/>. 
        /// Returns <see cref="Brushes.LightGreen"/> if true, <see cref="Brushes.LightCoral"/> if false, 
        /// and <see cref="Brushes.White"/> if the value is not a boolean.
        /// </summary>
        /// <param name="value">The input value expected to be a boolean.</param>
        /// <param name="targetType">The target type of the binding (unused).</param>
        /// <param name="parameter">An optional parameter (unused).</param>
        /// <param name="culture">The culture information (unused).</param>
        /// <returns>
        /// A <see cref="Brush"/> corresponding to the boolean value: 
        /// <see cref="Brushes.LightGreen"/> for true, <see cref="Brushes.LightCoral"/> for false, or <see cref="Brushes.White"/> if not a boolean.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? Brushes.LightGreen : Brushes.LightCoral;
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
