using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DerivativeVisualizerGUI
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Instantiates the base class for the ViewModel.
        /// </summary>
        protected ViewModelBase() { }

        /// <summary>
        /// Event for property change.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Property change notification with validation.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] String? propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
