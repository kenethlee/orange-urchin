using System;
using System.Windows.Input;

namespace Prototype.Utilities
{
    public class RelayCommand : ObservableObject, ICommand
    {
        public RelayCommand(Action action)
            : this(action, null)
        {
        }

        public RelayCommand(Action action, Func<bool> predicate)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _predicate = predicate;
        }

        public RelayCommand(Action<object> action)
            : this(action, null)
        {
        }

        public RelayCommand(Action<object> action, Predicate<object> predicate)
        {
            _parameterizedAction = action ?? throw new ArgumentNullException(nameof(action));
            _parameterizedPredicate = predicate;
        }

        public bool CanExecute(object parameter)
        {
            if (_parameterizedPredicate != null)
                return _parameterizedPredicate(parameter);

            return _predicate == null || _predicate();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;

            if (_parameterizedAction != null)
                _parameterizedAction(parameter);
            else
                _action();
        }

        private readonly Action _action;
        private readonly Func<bool> _predicate;
        private readonly Action<object> _parameterizedAction;
        private readonly Predicate<object> _parameterizedPredicate;
    }
}