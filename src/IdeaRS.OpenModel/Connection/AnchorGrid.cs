using IdeaRS.OpenModel.Parameters;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Connection
{
	/// <summary>
	/// Data of the anchor grid
	/// </summary>
	[DataContract]
	public class AnchorGrid : FastenerGridBase
	{
		/// <summary>
		/// Indicates, whether a shear plane is in the thread of a bolt.
		/// </summary>
		[DataMember]
		public bool ShearInThread { get; set; }

		/// <summary>
		/// Data of concrete block
		/// </summary>
		[DataMember]
		public ConcreteBlock ConcreteBlock { get; set; }

		/// <summary>
		/// Anchor Type - washer
		/// </summary>
		[DataMember]
		public AnchorType AnchorType { get; set; }


		/// <summary>
		/// Washer Size used if AnchorType is washer
		/// </summary>
		[DataMember]
		public double WasherSize { get; set; }

		/// <summary>
		/// Anchoring Length
		/// </summary>
		[DataMember]
		public double AnchoringLength
		{
			get;
			set;
		}

		/// <summary>
		/// Length of anchor hook<br/>
		/// (distance from the inner surface of the anchor shaft to the outer tip of the hook specified as an anchor diameter multiplier)
		/// </summary>
		[DataMember]
		public double HookLength { get; set; }

		/// <summary>
		/// Assembly
		/// </summary>
		[DataMember]
		public ReferenceElement BoltAssembly { get; set; }
	}
}
