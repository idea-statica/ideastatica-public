using System;
using System.Runtime.InteropServices;

namespace CI.DataModel
{
	/// <summary>
	/// Interface implemented by items which can be used in models
	/// </summary>
	[ComVisible(true)]
	public interface IModelItem : IComparable
	{
		/// <summary>
		/// Gets the Container ID which identifies the container where the object is stored.
		/// </summary>
		Guid ContainerID { get; }

		/// <summary>
		/// Gets, sets identifier of the model item
		/// </summary>
		int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Name of the model item
		/// </summary>
		string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Sets modification for database
		/// </summary>
		void Modify();

		/// <summary>
		/// Notify all the changes that have happened in this object
		/// </summary>
		void NotifyAllChanges();
	}
}
