using IdeaRS.OpenModel.Geometry2D;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// Cross-section defined by components
	/// </summary>
	/// <example> 
	/// This sample shows how to create a cross-section defined by components.
	/// <code lang = "C#">
	/// //Creating the model
	/// OpenModel openModel = new OpenModel();
	/// 
	/// //Cocrete material
	/// MatConcreteEc2 mat = new MatConcreteEc2();
	/// //...
	/// openModel.AddObject(mat);
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
	/// </code>
	/// </example>
	[OpenModelClass("CI.StructModel.Libraries.CrossSection.CrossSection,CI.CrossSection", "CI.StructModel.Libraries.CrossSection.ICrossSection,CI.BasicTypes", typeof(CrossSection))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class CrossSectionComponent : CrossSection
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CrossSectionComponent()
		{
			Components = new List<CssComponent>();
		}

		/// <summary>
		/// List of components
		/// </summary>
		public List<CssComponent> Components { get; set; }
	}

}