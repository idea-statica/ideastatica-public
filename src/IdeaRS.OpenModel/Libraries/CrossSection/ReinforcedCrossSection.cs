using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// Reinforced cross-section
	/// </summary>
	/// <example> 
	/// This sample shows how to create reinforced cross-section.
	/// <code lang = "C#">
	/// //Creating the model
	/// OpenModel openModel = new OpenModel();
	/// 
	/// //Cocrete material
	/// MatConcreteEc2 mat = new MatConcreteEc2();
	/// //...
	/// openModel.AddObject(mat);
	///
	/// //Reinforcement material
	/// MatReinforcementEc2 matR = new MatReinforcementEc2();
	/// //...
	/// openModel.AddObject(matR);
	///
	/// //Cross-section without the bars
	/// //One component square 0.8x0.8m
	/// IdeaRS.OpenModel.CrossSection.CrossSectionComponent css = new IdeaRS.OpenModel.CrossSection.CrossSectionComponent();
	/// css.Name = "CSS1";
	/// 
	/// //Component of cross-section
	/// CssComponent comp = new CssComponent();
	/// comp.Material = new ReferenceElement(mat);
	/// comp.Phase = 0;
	/// 
	/// //Geometry of component 0.8x0.8
	/// Region2D region = new Region2D();
	/// PolyLine2D outline = new PolyLine2D();
	/// outline.StartPoint = new Point2D();
	/// outline.StartPoint.X = -0.04;
	/// outline.StartPoint.Y = -0.04;
	/// LineSegment2D seg = new LineSegment2D();
	/// seg.EndPoint = new Point2D();
	/// seg.EndPoint.X = 0.04;
	/// seg.EndPoint.Y = -0.04;
	/// outline.Segments.Add(seg);
	/// seg = new LineSegment2D();
	/// seg.EndPoint = new Point2D();
	/// seg.EndPoint.X = 0.04;
	/// seg.EndPoint.Y = 0.04;
	/// outline.Segments.Add(seg);
	/// seg = new LineSegment2D();
	/// seg.EndPoint = new Point2D();
	/// seg.EndPoint.X = -0.04;
	/// seg.EndPoint.Y = 0.04;
	/// outline.Segments.Add(seg);
	/// seg = new LineSegment2D();
	/// seg.EndPoint = new Point2D();
	/// seg.EndPoint.X = -0.04;
	/// seg.EndPoint.Y = -0.04;
	/// outline.Segments.Add(seg);
	/// region.Outline = outline;
	/// 
	/// //Optional - setting openning in this component 0,2x0,2
	/// PolyLine2D openning = new PolyLine2D();
	/// openning.StartPoint = new Point2D();
	/// openning.StartPoint.X = -0.01;
	/// openning.StartPoint.Y = -0.01;
	/// seg = new LineSegment2D();
	/// seg.EndPoint = new Point2D();
	/// seg.EndPoint.X = -0.01;
	/// seg.EndPoint.Y = 0.01;
	/// openning.Segments.Add(seg);
	/// seg = new LineSegment2D();
	/// seg.EndPoint = new Point2D();
	/// seg.EndPoint.X = 0.01;
	/// seg.EndPoint.Y = 0.01;
	/// openning.Segments.Add(seg);
	/// seg = new LineSegment2D();
	/// seg.EndPoint = new Point2D();
	/// seg.EndPoint.X = 0.01;
	/// seg.EndPoint.Y = -0.01;
	/// openning.Segments.Add(seg);
	/// seg = new LineSegment2D();
	/// seg.EndPoint = new Point2D();
	/// seg.EndPoint.X = -0.01;
	/// seg.EndPoint.Y = -0.01;
	/// openning.Segments.Add(seg);
	/// region.Openings.Add(openning);
	/// 
	/// comp.Geometry = region;
	/// css.Components.Add(comp);
	/// openModel.AddObject(css);
	/// 
	/// //Reinforced cross-section (references cross-section and adds the bars)
	/// //bar 8mm in each corner
	/// ReinforcedCrossSection rcs = new ReinforcedCrossSection();
	/// rcs.Name = "RCSS1";
	/// rcs.CrossSection = new ReferenceElement(css);
	/// 
	/// //One bar in the corner
	/// ReinforcedBar bar = new ReinforcedBar();
	/// bar.Diameter = 0.008;
	/// bar.Material = new ReferenceElement(matR);
	/// bar.Point = new Point2D();
	/// bar.Point.X = -0.025;
	/// bar.Point.Y = -0.025;
	/// rcs.Bars.Add(bar);
	/// 
	/// bar = new ReinforcedBar();
	/// bar.Diameter = 0.008;
	/// bar.Material = new ReferenceElement(matR);
	/// bar.Point = new Point2D();
	/// bar.Point.X = 0.025;
	/// bar.Point.Y = -0.025;
	/// rcs.Bars.Add(bar);
	/// 
	/// bar = new ReinforcedBar();
	/// bar.Diameter = 0.008;
	/// bar.Material = new ReferenceElement(matR);
	/// bar.Point = new Point2D();
	/// bar.Point.X = 0.025;
	/// bar.Point.Y = 0.025;
	/// rcs.Bars.Add(bar);
	/// 
	/// bar = new ReinforcedBar();
	/// bar.Diameter = 0.008;
	/// bar.Material = new ReferenceElement(matR);
	/// bar.Point = new Point2D();
	/// bar.Point.X = -0.025;
	/// bar.Point.Y = 0.025;
	/// rcs.Bars.Add(bar);
	/// 
	/// openModel.AddObject(rcs);
	/// </code>
	/// </example>
	[OpenModelClass("CI.Services.Concrete.ReinforcedSection.ReinfSection,CI.ReinforcedSection", "CI.Services.Concrete.ReinforcedSection.IReinforcedSection,CI.ServiceTypes")]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class ReinforcedCrossSection : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ReinforcedCrossSection()
		{
			Bars = new List<ReinforcedBar>();
			Stirrups = new List<Stirrup>();
			TendonBars = new List<TendonBar>();
			TendonDucts = new List<TendonDuct>();
		}

		/// <summary>
		/// Name of cross-section
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Cross-section
		/// </summary>
		public ReferenceElement CrossSection { get; set; }

		/// <summary>
		/// Reinforced bars
		/// </summary>
		public List<ReinforcedBar> Bars { get; set; }

		/// <summary>
		/// Stirrups
		/// </summary>
		public List<Stirrup> Stirrups { get; set; }

		/// <summary>
		/// Tendon bars
		/// </summary>
		public List<TendonBar> TendonBars { get; set; }

		/// <summary>
		/// Tendon ducts
		/// </summary>
		public List<TendonDuct> TendonDucts { get; set; }
	}
}