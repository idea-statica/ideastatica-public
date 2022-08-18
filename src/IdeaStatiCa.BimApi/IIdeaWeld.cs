
using IdeaRS.OpenModel.Connection;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// An weld is a part of a connection.
	/// </summary>
	public interface IIdeaWeld : IIdeaPersistentObject
	{
		double Thickness { get; set; }

		/// <summary>
		/// Material of the plate.
		/// </summary>
		IIdeaMaterial Material { get; }

		/// <summary>
		/// Start node of the weld line.
		/// </summary>
		IIdeaNode Start { get; }

		/// <summary>
		/// End node of the weld line. Coudl be same as start point
		/// </summary>
		IIdeaNode End { get; }

		/// <summary>
		/// Collection of parf witch shoud be welded
		/// </summary>
		IEnumerable<IIdeaObjectConnectable> ConnectedParts { get; }

		/// <summary>
		/// Type of weld
		/// </summary>
		WeldType WeldType { get; }
	}
}
