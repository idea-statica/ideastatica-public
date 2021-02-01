using System.Runtime.InteropServices;

namespace CI.Geometry3D
{
	/// <summary>
	/// Interface for Point On Region.
	/// </summary>
	[Guid("5255d329-4139-38a5-93f4-3845a86bb0e0")]
	public interface IPointOnRegion : IPoint3D
	{
		#region Properties

		/// <summary>
		/// Region
		/// </summary>
		IRegion3D Region
		{
			get;
			set;
		}

		/// <summary>
		/// offset of LocalX
		/// </summary>
		double OffsetX
		{
			get;
			set;
		}

		/// <summary>
		/// offset of LocalY
		/// </summary>
		double OffsetY
		{
			get;
			set;
		}

		#endregion

		#region Methods

		#endregion
	}
}
