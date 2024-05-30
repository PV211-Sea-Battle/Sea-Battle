﻿using System.CodeDom.Compiler;
using System.Windows.Input;

namespace Sea_Battle.Infrastructure
{
    delegate bool Predicate();
    internal class RelayCommand<T1> : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        private readonly Action<T1>? execute;
        private readonly Predicate<T1>? canExecute;
        private readonly Action? _execute;
        private readonly Predicate? _canExecute;
        public RelayCommand(Action<T1> execute, Predicate<T1> canExecute)
            => (this.execute, this.canExecute) = (execute, canExecute);
        public RelayCommand(Action<T1> execute, Predicate? _canExecute = null)
            => (this.execute, this._canExecute) = (execute, _canExecute);
        public RelayCommand(Action _execute, Predicate<T1> canExecute)
            => (this._execute, this.canExecute) = (_execute, canExecute);
        public RelayCommand(Action _execute, Predicate? _canExecute = null)
            => (this._execute, this._canExecute) = (_execute, _canExecute);
        public bool CanExecute(object? parameter)
        {
            if (canExecute is null)
            {
                if (_canExecute is null)
                {
                    return true;
                }

                return _canExecute();
            }

            return canExecute((T1)parameter!);
        }
        public void Execute(object? parameter)
        {
            if (execute is null)
            {
                _execute?.Invoke();
            }

            execute?.Invoke((T1)parameter!);
        }
    }
    internal class RelayCommand(Action execute, Predicate? canExecute = null) 
        : RelayCommand<object?>(new Action(() => execute()),
            canExecute is null ? null : new Predicate(() => canExecute()));
}