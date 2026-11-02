using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Windows;


namespace CI.DataModel
{
	/// <summary>
	/// Represents the method that will handle the ElementBase.ObjectChanged
	/// event raised when more than one property of the object is changed and the object is in a dirty state
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="args">The args of the event.</param>
	public delegate void ObjectChangedEventHandler(object sender, EventArgs args);

	///// <summary>
	///// An event which is published when we need a Linked objects
	///// </summary>
	///// <param name="sender">Source of the event</param>
	///// <param name="linkedObjects">List of Linked Object</param>
	//public delegate void ObjectLinkedEventHandler(object sender, IList<IModelItem> linkedObjects);

	/// <summary>
	/// Base abstarct class for structural objects
	/// </summary>
	[ComVisible(true)]
	public abstract class ElementBase :   INotifyPropertyChanged, IWeakEventListener
	{
		#region Constructors

		public ElementBase()
		{
			Id = -1;
			Name = string.Empty;
			isDirty = false;
			containerID = Guid.Empty;
			name = string.Empty;
		}

		public ElementBase(ElementBase source)
		{
			Id = -1;
			Name = string.Empty;
			isDirty = false;
			containerID = source.ContainerID;
		}

		#endregion

		#region Member Variables

		[NonSerialized]
		private bool isDirty;

		private Guid containerID;
		private string name;

		#endregion

		#region Events

		/// <summary>
		/// An event which is published when a property is changed
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		public event System.EventHandler NotifyDirty;

		/// <summary>
		/// An event which is published when more than one properties of the object is changed and the object is dirty
		/// </summary>
		public event ObjectChangedEventHandler ObjectChanged;

		///// <summary>
		///// An event which is published when we need a Linked objects
		///// </summary>
		//public event ObjectLinkedEventHandler ObjectLinked;

		#endregion

		#region Properties

		#region IModelItem Properties

		/// <summary>
		/// ID of Element
		/// </summary>
		public virtual int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Name of Element
		/// </summary>
		public virtual string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Default name for case when no name was set.
		/// </summary>
		public virtual string DefaultName
		{
			get
			{
				return Id.ToString();
			}
		}

		#endregion


		[XmlIgnore]
		public bool IsDirty
		{
			get
			{
				return isDirty;
			}

			set
			{
				isDirty = value;
				OnNotifyDirty();
			}
		}

		/// <summary>
		/// Container ID is the type identifier for the container where the object is stored.
		/// This value is set by the 
		/// </summary>
		[XmlIgnore]
		public Guid ContainerID
		{
			get
			{
				return containerID;
			}

			set
			{
				if (containerID == Guid.Empty)
				{
					containerID = value;
				}
				else
				{
					throw new InvalidOperationException("Container ID is set when the instance is added to the container. You cannot reset it");
				}
			}
		}
		#endregion

		#region IComparable

		/// <summary>
		/// Compares the current instance with another object
		/// </summary>
		/// <param name="obj">An object to compare with this instance</param>
		/// <returns>A value that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (GetType() == obj.GetType())
			{
				ElementBase elementBase = obj as ElementBase;
				// types are same, compare IDs 
				if (Id == elementBase.Id)
				{
					// equals
					return 0;
				}
				else if (Id < elementBase.Id)
				{
					// the first is less
					return -1;
				}

				// the first is bigger
				return 1;
			}

			// different types
			if (GetType().GetHashCode() < obj.GetType().GetHashCode())
			{
				return -1;
			}

			return 1;
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Notify all the changes that have happened in this object
		/// </summary>
		public void NotifyAllChanges()
		{
			if (Id > 0)
			{
				OnObjectChanged(this);
			}
		}

		/// <summary>
		/// This method can be overriden by the derived classes to register the events which the object is interested in
		/// </summary>
		public virtual void RegisterEvents()
		{
		}

		/// <summary>
		/// This method can be overriden by the derived classes to unregister the events which the object is no more interested in
		/// </summary>
		public virtual void UnRegisterEvents()
		{
		}

		public void NotifyPropertyChanged(string property)
		{
			OnPropertyChanged(property);
		}


		/// <summary>
		/// ReceiveWeakEvent
		/// </summary>
		/// <param name="managerType">type</param>
		/// <param name="sender">sender</param>
		/// <param name="e">args</param>
		/// <returns>true if  managed</returns>
		public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			return ReceiveWeakEventTo(managerType, sender, e);
		}

		protected virtual bool ReceiveWeakEventTo(Type managerType, object sender, EventArgs e)
		{
			return true;
		}


		///// <summary>
		///// Find the list of linked object for this element
		///// </summary>
		///// <param name="linkedObjects">List of object linked</param>
		//public void FindLinkedObject(IList<IModelItem> linkedObjects)
		//{
		//  RaiseObjectLinkedEvent(this, linkedObjects);
		//}

		/// <summary>
		/// Resets id and container id of this element of this element
		/// </summary>
		public void ResetContainerId()
		{
			containerID = Guid.Empty;
			//Id = 0; - HERY 3.4.2013 zakomentováno, ID by se nemělo nulovat! Existují objekty, které referencují přes ID, dělá to pak binec v datech (např. při synchronizaci mezi modely).
		}
		#endregion

		#region Protected methods

		protected void ClearAllEvents()
		{
			if (PropertyChanged != null)
			{
				foreach (var eventhandler in PropertyChanged.GetInvocationList())
				{
					PropertyChanged -= (PropertyChangedEventHandler)eventhandler;
				}

				PropertyChanged = null;
			}

			if (NotifyDirty != null)
			{
				foreach (var eventhandler in NotifyDirty.GetInvocationList())
				{
					NotifyDirty -= (System.EventHandler)eventhandler;
				}

				NotifyDirty = null;
			}

			if (ObjectChanged != null)
			{
				foreach (var eventhandler in ObjectChanged.GetInvocationList())
				{
					ObjectChanged -= (ObjectChangedEventHandler)eventhandler;
				}

				ObjectChanged = null;
			}
		}

		protected void OnObjectChanged(ElementBase dataElement)
		{
			ObjectChangedEventHandler handler = ObjectChanged;
			if (handler != null)
			{
				handler(this, new EventArgs());
			}
		}

		/// <summary>
		/// Raise on Properchange event when property value changed
		/// </summary>
		/// <param name="name">Name of the property</param>
		protected virtual void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}

		protected void OnNotifyDirty()
		{
			EventHandler handler = NotifyDirty;
			if (handler != null)
			{
				handler(this, null);
			}
		}

		#endregion
	}
}
