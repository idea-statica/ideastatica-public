using IdeaRS.OpenModel.Geometry2D;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// Creates one item cross-section of general cold formed profile defined by it´s center line
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.CrossSection.CrossSection,CI.CrossSection", "CI.StructModel.Libraries.CrossSection.ICrossSection,CI.BasicTypes", typeof(CrossSection))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class CrossSectionGeneralColdFormed : CrossSection
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CrossSectionGeneralColdFormed()
		{
			//CenterLine = new List<PolyLine2D>();
			Centerline = new PolyLine2D();
		}

		///// <summary>
		///// List of components
		///// </summary>
		//public List<PolyLine2D> CenterLine { get; set; }

		/// <summary>
		/// Centerline of cross-section made from linear segments
		/// </summary>
		public PolyLine2D Centerline { get; set; }


		/// <summary>
		/// Type of cross-section
		/// </summary>
		public CrossSectionType CrossSectionType { get; set; }

		/// <summary>
		/// Material of cross-section
		/// </summary>
		public ReferenceElement Material { get; set; }

		/// <summary>
		/// Thickness of thin wall cross-section
		/// </summary>
		public double Thickness { get; set; }

		/// <summary>
		/// Inner rounding radius between linear segments of centerline
		/// </summary>
		public double Radius { get; set; }


	}
}
