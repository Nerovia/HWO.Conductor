using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Conductor.App.Resources
{
    public class ParameterlessCommandHandler : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action _action;

        public ParameterlessCommandHandler(Action action)
        {
            this._action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this._action();
        }
    }

    public class CommandHandler<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<T> _action;

        public CommandHandler(Action<T> action)
        {
            this._action = action;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is null)
                return false;
            if (typeof(T) == parameter.GetType())
                return false;
            return true;
        }

        public void Execute(object parameter)
        {
            this._action((T)parameter);
        }
    }
}
