using IdeaRS.OpenModel.Parameters;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Connection
{
	[DataContract]
	public class BoltGrid : FastenerGridBase
	{
		/// <summary>
		/// Indicates, whether a shear plane is in the thread of a bolt.
		/// </summary>
		[DataMember]
		public bool ShearInThread { get; set; }

		/// <summary>
		/// Indicates type of shear transfer
		/// </summary>
		[DataMember]
		public BoltShearType BoltInteraction { get; set; }

		/// <summary>
		/// Assembly
		/// </summary>
		[DataMember]
		public ReferenceElement BoltAssembly { get; set; }

		/// <summary>
		/// Slotted holes of grid positions per connected plate. Null or missing entry means a round hole.
		/// </summary>
		[DataMember]
		public List<SlottedHole> SlottedHoles { get; set; }
	}
}
