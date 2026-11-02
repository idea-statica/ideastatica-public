using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using CI.Common;

namespace System.Collections.CI.Common //TODO
{
	/// <summary>
	/// Wrapper for generic <see cref="List(T)"/>. It allows to override methods such as Add, Remove etc...
	/// </summary>
	/// <typeparam name="T">Type of values in the list</typeparam>
	[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	public class ObservableList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged where T : class
	{
		private List<T> list;

		public ObservableList()
		{
			list = new List<T>();
		}

		public ObservableList(IEnumerable<T> collection)
		{
			list = new List<T>(collection);
			//foreach (INotifyPropertyChanged item in collection)
			foreach (T item in collection)
			{
				//item.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChangedHandler);
				T oldValue = null;
				ObservableItemsTools.SetNewObservedValue<T>(item, ref oldValue, OnPropertyChangedHandler);
			}
		}

		public ObservableList(int capacity)
		{
			list = new List<T>(capacity);
		}

		#region Events
		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		// Summary:
		//     Gets or sets the element at the specified index.
		//
		// Parameters:
		//   index:
		//     The zero-based index of the element to get or set.
		//
		// Returns:
		//     The element at the specified index.
		//
		// Exceptions:
		//   System.ArgumentOutOfRangeException:
		//     index is not a valid index in the System.Collections.Generic.IList<T>.
		//
		//   System.NotSupportedException:
		//     The property is set and the System.Collections.Generic.IList<T> is read-only.
		public T this[int index]
		{
			get
			{
				return list[index];
			}
			set
			{
				T oldValue = list[index];
				ObservableItemsTools.SetNewObservedValue<T>(value, ref oldValue, OnPropertyChangedHandler);
				list[index] = value;

#if SILVERLIGHT
				NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
#else
				NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue);
#endif

				OnCollectionChanged(arg);
			}
		}

		// Summary:
		//     Determines the index of a specific item in the System.Collections.Generic.IList<T>.
		//
		// Parameters:
		//   item:
		//     The object to locate in the System.Collections.Generic.IList<T>.
		//
		// Returns:
		//     The index of item if found in the list; otherwise, -1.
		public int IndexOf(T item)
		{
			return list.IndexOf(item);
		}

		//
		// Summary:
		//     Inserts an item to the System.Collections.Generic.IList<T> at the specified
		//     index.
		//
		// Parameters:
		//   index:
		//     The zero-based index at which item should be inserted.
		//
		//   item:
		//     The object to insert into the System.Collections.Generic.IList<T>.
		//
		// Exceptions:
		//   System.ArgumentOutOfRangeException:
		//     index is not a valid index in the System.Collections.Generic.IList<T>.
		//
		//   System.NotSupportedException:
		//     The System.Collections.Generic.IList<T> is read-only.
		public void Insert(int index, T item)
		{
			list.Insert(index, item);
			T oldValue = null;
			ObservableItemsTools.SetNewObservedValue<T>(item, ref oldValue, OnPropertyChangedHandler);

#if SILVERLIGHT
			NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
#else
			NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item);
#endif

			OnCollectionChanged(arg);
		}

		//
		// Summary:
		//     Removes the System.Collections.Generic.IList<T> item at the specified index.
		//
		// Parameters:
		//   index:
		//     The zero-based index of the item to remove.
		//
		// Exceptions:
		//   System.ArgumentOutOfRangeException:
		//     index is not a valid index in the System.Collections.Generic.IList<T>.
		//
		//   System.NotSupportedException:
		//     The System.Collections.Generic.IList<T> is read-only.
		public void RemoveAt(int index)
		{
			T oldValue = list[index];
			T removedValue = oldValue;
			ObservableItemsTools.SetNewObservedValue<T>(null, ref removedValue, OnPropertyChangedHandler);
			list.RemoveAt(index);

#if SILVERLIGHT
			NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
#else
			NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldValue);
#endif

			OnCollectionChanged(arg);
		}

		// Summary:
		//     Gets the number of elements contained in the System.Collections.Generic.ICollection<T>.
		//
		// Returns:
		//     The number of elements contained in the System.Collections.Generic.ICollection<T>.
		public int Count
		{
			get
			{
				return list.Count;
			}
		}

		//
		// Summary:
		//     Gets a value indicating whether the System.Collections.Generic.ICollection<T>
		//     is read-only.
		//
		// Returns:
		//     true if the System.Collections.Generic.ICollection<T> is read-only; otherwise,
		//     false.
		public bool IsReadOnly
		{
			get
			{
				return ((IList)list).IsReadOnly;
			}
		}

		// Summary:
		//     Adds an item to the System.Collections.Generic.ICollection<T>.
		//
		// Parameters:
		//   item:
		//     The object to add to the System.Collections.Generic.ICollection<T>.
		//
		// Exceptions:
		//   System.NotSupportedException:
		//     The System.Collections.Generic.ICollection<T> is read-only.
		public void Add(T item)
		{
			list.Add(item);
			T oldValue = null;
			ObservableItemsTools.SetNewObservedValue<T>(item, ref oldValue, OnPropertyChangedHandler);

#if SILVERLIGHT
			NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
#else
			NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item);
#endif

			OnCollectionChanged(arg);
		}

		//
		// Summary:
		//     Adds the elements of the specified collection to the end of the System.Collections.CI.Common.ObservableList<T>.
		//
		// Parameters:
		//   collection:
		//     The collection whose elements should be added to the end of the System.Collections.CI.Common.ObservableList<T>.
		//     The collection itself cannot be null, but it can contain elements that are
		//     null, if type T is a reference type.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     collection is null.
		public void AddRange(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException();
			}

			list.AddRange(collection);
			T oldValue = null;
			foreach (var item in collection)
			{
				oldValue = null;
				ObservableItemsTools.SetNewObservedValue<T>(item, ref oldValue, OnPropertyChangedHandler);
			}

			var arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
			OnCollectionChanged(arg);
		}

		//
		// Summary:
		//     Removes all items from the System.Collections.Generic.ICollection<T>.
		//
		// Exceptions:
		//   System.NotSupportedException:
		//     The System.Collections.Generic.ICollection<T> is read-only.
		public void Clear()
		{
			int count = list.Count;
			T newVal = null;
			foreach (T item in list)
			{
				T it = item;
				ObservableItemsTools.SetNewObservedValue<T>(newVal, ref it, OnPropertyChangedHandler);
				//item.PropertyChanged -= new PropertyChangedEventHandler(OnPropertyChangedHandler);
			}

			list.Clear();
			if (count > 0)
			{
				NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
				OnCollectionChanged(arg);
			}
		}

		//
		// Summary:
		//     Determines whether the System.Collections.Generic.ICollection<T> contains
		//     a specific value.
		//
		// Parameters:
		//   item:
		//     The object to locate in the System.Collections.Generic.ICollection<T>.
		//
		// Returns:
		//     true if item is found in the System.Collections.Generic.ICollection<T>; otherwise,
		//     false.
		public bool Contains(T item)
		{
			return list.Contains(item);
		}

		//
		// Summary:
		//     Copies the elements of the System.Collections.Generic.ICollection<T> to an
		//     System.Array, starting at a particular System.Array index.
		//
		// Parameters:
		//   array:
		//     The one-dimensional System.Array that is the destination of the elements
		//     copied from System.Collections.Generic.ICollection<T>. The System.Array must
		//     have zero-based indexing.
		//
		//   arrayIndex:
		//     The zero-based index in array at which copying begins.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     array is null.
		//
		//   System.ArgumentOutOfRangeException:
		//     arrayIndex is less than 0.
		//
		//   System.ArgumentException:
		//     array is multidimensional.-or-The number of elements in the source System.Collections.Generic.ICollection<T>
		//     is greater than the available space from arrayIndex to the end of the destination
		//     array.-or-Type T cannot be cast automatically to the type of the destination
		//     array.
		public void CopyTo(T[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		//
		// Summary:
		//     Removes the first occurrence of a specific object from the System.Collections.Generic.ICollection<T>.
		//
		// Parameters:
		//   item:
		//     The object to remove from the System.Collections.Generic.ICollection<T>.
		//
		// Returns:
		//     true if item was successfully removed from the System.Collections.Generic.ICollection<T>;
		//     otherwise, false. This method also returns false if item is not found in
		//     the original System.Collections.Generic.ICollection<T>.
		//
		// Exceptions:
		//   System.NotSupportedException:
		//     The System.Collections.Generic.ICollection<T> is read-only.
		public bool Remove(T item)
		{
			T removedItem = item;
			ObservableItemsTools.SetNewObservedValue<T>(null, ref removedItem, OnPropertyChangedHandler);
			bool retval = list.Remove(item);
			if (retval)
			{
#if SILVERLIGHT
				NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
#else
				NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item);
#endif

				OnCollectionChanged(arg);
			}

			return retval;
		}

		// Summary:
		//     Returns an enumerator that iterates through the collection.
		//
		// Returns:
		//     A System.Collections.Generic.IEnumerator<T> that can be used to iterate through
		//     the collection.
		public IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		// Summary:
		//     Returns an enumerator that iterates through a collection.
		//
		// Returns:
		//     An System.Collections.IEnumerator object that can be used to iterate through
		//     the collection.
		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		// Summary:
		//     Gets a value indicating whether the System.Collections.IList has a fixed
		//     size.
		//
		// Returns:
		//     true if the System.Collections.IList has a fixed size; otherwise, false.
		public bool IsFixedSize
		{
			get
			{
				return ((IList)list).IsFixedSize;
			}
		}

		// Summary:
		//     Gets or sets the element at the specified index.
		//
		// Parameters:
		//   index:
		//     The zero-based index of the element to get or set.
		//
		// Returns:
		//     The element at the specified index.
		//
		// Exceptions:
		//   System.ArgumentOutOfRangeException:
		//     index is not a valid index in the System.Collections.IList.
		//
		//   System.NotSupportedException:
		//     The property is set and the System.Collections.IList is read-only.
		object IList.this[int index]
		{
			get
			{
				return ((IList)list)[index];
			}

			set
			{
				T oldValue = list[index];
				ObservableItemsTools.SetNewObservedValue<T>(value as T, ref oldValue, OnPropertyChangedHandler);
				((IList)list)[index] = value;
			}
		}

		// Summary:
		//     Adds an item to the System.Collections.IList.
		//
		// Parameters:
		//   value:
		//     The object to add to the System.Collections.IList.
		//
		// Returns:
		//     The position into which the new element was inserted, or -1 to indicate that
		//     the item was not inserted into the collection,
		//
		// Exceptions:
		//   System.NotSupportedException:
		//     The System.Collections.IList is read-only.-or- The System.Collections.IList
		//     has a fixed size.
		int IList.Add(object value)
		{
			T oldValue = null;
			ObservableItemsTools.SetNewObservedValue<T>(value as T, ref oldValue, OnPropertyChangedHandler);
			int retval = ((IList)list).Add(value);

#if SILVERLIGHT
			NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
#else
			NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value);
#endif

			OnCollectionChanged(arg);
			return retval;
		}

		//
		// Summary:
		//     Determines whether the System.Collections.IList contains a specific value.
		//
		// Parameters:
		//   value:
		//     The object to locate in the System.Collections.IList.
		//
		// Returns:
		//     true if the System.Object is found in the System.Collections.IList; otherwise,
		//     false.
		bool IList.Contains(object value)
		{
			return ((IList)list).Contains(value);
		}

		//
		// Summary:
		//     Determines the index of a specific item in the System.Collections.IList.
		//
		// Parameters:
		//   value:
		//     The object to locate in the System.Collections.IList.
		//
		// Returns:
		//     The index of value if found in the list; otherwise, -1.
		int IList.IndexOf(object value)
		{
			return ((IList)list).IndexOf(value);
		}

		//
		// Summary:
		//     Inserts an item to the System.Collections.IList at the specified index.
		//
		// Parameters:
		//   index:
		//     The zero-based index at which value should be inserted.
		//
		//   value:
		//     The object to insert into the System.Collections.IList.
		//
		// Exceptions:
		//   System.ArgumentOutOfRangeException:
		//     index is not a valid index in the System.Collections.IList.
		//
		//   System.NotSupportedException:
		//     The System.Collections.IList is read-only.-or- The System.Collections.IList
		//     has a fixed size.
		//
		//   System.NullReferenceException:
		//     value is null reference in the System.Collections.IList.
		void IList.Insert(int index, object value)
		{
			T oldValue = null;
			ObservableItemsTools.SetNewObservedValue<T>(value as T, ref oldValue, OnPropertyChangedHandler);
			((IList)list).Insert(index, value);

#if SILVERLIGHT
			NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
#else
			NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value);
#endif

			OnCollectionChanged(arg);
		}

		//
		// Summary:
		//     Removes the first occurrence of a specific object from the System.Collections.IList.
		//
		// Parameters:
		//   value:
		//     The object to remove from the System.Collections.IList.
		//
		// Exceptions:
		//   System.NotSupportedException:
		//     The System.Collections.IList is read-only.-or- The System.Collections.IList
		//     has a fixed size.
		void IList.Remove(object value)
		{
			T oldValue = value as T;
			ObservableItemsTools.SetNewObservedValue<T>(null, ref oldValue, OnPropertyChangedHandler);
			((IList)list).Remove(value);

#if SILVERLIGHT
			NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
#else
			NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value);
#endif

			OnCollectionChanged(arg);
		}

		//
		// Summary:
		//     Gets a value indicating whether access to the System.Collections.ICollection
		//     is synchronized (thread safe).
		//
		// Returns:
		//     true if access to the System.Collections.ICollection is synchronized (thread
		//     safe); otherwise, false.
		public bool IsSynchronized
		{
			get
			{
				return ((IList)list).IsSynchronized;
			}
		}

		//
		// Summary:
		//     Gets an object that can be used to synchronize access to the System.Collections.ICollection.
		//
		// Returns:
		//     An object that can be used to synchronize access to the System.Collections.ICollection.
		public object SyncRoot
		{
			get
			{
				return ((IList)list).SyncRoot;
			}
		}

		// Summary:
		//     Copies the elements of the System.Collections.ICollection to an System.Array,
		//     starting at a particular System.Array index.
		//
		// Parameters:
		//   array:
		//     The one-dimensional System.Array that is the destination of the elements
		//     copied from System.Collections.ICollection. The System.Array must have zero-based
		//     indexing.
		//
		//   index:
		//     The zero-based index in array at which copying begins.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     array is null.
		//
		//   System.ArgumentOutOfRangeException:
		//     index is less than zero.
		//
		//   System.ArgumentException:
		//     array is multidimensional.-or- The number of elements in the source System.Collections.ICollection
		//     is greater than the available space from index to the end of the destination
		//     array.
		//
		//   System.ArgumentException:
		//     The type of the source System.Collections.ICollection cannot be cast automatically
		//     to the type of the destination array.
		public void CopyTo(Array array, int index)
		{
			((IList)list).CopyTo(array, index);
		}

		public virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (CollectionChanged != null)
			{
				CollectionChanged(this, e);
			}
		}

		protected void OnPropertyChangedHandler(object sender, PropertyChangedEventArgs arg)
		{
			ForwardPropertyChanged(arg.PropertyName, sender, arg);
		}

		/// <summary>
		/// Wraps <paramref name="origArg"/> into <see cref="ForwardedPropertyChangedEventArgs"/> and raises <see cref="PropertyChangedEventHandler"/>.
		/// </summary>
		/// <param name="propertyName">Name of the property in this object.</param>
		/// <param name="origSender">Original sender.</param>
		/// <param name="origArg">Original argument.</param>
		protected void ForwardPropertyChanged(string propertyName, object origSender, PropertyChangedEventArgs origArg)
		{
			/*PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				if (origArg is ForwardedPropertyChangedEventArgs)
				{
					origSender = (origArg as ForwardedPropertyChangedEventArgs).OriginalSender;
				}
				handler(this, new ForwardedPropertyChangedEventArgs(propertyName, origSender, origArg));
			}*/
		}

		/// <summary>
		/// Raises event about property change
		/// </summary>
		/// <param name="propertyName">Name of changed property</param>
		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
