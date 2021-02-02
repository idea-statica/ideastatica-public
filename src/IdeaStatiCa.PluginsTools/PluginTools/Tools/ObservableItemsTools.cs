using System;
using System.Collections;
using System.Collections.CI.Common;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace CI.Common
{
	/// <summary>
	/// 
	/// </summary>
	public static class ObservableItemsTools
	{
		/// <summary>
		/// The extension for ObservableList generic collection with Clonables types.
		/// Creates a new collection that is a copy of the listToClone instance.
		/// </summary>
		/// <typeparam name="T">The type of elements in the list.</typeparam>
		/// <param name="listToClone">The collection to Cloning.</param>
		/// <returns>A new collection that is a copy of the listToClone instance.</returns>
		public static ObservableList<T> Clone<T>(this ObservableList<T> listToClone) where T : class
		{
			var a = listToClone.Where(item => item is ICloneable).Select(item => (item as ICloneable).Clone());
			return new ObservableList<T>(a.Cast<T>());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="oldValue"></param>
		/// <param name="oldvalue"></param>
		/// <param name="handler"></param>
		public static void SetNewObservedValue<T>(T newValue, ref T oldvalue, PropertyChangedEventHandler handler) where T : class
		{
			if (newValue == oldvalue)
			{
				return;
			}

			if (oldvalue != null)
			{
				// unregister previous event handler
				((INotifyPropertyChanged)oldvalue).PropertyChanged -= handler;
			}

			// set new value and register its event handler
			oldvalue = newValue;
			if (oldvalue != null)
			{
				((INotifyPropertyChanged)oldvalue).PropertyChanged += handler;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="oldValue"></param>
		/// <param name="oldvalue"></param>
		/// <param name="handler"></param>
		public static void AttachObservedValue<T>(T value, PropertyChangedEventHandler handler) where T : class
		{
			// register its event handler
			if (value != null)
			{
				((INotifyPropertyChanged)value).PropertyChanged += handler;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="oldValue"></param>
		/// <param name="oldvalue"></param>
		/// <param name="handler"></param>
		public static void DetachObservedValue<T>(T value, PropertyChangedEventHandler handler) where T : class
		{
			// register its event handler
			if (value != null)
			{
				((INotifyPropertyChanged)value).PropertyChanged -= handler;
			}
		}

		/// <summary>
		/// Registers observable handlers to specified container.
		/// </summary>
		/// <typeparam name="T">The generic type of container, must implement INotifyPropertyChanged.</typeparam>
		/// <param name="container">The specified container.</param>
		/// <param name="colectionChangedHandler">The handler for collection changes.</param>
		/// <param name="propertyChangedHandler">The handler for properties changes.</param>
		public static void RegisterContainerObservers<T>(IEnumerable<T> container, NotifyCollectionChangedEventHandler colectionChangedHandler, PropertyChangedEventHandler propertyChangedHandler)
			where T : class
		{
			foreach (INotifyPropertyChanged item in container)
			{
				item.PropertyChanged += propertyChangedHandler;
			}

			((INotifyCollectionChanged)container).CollectionChanged += colectionChangedHandler;
		}

		/// <summary>
		/// Detached observable handlers to specified container.
		/// </summary>
		/// <typeparam name="T">The generic type of container, must implement INotifyPropertyChanged.</typeparam>
		/// <param name="container">The specified container.</param>
		/// <param name="colectionChangedHandler">The handler for collection changes.</param>
		/// <param name="propertyChangedHandler">The handler for properties changes.</param>
		public static void DetachedContainerObservers<T>(IEnumerable<T> container, NotifyCollectionChangedEventHandler colectionChangedHandler, PropertyChangedEventHandler propertyChangedHandler)
			where T : class
		{
			foreach (INotifyPropertyChanged item in container)
			{
				item.PropertyChanged -= propertyChangedHandler;
			}

			((INotifyCollectionChanged)container).CollectionChanged -= colectionChangedHandler;
		}

		/// <summary>
		/// Registers observers.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <param name="handler"></param>
		public static void RegisterObservers(object sender, NotifyCollectionChangedEventArgs e, PropertyChangedEventHandler handler)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (INotifyPropertyChanged item in e.NewItems)
				{
					item.PropertyChanged += handler;
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (INotifyPropertyChanged item in e.OldItems)
				{
					item.PropertyChanged -= handler;
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Reset && sender is IEnumerable)
			{
				var items = sender as IEnumerable;

				foreach (INotifyPropertyChanged item in items)
				{
					item.PropertyChanged -= handler;
					item.PropertyChanged += handler;
				}
			}
		}
	}
}
