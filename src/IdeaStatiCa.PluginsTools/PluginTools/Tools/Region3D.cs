using System;
using System.Collections.Generic;
using CI.DataModel;

namespace CI.Geometry3D
{
	public class Region3D : ElementBase, IRegion3D
	{
		#region Fields

		/// <summary>
		/// Region Outline
		/// </summary>
		private IPolyLine3D outline;

		/// <summary>
		/// List of Openings
		/// </summary>
		private IList<IPolyLine3D> openings;

		private ICoordSystemByVector lcs;

		#endregion

		#region Constructors

		/// <summary>
		/// creates a region of zero area
		/// </summary>
		public Region3D()
		{
			Name = "Region";
			Outline = new PolyLine3D();
			openings = new List<IPolyLine3D>();
			lcs = new CoordSystemByVector();
		}

		/// <summary>
		/// Creates a region from another one
		/// </summary>
		/// <param name="source">Source region</param>
		public Region3D(IRegion3D source)
		{
			if (source != null)
			{
				Name = (source as ElementBase).Name;
				Outline = new PolyLine3D(source.Outline);
				if (source.Openings != null)
				{
					openings = new List<IPolyLine3D>();
					foreach (IPolyLine3D opening in source.Openings)
					{
						AddOpening(new PolyLine3D(opening));
					}
				}

				if (source.LCS != null)
				{
					this.LCS = new CoordSystemByVector(source.LCS);
				}
			}
		}

		#endregion

		#region IRegion3D Properties

		/// <summary>
		/// Count of openings
		/// </summary>
		public int OpeningsCount
		{
			get
			{
				return openings.Count;
			}
		}

		/// <summary>
		/// Outline
		/// </summary>
		public IPolyLine3D Outline
		{
			get
			{
				return outline;
			}

			set
			{
				if (outline != value)
				{
					UnsubscribeEvents(outline);
					outline = value;
					SubscribeEvents(outline);
				}
			}
		}

		/// <summary>
		/// Openings
		/// </summary>
		public IEnumerable<IPolyLine3D> Openings
		{
			get
			{
				return openings;
			}
		}

		/// <summary>
		/// LCS vectors of region
		/// </summary>
		public ICoordSystemByVector LCS
		{
			get
			{
				return lcs;
			}
			set
			{
				lcs = value;
			}
		}

		#endregion

		#region Methods

		#region IRegion3D Methods

		/// <summary>
		/// Add a opening to collection
		/// </summary>
		/// <param name="opening">New opening to be added</param>
		public void AddOpening(IPolyLine3D opening)
		{
			SubscribeEvents(opening);
			openings.Add(opening);
		}

		/// <summary>
		/// Remove a opening from collection
		/// </summary>
		/// <param name="opening">The opening to be deleted</param>
		public void RemoveOpening(IPolyLine3D opening)
		{
			if (openings.Remove(opening))
			{
				UnsubscribeEvents(opening);
			}
		}

		/// <summary>
		/// Remove a opening from collection
		/// </summary>
		/// <param name="index">The zero based index of the opening to get or set</param>
		public void RemoveOpeningAt(int index)
		{
			if ((index < 0) || (index >= openings.Count))
			{
				return;
			}

			UnsubscribeEvents(openings[index]);
			openings.RemoveAt(index);
		}

		/// <summary>
		/// Remove all openings from the collection
		/// </summary>
		public void ClearAllOpenings()
		{
			foreach (IPolyLine3D opening in openings)
			{
				UnsubscribeEvents(opening);
			}

			openings.Clear();
		}

		/// <summary>
		/// Gets the opening at the specified index
		/// </summary>
		/// <param name="index">The zero based index of the opening to get or set</param>
		/// <returns>The opening at the specified index</returns>
		public IPolyLine3D GetOpeningAt(int index)
		{
			return openings[index];
		}

		/// <summary>
		/// Sets the given opening at the specified index
		/// </summary>
		/// <param name="index">The zero based index of the opening to get or set</param>
		/// <param name="opening">The opening to be set at the specified index</param>
		/// <returns>True if the opening is set correctly
		/// False otherwise</returns>
		public bool SetOpeningAt(int index, IPolyLine3D opening)
		{
			if (index < 0 || index >= opening.Count)
			{
				return false;
			}

			UnsubscribeEvents(openings[index]);
			openings[index] = opening;
			SubscribeEvents(openings[index]);
			return true;
		}

		#endregion

		

		/// <summary>
		/// Subscribe events from openings
		/// </summary>
		protected void SubscribeEventsFromOpenings()
		{
			foreach (IPolyLine3D opening in Openings)
			{
				SubscribeEvents(opening);
			}
		}

		/// <summary>
		/// Subscribe Events From given polyline
		/// </summary>
		protected void SubscribeEvents(IPolyLine3D geometry)
		{
			//ElementBase element = geometry as ElementBase;
			//if (element == null)
			//{
			//  return;
			//}

			////UnsubscribeEvents(geometry);
			////element.ObjectChanged += new ObjectChangedEventHandler(OnGeometryChanged);
			////element.ObjectLinked += new ObjectLinkedEventHandler(OnObjectLinked);
			//PropertyChangedEventManager.AddListener(element, this);
		}

		/// <summary>
		/// Unsubscribe Events From given polyline
		/// </summary>
		protected void UnsubscribeEvents(IPolyLine3D geometry)
		{
			//ElementBase element = geometry as ElementBase;
			//if (element == null)
			//{
			//  return;
			//}

			////element.ObjectChanged -= new ObjectChangedEventHandler(OnGeometryChanged);
			////element.ObjectLinked -= new ObjectLinkedEventHandler(OnObjectLinked);
			//PropertyChangedEventManager.RemoveListener(element, this);
		}

		///// <summary>
		///// Find linked objects of Region and add into the list
		///// </summary>
		///// <param name="sender">Segment</param>
		///// <param name="linkedObjects">add linked objects to this list</param>
		//protected override void OnObjectLinked(object sender, IList<IModelItem> linkedObjects)
		//{
		//  RaiseObjectLinkedEvent(this, linkedObjects);
		//}

		/// <summary>
		/// Method is raised when geometry is changed
		/// </summary>
		/// <param name="sender">object</param>
		private void OnGeometryChanged(object sender)
		{
			NotifyAllChanges();
		}

		#endregion
	}
}