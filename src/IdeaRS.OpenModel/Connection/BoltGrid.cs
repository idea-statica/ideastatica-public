using IdeaRS.OpenModel.Parameters;

namespace IdeaRS.OpenModel.Connection
{
	public class BoltGrid : FastenerGridBase
	{
		/// <summary>
		/// Indicates, whether a shear plane is in the thread of a bolt.
		/// </summary>
		public bool ShearInThread { get; set; }

		/// <summary>
		/// Indicates type of shear transfer
		/// </summary>
		public BoltShearType BoltInteraction { get; set; }

		/// <summary>
		/// Assembly
		/// </summary>
		public ReferenceElement BoltAssembly { get; set; }
	}
}
