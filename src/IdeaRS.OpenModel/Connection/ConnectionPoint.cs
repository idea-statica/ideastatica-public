using System.Collections.Generic;

namespace IdeaRS.OpenModel.Connection
{
	/// <summary>
	/// ConnectedDesignMemeber
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.ConnectionPoint,CI.StructuralElements", "CI.StructModel.Structure.IConnectionPoint,CI.BasicTypes")]
	public class ConnectionPoint : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ConnectionPoint()
		{
			ConnectedMembers = new List<ConnectedMember>();
		}

		/// <summary>
		/// Name of Element
		/// </summary>
		public System.String Name { get; set; }

		/// <summary>
		/// Id of node
		/// </summary>
		/// <remarks>
		/// This property is not longer supported. Use the <see cref="Node"/> property instead of this one.
		/// </remarks>
		[System.Obsolete("Use the Node reference element.")]
		[System.Xml.Serialization.XmlIgnore]
		public int NodeId { get; set; }

		/// <summary>
		/// Node
		/// </summary>
		public ReferenceElement Node { get; set; }

		/// <summary>
		/// Connected members as collection of IDs.
		/// </summary>
		public List<ConnectedMember> ConnectedMembers { get; set; }

		/// <summary>
		/// Bearing Member
		/// </summary>
		public ReferenceElement BearingMember { get; set; }

		/// <summary>
		/// Gets or sets the Id of node for Xml serialization (back compatibility).
		/// </summary>
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		[System.Xml.Serialization.XmlElement(ElementName = "NodeId")]
		public int NodeIdSerialize
		{
#pragma warning disable 612, 618
			get { return NodeId; }
			set { NodeId = value; }
#pragma warning restore 612, 618
		}

		/// <summary>
		/// Set filename for Connection
		/// </summary>
		public string ProjectFileName { get; set; }

    }
}