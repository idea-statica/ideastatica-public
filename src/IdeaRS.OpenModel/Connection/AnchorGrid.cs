using IdeaRS.OpenModel.Parameters;

namespace IdeaRS.OpenModel.Connection
{
	/// <summary>
	/// Data of the anchor grid
	/// </summary>
	public class AnchorGrid : FastenerGridBase
	{
		/// <summary>
		/// Indicates, whether a shear plane is in the thread of a bolt.
		/// </summary>
		public bool ShearInThread { get; set; }

		/// <summary>
		/// Data of concrete block
		/// </summary>
		public ConcreteBlock ConcreteBlock { get; set; }

		/// <summary>
		/// Anchor Type - washer
		/// </summary>
		public AnchorType AnchorType { get; set; }


		/// <summary>
		/// Washer Size used if AnchorType is washer
		/// </summary>
		public double WasherSize { get; set; }
	}
}
