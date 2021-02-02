using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CI.Geometry3D
{
	[Guid("5c28ebbb-8f4b-3201-b5cc-57f385f84a7a")]
	public interface IRegion3D
	{
		#region Properties

		/// <summary>
		/// Count of openings
		/// </summary>
		int OpeningsCount
		{
			get;
		}

		/// <summary>
		/// Gets or sets the outline polyline of the region
		/// </summary>
		IPolyLine3D Outline
		{
			get;
			set;
		}

		/// <summary>
		/// gets or sets the hole polygons inside the region
		/// </summary>
		IEnumerable<IPolyLine3D> Openings
		{
			get;
		}

		/// <summary>
		/// Coordinate system
		/// </summary>
		ICoordSystemByVector LCS {	get;}

		#endregion

		#region Methods

		/// <summary>
		/// Add a opening to collection
		/// </summary>
		/// <param name="opening">New opening to be added</param>
		void AddOpening(IPolyLine3D opening);

		/// <summary>
		/// Remove a opening from collection
		/// </summary>
		/// <param name="opening">The opening to be deleted</param>
		void RemoveOpening(IPolyLine3D opening);

		/// <summary>
		/// Remove a opening from collection
		/// </summary>
		/// <param name="index">The zero based index of the opening to get or set</param>
		void RemoveOpeningAt(int index);

		/// <summary>
		/// Remove all openings from the collection
		/// </summary>
		void ClearAllOpenings();

		/// <summary>
		/// Gets the opening at the specified index
		/// </summary>
		/// <param name="index">The zero based index of the opening to get or set</param>
		/// <returns>The opening at the specified index</returns>
		IPolyLine3D GetOpeningAt(int index);

		/// <summary>
		/// Sets the given opening at the specified index
		/// </summary>
		/// <param name="index">The zero based index of the opening to get or set</param>
		/// <param name="opening">The opening to be set at the specified index</param>
		/// <returns>True if the opening is set correctly
		/// False otherwise</returns>
		bool SetOpeningAt(int index, IPolyLine3D opening);

		#endregion
	}
}
