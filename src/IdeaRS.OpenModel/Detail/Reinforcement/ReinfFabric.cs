using IdeaRS.OpenModel.Geometry3D;
using System.Collections.Generic;

namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	///  Representing fabric reinforcement in IDEA StatiCa Detail
	/// </summary>
	[OpenModelClass(typeof(Reinforcement))]
	public class ReinfFabric : Reinforcement
	{
		/// <summary>
		/// constructor
		/// </summary>
		public ReinfFabric()
		{
		}

		/// <summary>
		/// Master component of reinforcement, if null, fabric reinforcement is in all components
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// Bar diameter - direction X
		/// </summary>
		public double DiameterX { get; set; }

		/// <summary>
		/// Bar diameter - direction Y
		/// </summary>
		public double DiameterY { get; set; }

		/// <summary>
		/// Distance between bars - direction X
		/// </summary>
		public double DistanceX { get; set; }

		/// <summary>
		/// Distance between bars - direction Y
		/// </summary>
		public double DistanceY { get; set; }

		/// <summary>
		/// true - add fabric reinfrocement separatelly for each wall, false - merge walls and add reinforcement on merged shape
		/// </summary>
		public bool SeparatelyPerWall { get; set; }

		/// <summary>
		/// number of fabric in wall projection
		/// </summary>
		public int Number { get; set; }

		/// <summary>
		/// Use cover from DetailModelSetting
		/// </summary>
		public bool CoverFromSetting { get; set; }

		/// <summary>
		/// Cover of reinforcement
		/// </summary>
		public double Cover { get; set; }

		/// <summary>
		/// Position of bottom left corner of fabric reinforcement related to point number 1 of master wall
		/// </summary>
		public Vector3D Position { get; set; }

		/// <summary>
		/// Rotation of fabric reinforcement related to global X coordinate
		/// </summary>
		public double Angle { get; set; }
	}
}
