using CI.DataModel;
using System;
using System.Collections;
using System.Collections.CI.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace CI.Geometry2D
{
	/// <summary>
	/// Creates a 2D region included outline (border) and openings.
	/// </summary>
	[Obfuscation(Feature = "renaming")]

	public class Region2D :  IRegion2D, IRegion2DCom, IEditableObject
	{
		#region Fields

		[NonSerialized]
		private bool isEditing;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates empty region
		/// </summary>
		public Region2D()
		{
			this.isEditing = false;
			Openings = new ObservableList<IPolyLine2D>();
			ArcDiscrAngle = Math.PI / 12; // 15 degrees
		}

		/// <summary>
		/// Creates region and sets its outline by passed polyline
		/// </summary>
		/// <param name="outline">outline of the region</param>
		public Region2D(IPolyLine2D outline)
		{
			this.isEditing = false;
			Outline = outline;
			Openings = new ObservableList<IPolyLine2D>();
			ArcDiscrAngle = Math.PI / 12; // 15 degrees
		}

		/// <summary>
		/// Creates region and sets its outline and openings
		/// </summary>
		/// <param name="outline">Outline of the region</param>
		/// <param name="openings">Openings in the region</param>
		public Region2D(IPolyLine2D outline, IList<IPolyLine2D> openings)
		{
			this.isEditing = false;
			Outline = outline;
			Openings = openings;
			ArcDiscrAngle = Math.PI / 12; // 15 degrees
		}

		/// <summary>
		/// Initializes a new instance of this class and copies properties from specified source.
		/// </summary>
		/// <param name="source">The source to copy.</param>
		public Region2D(IRegion2D source)
		{
			this.isEditing = false;
			Outline = (IPolyLine2D)source.Outline.Clone();
			Openings = new ObservableList<IPolyLine2D>(source.Openings.Select(item => (IPolyLine2D)item.Clone()));

			ArcDiscrAngle = source.ArcDiscrAngle;
		}

		#endregion Constructors

		#region IRegion2D
		public virtual int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the outline curve of Region2D.
		/// </summary>
		[XmlIgnore]
		public IPolyLine2D Outline
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the list of openings in the Region2D.
		/// </summary>
		[XmlIgnore]
		public IList<IPolyLine2D> Openings { get; set; }

		/// <summary>
		/// angle for arc segment discretization
		/// </summary>
		public double ArcDiscrAngle { get; set; }

		#endregion IRegion2D

		#region Properties for serialization only

		/// <summary>
		/// Gets or sets the outline for serialization.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[EditorBrowsableAttribute(EditorBrowsableState.Never)]
		[XmlElement(ElementName = "Outline")]
		public PolyLine2D OutlineSerialize
		{
			get { return Outline as PolyLine2D; }
			set { Outline = value; }
		}

		#endregion Properties for serialization only

		#region IRegion2DCom

		/// <summary>
		/// Gets or sets the outline curve of Region2D.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[XmlIgnore]
		public IPolyLine2DCom OutlineCom
		{
			get { return Outline as IPolyLine2DCom; }
			set { Outline = value as IPolyLine2D; }
		}

		/// <summary>
		/// Gets or sets the list of openings in the Region2D.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[XmlArray(ElementName = "Openings")]
		[XmlArrayItem(Type = typeof(PolyLine2D))]
		public IList OpeningsCom
		{
			get { return Openings as IList; }
			set { Openings = value as IList<IPolyLine2D>; }
		}

		#endregion IRegion2DCom

		#region IEditableObject

		/// <summary>
		/// True if instance is currently edited
		/// </summary>
		protected bool IsEditing
		{
			get { return isEditing; }
			set { isEditing = value; }
		}

		/// <summary>
		/// Begins an edit on an object.
		/// </summary>
		public void BeginEdit()
		{
			IsEditing = true;
		}

		/// <summary>
		/// Discards changes since the last System.ComponentModel.IEditableObject.BeginEdit()
		/// </summary>
		public void CancelEdit()
		{
			IsEditing = false;
		}

		/// <summary>
		/// Rebuilds this instance
		/// </summary>
		public void EndEdit()
		{
			IsEditing = false;
			GeometryChanged();
		}

		#endregion IEditableObject

		#region Public methods

		/// <summary>
		/// adds opening in opening list, create new Id for new opening
		/// </summary>
		/// <param name="opening">new opening</param>
		public void AddOpening(IPolyLine2D opening)
		{
			var lastId = 0;
			if (Openings.Count > 0)
			{
				var opLast = Openings.MaxByOrDefault(o => o.Id);
				lastId = opLast != null ? opLast.Id : 0;
			}

			opening.Id = lastId + 1;
			Openings.Add(opening);
		}

		#endregion Public methods

				/// <summary>
		/// Updates geometry of this instance.
		/// </summary>
		protected virtual void GeometryChanged()
		{
		}

		/// <summary>
		/// this fuction must be called from all defaults constructors
		/// </summary>
		protected void StartDeserialization()
		{
			BeginEdit();
		}


		/// <summary>
		/// this function is called from the last deserialized property
		/// </summary>
		protected void EndDeserialization()
		{
			CancelEdit();
		}

		public object Clone()
		{
			throw new NotImplementedException();
		}
	}
}