// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelayCommand{T}.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tracking.Windows
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// A command whose sole purpose is to 
    ///   relay its functionality to other
    ///   objects by invoking delegates. The
    ///   default return value for the CanExecute
    ///   method is 'true'.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the command
    /// </typeparam>
    public class RelayCommand<T> : ICommand
    {
        #region Fields

        /// <summary>
        ///   Predicate that determines if an T can execute
        /// </summary>
        private readonly Predicate<T> canExecute;

        /// <summary>
        ///   The action to execute when the command is invoked
        /// </summary>
        private readonly Action<T> execute;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand{T}"/> class. 
        ///   Initializes a new instance of the <see cref="RelayCommand"/> class. 
        ///   Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">
        /// The execution logic. 
        /// </param>
        public RelayCommand(Action<T> execute)
        {
            Contract.Requires(execute != null);
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            this.execute = execute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand{T}"/> class. 
        ///   Initializes a new instance of the <see cref="RelayCommand"/> class. 
        ///   Creates a new command.
        /// </summary>
        /// <param name="execute">
        /// The execution logic. 
        /// </param>
        /// <param name="canExecute">
        /// The execution status logic. 
        /// </param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            Contract.Requires(execute != null);
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            Contract.Requires(canExecute != null);
            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion

        #region Public Events

        /// <summary>
        ///   The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines if the command can execute
        /// </summary>
        /// <param name="parameter">
        /// The parameter. 
        /// </param>
        /// <returns>
        /// true if the command can execute, false if not 
        /// </returns>
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (this.canExecute == null)
            {
                return true;
            }

            if (parameter == null
                && typeof(T).IsValueType)
            {
                return this.canExecute(default(T));
            }

            return this.canExecute((T)parameter);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter. 
        /// </param>
        public void Execute(object parameter)
        {
            var value = parameter;

            if (parameter != null
                && parameter.GetType() != typeof(T))
            {
                if (parameter is IConvertible)
                {
                    value = Convert.ChangeType(parameter, typeof(T), null);
                }
            }

            if (this.CanExecute(value))
            {
                if (value == null
                    && typeof(T).IsValueType)
                {
                    this.execute(default(T));
                }
                else
                {
                    this.execute((T)value);
                }
            }
        }

        #endregion
    }
}