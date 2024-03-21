using System.Collections.Generic;

namespace IdeaRS.OpenModel.Detail.Loading
{
	/// <summary>
	/// Force 3D
	/// </summary>
	public class ForceLoad3D
	{
		/// <summary>
		/// force in X
		/// </summary>
		public double Fx { get; set; }

		/// <summary>
		/// force in Y
		/// </summary>
		public double Fy { get; set; }

		/// <summary>
		/// force in Z
		/// </summary>
		public double Fz { get; set; }

		/// <summary>
		/// Moment around X
		/// </summary>
		public double Mx { get; set; }

		/// <summary>
		/// Moment around Y
		/// </summary>
		public double My { get; set; }

		/// <summary>
		/// Moment around Z
		/// </summary>
		public double Mz { get; set; }

		/// <summary>
		/// point support position X
		/// </summary>
		public double PositionX { get; set; }

		/// <summary>
		/// point support position Y
		/// </summary>
		public double PositionY { get; set; }
	}


	/// <summary>
	/// Representation of Point Load 3D in IDEA StatiCa Detail
	/// </summary>
	public class ForcesLoad3D : LoadBase
	{
		public ForcesLoad3D() 
			: base ()
		{
			Forces = new List<ForceLoad3D> ();
		}

		/// <summary>
		/// Name of 3D element
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Load forces
		/// </summary>
		public List<ForceLoad3D> Forces { get; set; }

		/// <summary>
		/// Gets or sets the master component
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// Surface index of solid block ;
		/// SurfaceIndex set on -1 when MasterComponent is not SB
		/// </summary>
		public int SurfaceIndex { get; set; }

		/// <summary>
		/// Edge index of solid block ;
		/// EdgeIndex set on -1 when MasterComponent is not SB
		/// </summary>
		public int EdgeIndex { get; set; }

		/// <summary>
		/// direction type of load - local/global
		/// </summary>
		public IdeaRS.OpenModel.Loading.LoadDirection Direction { get; set; }
	}
}
