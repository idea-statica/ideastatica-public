using System;
using System.Collections;
using System.Collections.Generic;

namespace CI.DataModel
{
	/// <summary>/// Conatainer of the items accessible by key
	/// Conatainer of the items accessible by key
	/// </summary>
	/// <typeparam name="ValueT">Type of the container</typeparam>
	public interface IContainer<ValueT> : IXSerializableContainer, IEnumerable<ValueT> where ValueT : class
	{
		/// <summary>
		/// Gets container id
		/// </summary>
		Guid ContainerId
		{
			get;
		}

		/// <summary>
		/// Gets the item from the container according to its id
		/// </summary>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException">The exception that is thrown when the key specified for accessing an element in a collection does not match any key in the collection.</exception>
		/// <param name="id">key</param>
		/// <returns>Required value</returns>
		ValueT GetValue(int id);

		/// <summary>
		/// Try get item from container
		/// </summary>
		/// <param name="id">key</param>
		/// <param name="value">value</param>
		/// <returns>True when operation was successful.</returns>
		bool TryGetValue(int id, out ValueT value);

		/// <summary>
		/// Adds item into container.
		/// </summary>
		/// <param name="item">Item which will be added into container</param>
		/// <returns>Key defing item in the container</returns>
		int Add(ValueT item);

		/// <summary>
		/// Adds item into container with specified id.
		/// </summary>
		/// <param name="item">Item which will be added into container</param>
		/// <param name="id">id of item</param>
		/// <returns>true if added sucessfully. false it this id already exists</returns>
		bool AddWithId(ValueT item, int id);

		/// <summary>
		/// Removes item from container.
		/// </summary>
		/// <param name="item">Object which will be removed</param>
		void Remove(ValueT item);

		/// <summary>
		///  Removes item from container.
		/// </summary>
		/// <param name="id">Id of the element which will be removed.</param>
		/// <returns>True if element with given Id is removed successfully,
		/// False otherwise</returns>
		bool Remove(int id);

		/// <summary>
		/// Removes all items from the <c>IContainer</c>.
		/// </summary>
		/// <exception cref="System.NotSupportedException">The <c>IContainer</c> is read-only.</exception>
		void Clear();

		/// <summary>
		/// Gets dictionary of the elements in the container
		/// </summary>
		/// <returns>Dictionary of the elements in the container</returns>
		IReadOnlyDictionary<int, ValueT> GetElements();

		/// <summary>
		/// Initiates a refresh operation
		/// </summary>
		/// <param name="completeRefresh">Raises OnCollectionChanged event with NotifyCollectionChangedAction.Reset argument</param>
		void Refresh(bool completeRefresh = false);

		/// <summary>
		/// Gets the count of elements in the container.
		/// </summary>
		/// <returns></returns>
		int Count { get; }
	}

	/// <summary>
	/// Conatainer of the items accessible by key (Nongeneric version of interface)
	/// </summary>
	public interface IContainer : ICollection, IXSerializableContainer
	{
		/// <summary>
		/// Get type of the items which are stored in the container
		/// </summary>
		/// <returns></returns>
		Type GetContItemType();

		/// <summary>
		///  Removes item from container.
		/// </summary>
		/// <param name="id">Id of the element which will be removed.</param>
		/// <returns>True if element with given Id is removed successfully,
		/// False otherwise</returns>
		bool Remove(int id);

		/// <summary>
		/// Removes all items from the <c>IContainer</c>.
		/// </summary>
		/// <exception cref="System.NotSupportedException">The <c>IContainer</c> is read-only.</exception>
		void Clear();

		///// <summary>
		///// Gets dictionary of the elements in the container
		///// </summary>
		///// <returns>Dictionary of the elements in the container</returns>
		//IDictionary GetDictionary();

		/// <summary>
		/// Try get item from container
		/// </summary>
		/// <param name="id">key</param>
		/// <param name="value">value</param>
		/// <returns>True when operation was successful.</returns>
		bool TryGetValue(int id, out object value);

		/// <summary>
		/// Initiates a refresh operation
		/// </summary>
		/// <param name="completeRefresh">Raises OnCollectionChanged event with NotifyCollectionChangedAction.Reset argument</param>
		void Refresh(bool completeRefresh = false);

		/// <summary>
		/// Adds item into container with specified id.
		/// </summary>
		/// <param name="item">Item which will be added into container</param>
		/// <param name="id">id of item</param>
		/// <returns>true if added sucessfully. false it this id already exists</returns>
		bool AddWithId(object item, int id);

		/// <summary>
		/// Adds item into container and sets correct id.
		/// </summary>
		/// <param name="item">Item which will be added into container</param>
		/// <returns>Key defing item in the container</returns>
		int Add(object item);

		/// <summary>
		///  Copy content of the <paramref name="source"/> into this container
		/// </summary>
		/// <param name="source">The source container</param>
		void Copy(IContainer source);
	}
}
