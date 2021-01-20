using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IdeaRS.OpenModel.Parameters
{
	/// <summary>
	/// Represents the code of the position
	/// </summary>
	public enum WeldType
	{
		/// <summary>
		/// No weld
		/// </summary>
		None,

		/// <summary>
		/// Fillet weld is on the left side
		/// </summary>
		LeftFillet,

		/// <summary>
		/// Fillet weld on the right side
		/// </summary>
		RightFillet,

		/// <summary>
		/// Fillet weld on both sides (double fillet)
		/// </summary>
		DoubleFillet,

		/// <summary>
		/// Bevel weld
		/// </summary>
		Bevel,
	}

	/// <summary>
	/// Parameter which represts the record of the weld in IDEA MPRL
	/// </summary>
	[DataContract]
	[Serializable]
	public class WeldParam : MprlRecord
	{
		/// <summary>
		/// Gets or sets the type of the weld
		/// </summary>
		[DataMember]
		public WeldType WeldType { get; set; }

		/// <summary>
		/// Gets or sets the size of the weld
		/// </summary>
		[DataMember]
		public double Size { get; set; }
	}
}
