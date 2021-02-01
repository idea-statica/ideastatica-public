using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;

namespace CI.Geometry2D
{
	[ComVisible(false)]
	public interface IRegions2D : ICloneable
	{
		/// <summary>
		/// 
		/// </summary>
		IRegion2D CrossSection_1stPart { get; }

		/// <summary>
		/// 
		/// </summary>
		IRegion2D CrossSection_2ndPart { get; }
	}

	[ComVisible(false)]
	public interface IRegion2D : ICloneable
	{
		IPolyLine2D Outline { get; set; }

		IList<IPolyLine2D> Openings { get; set; }

		double ArcDiscrAngle { get; set; }

		/// <summary>
		/// Begins an edit on an object.
		/// </summary>
		void BeginEdit();

		/// <summary>
		/// Rebuilds this instance
		/// </summary>
		void EndEdit();

		/// <summary>
		/// adds opening in opening list, create new Id for new opening
		/// </summary>
		/// <param name="opening">new openint</param>
		void AddOpening(IPolyLine2D opening);
	}

	[Guid("99A1EF5E-C92A-4253-A783-C2397DB239B1")]
	[ComVisible(true)]
	public interface IRegion2DCom
	{
		IPolyLine2DCom OutlineCom { get; set; }

		IList OpeningsCom { get; set; }
	}
}
