using System;
using System.Collections.Generic;

namespace CI.DataModel
{
	public enum TransactionType
	{
		ReadOnly,
		ReadWrite
	}

	public interface IContainerStorageProvider
	{
		IContainerStorage GetStorage();

		void SetStorage(IContainerStorage contStorage);
	}


	/// <summary>
	/// Storage of conatiners
	/// </summary>
	public interface IContainerStorage
	{
		/// <summary>
		/// POS - temp solution, to be able to switch storages
		/// </summary>
		IContainerStorageProvider StorageProvider { get; }

		/// <summary>
		/// Gets the map of the named instances.
		/// It allows to store instances which are accessible by name.
		/// </summary>
		IDictionary<string, object> NamedInstanceMap
		{
			get;
		}

		/// <summary>
		/// Get value from NamedInstanceMap
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		bool NamedInstanceMapTryGetValue(string name, out object value);

		/// <summary>
		/// Contains key in NamedInstanceMap
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		bool NamedInstanceMapContainsKey(string name);

		/// <summary>
		/// Gets dictionary of all containers in the storage
		/// </summary>
		IDictionary<Guid, object> Containers { get; }

		/// <summary>
		/// Gets container of the required type. If container doesn't exist, new instance is created.
		/// </summary>
		/// <typeparam name="ValueT">Type of the container</typeparam>
		/// <returns>Reference to the required container</returns>
		IContainer<ValueT> GetContainer<ValueT>() where ValueT : class;

		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <typeparam name="ValueT">Type of the container</typeparam>
		/// <param name="foundContainer">Found container or null</param>
		/// <returns>true if the storage contains tre required container</returns>
		bool TryGetContainer<ValueT>(out IContainer<ValueT> foundContainer) where ValueT : class;

		/// <summary>
		/// Gets container of the required type
		/// </summary>
		/// <param name="containerType">Type of the container</param>
		/// <param name="create">Create id it doesn't exist</param>
		/// <returns>Reference to the required container or null if container doesn't exist.</returns>
		IContainer GetContainer(Type containerType, bool create = false);

		/// <summary>
		/// Gets container of the required type if the ID of the container is known
		/// </summary>
		/// <param name="containerId">ID of the type of container</param>
		/// <returns>Reference to the required container or null if container doesn't exist.</returns>
		IContainer GetContainer(Guid containerId);

		/// <summary>
		/// Checks whether container of the given type exists in the storage
		/// </summary>
		/// <typeparam name="T">Type of the container</typeparam>
		/// <returns>True if container exists in the storage</returns>
		bool IsContainer<T>();

		/// <summary>
		/// Removes container of the given type from the storage
		/// </summary>
		/// <typeparam name="T">Type of the container</typeparam>
		/// <returns>true if the container is successfully removed from the storage; otherwise, false.</returns>
		bool RemoveContainer<T>();

		/// <summary>
		/// Clears container storage.
		/// </summary>
		void Clear();

		/// <summary>
		/// Commits last changes in the storage
		/// </summary>
		void Commit();

		/// <summary>
		/// 	Starts a database transaction with the specified isolation level.
		/// </summary>
		/// <param name="trasactionType">The isolation level under which the transaction should run.</param>
		void BeginTransaction(CI.DataModel.TransactionType trasactionType);

		/// <summary>
		/// Finishes the current database transaction
		/// </summary>
		void EndTransaction();

		/// <summary>
		/// Deletes all changes since the last calling BeginTransaction
		/// </summary>
		void RollBackTransaction();
	}
}
