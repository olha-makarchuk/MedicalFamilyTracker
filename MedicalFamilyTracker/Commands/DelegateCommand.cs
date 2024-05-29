using System.Windows.Input;

namespace MedicalFamilyTracker.Commands
{
	public class DelegateCommand : ICommand
	{
		#region Constructors
		public DelegateCommand(Action executeMethod)
			: this(executeMethod, null, false)
		{
		}

		public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
			: this(executeMethod, canExecuteMethod, false)
		{
		}

		public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod, bool isAutomaticRequeryDisabled)
		{
			_executeMethod = executeMethod ?? throw new ArgumentNullException("executeMethod");
			_canExecuteMethod = canExecuteMethod;
			_isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
		}
		#endregion

		#region Public Methods
		public bool CanExecute()
		{
			if (_canExecuteMethod != null)
			{
				return _canExecuteMethod();
			}
			return true;
		}

		public void Execute()
		{
			_executeMethod?.Invoke();
		}

		public bool IsAutomaticRequeryDisabled
		{
			get
			{
				return _isAutomaticRequeryDisabled;
			}
			set
			{
				if (_isAutomaticRequeryDisabled != value)
				{
					if (value)
					{
						CommandManagerHelper.RemoveHandlersFromRequerySuggested(_canExecuteChangedHandlers);
					}
					else
					{
						CommandManagerHelper.AddHandlersToRequerySuggested(_canExecuteChangedHandlers);
					}
					_isAutomaticRequeryDisabled = value;
				}
			}
		}

		public void RaiseCanExecuteChanged()
		{
			OnCanExecuteChanged();
		}

		protected virtual void OnCanExecuteChanged()
		{
			CommandManagerHelper.CallWeakReferenceHandlers(_canExecuteChangedHandlers);
		}
		#endregion

		#region ICommand Members
		public event EventHandler CanExecuteChanged
		{
			add
			{
				if (!_isAutomaticRequeryDisabled)
				{
					CommandManager.RequerySuggested += value;
				}
				CommandManagerHelper.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value, 2);
			}
			remove
			{
				if (!_isAutomaticRequeryDisabled)
				{
					CommandManager.RequerySuggested -= value;
				}
				CommandManagerHelper.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value);
			}
		}

		bool ICommand.CanExecute(object parameter)
		{
			return CanExecute();
		}

		void ICommand.Execute(object parameter)
		{
			Execute();
		}
		#endregion

		#region Data

		private readonly Action _executeMethod = null;
		private readonly Func<bool> _canExecuteMethod = null;
		private bool _isAutomaticRequeryDisabled = false;
		private List<WeakReference> _canExecuteChangedHandlers;

		#endregion
	}

	public class DelegateCommand<T> : ICommand
	{
		#region Constructors

		public DelegateCommand(Action<T> executeMethod)
			: this(executeMethod, null, false)
		{
		}

		public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
			: this(executeMethod, canExecuteMethod, false)
		{
		}

		public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod, bool isAutomaticRequeryDisabled)
		{
			_executeMethod = executeMethod ?? throw new ArgumentNullException("executeMethod");
			_canExecuteMethod = canExecuteMethod;
			_isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
		}

		#endregion

		#region Public Methods

		public bool CanExecute(T parameter)
		{
			if (_canExecuteMethod != null)
			{
				return _canExecuteMethod(parameter);
			}
			return true;
		}

		public void Execute(T parameter)
		{
			if (_executeMethod != null)
			{
				_executeMethod(parameter);
			}
		}

		public void RaiseCanExecuteChanged()
		{
			OnCanExecuteChanged();
		}

		protected virtual void OnCanExecuteChanged()
		{
			CommandManagerHelper.CallWeakReferenceHandlers(_canExecuteChangedHandlers);
		}

		public bool IsAutomaticRequeryDisabled
		{
			get
			{
				return _isAutomaticRequeryDisabled;
			}
			set
			{
				if (_isAutomaticRequeryDisabled != value)
				{
					if (value)
					{
						CommandManagerHelper.RemoveHandlersFromRequerySuggested(_canExecuteChangedHandlers);
					}
					else
					{
						CommandManagerHelper.AddHandlersToRequerySuggested(_canExecuteChangedHandlers);
					}
					_isAutomaticRequeryDisabled = value;
				}
			}
		}

		#endregion

		#region ICommand Members

		public event EventHandler CanExecuteChanged
		{
			add
			{
				if (!_isAutomaticRequeryDisabled)
				{
					CommandManager.RequerySuggested += value;
				}
				CommandManagerHelper.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value, 2);
			}
			remove
			{
				if (!_isAutomaticRequeryDisabled)
				{
					CommandManager.RequerySuggested -= value;
				}
				CommandManagerHelper.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value);
			}
		}

		bool ICommand.CanExecute(object parameter)
		{
			// if T is of value type and the parameter is not
			// set yet, then return false if CanExecute delegate
			// exists, else return true
			if (parameter == null &&
				typeof(T).IsValueType)
			{
				return (_canExecuteMethod == null);
			}
			return CanExecute((T)parameter);
		}

		void ICommand.Execute(object parameter)
		{
			Execute((T)parameter);
		}

		#endregion

		#region Data

		private readonly Action<T> _executeMethod = null;
		private readonly Func<T, bool> _canExecuteMethod = null;
		private bool _isAutomaticRequeryDisabled = false;
		private List<WeakReference> _canExecuteChangedHandlers;

		#endregion
	}

	internal class CommandManagerHelper
	{
		internal static void CallWeakReferenceHandlers(List<WeakReference> handlers)
		{
			if (handlers != null)
			{
				// Take a snapshot of the handlers before we call out to them since the handlers
				// could cause the array to me modified while we are reading it.

				EventHandler[] callees = new EventHandler[handlers.Count];
				int count = 0;

				for (int i = handlers.Count - 1; i >= 0; i--)
				{
					WeakReference reference = handlers[i];
					if (reference.Target is not EventHandler handler)
					{
						// Clean up old handlers that have been collected
						handlers.RemoveAt(i);
					}
					else
					{
						callees[count] = handler;
						count++;
					}
				}

				// Call the handlers that we snapshotted
				for (int i = 0; i < count; i++)
				{
					EventHandler handler = callees[i];
					handler(null, EventArgs.Empty);
				}
			}
		}

		internal static void AddHandlersToRequerySuggested(List<WeakReference> handlers)
		{
			if (handlers != null)
			{
				foreach (WeakReference handlerRef in handlers)
				{
					if (handlerRef.Target is EventHandler handler)
					{
						CommandManager.RequerySuggested += handler;
					}
				}
			}
		}

		internal static void RemoveHandlersFromRequerySuggested(List<WeakReference> handlers)
		{
			if (handlers != null)
			{
				foreach (WeakReference handlerRef in handlers)
				{
					if (handlerRef.Target is EventHandler handler)
					{
						CommandManager.RequerySuggested -= handler;
					}
				}
			}
		}

		internal static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler)
		{
			AddWeakReferenceHandler(ref handlers, handler, -1);
		}

		internal static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler, int defaultListSize)
		{
			handlers ??= (defaultListSize > 0 ? new List<WeakReference>(defaultListSize) : []);

			handlers.Add(new WeakReference(handler));
		}

		internal static void RemoveWeakReferenceHandler(List<WeakReference> handlers, EventHandler handler)
		{
			if (handlers != null)
			{
				for (int i = handlers.Count - 1; i >= 0; i--)
				{
					WeakReference reference = handlers[i];
					if ((reference.Target is not EventHandler existingHandler) || (existingHandler == handler))
					{
						// Clean up old handlers that have been collected
						// in addition to the handler that is to be removed.
						handlers.RemoveAt(i);
					}
				}
			}
		}
	}
}