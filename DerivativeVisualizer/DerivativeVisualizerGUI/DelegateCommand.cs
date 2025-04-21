using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DerivativeVisualizerGUI
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<Object?> _execute; // The lambda expression that performs the action
        private readonly Func<Object?, Boolean>? _canExecute; // The lambda expression that checks the action's condition

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="execute">The action to be executed.</param>
        public DelegateCommand(Action<Object?> execute) : this(null, execute) { }

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="canExecute">The condition that determines if the action can be executed.</param>
        /// <param name="execute">The action to be executed.</param>
        public DelegateCommand(Func<Object?, Boolean>? canExecute, Action<Object?> execute)
        {
            ArgumentNullException.ThrowIfNull(execute);

            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Event for changes in executability.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Checks if the action can be executed.
        /// </summary>
        /// <param name="parameter">The parameter for the action.</param>
        /// <returns>True if the action can be executed.</returns>
        public Boolean CanExecute(Object? parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="parameter">The parameter for the action.</param>
        public void Execute(Object? parameter)
        {
            if (!CanExecute(parameter))
            {
                throw new InvalidOperationException("Command execution is disabled.");
            }
            _execute(parameter);
        }

        /// <summary>
        /// Raises the event for changes in executability.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}
