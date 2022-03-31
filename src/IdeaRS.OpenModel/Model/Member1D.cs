using System.Collections.Generic;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Type of Member 1D
	/// </summary>
	public enum Member1DType
	{
		/// <summary>
		/// Beam member
		/// </summary>
		Beam,

		/// <summary>
		/// Column member
		/// </summary>
		Column,

		/// <summary>
		/// Truss member
		/// </summary>
		Truss,

		/// <summary>
		/// Rib member
		/// </summary>
		Rib,

		/// <summary>
		/// Beam slab, rcs element
		/// </summary>
		BeamSlab,
	}

	/// <summary>
	/// Representation of member1D
	/// </summary>
	/// <example>
	/// This sample shows how to create a member 1D .
	/// <code lang = "C#">
	/// //Creating the model
	/// OpenModel openModel = new OpenModel();
	///
	/// //Create nodes
	/// Point3D pointA = new Point3D();
	/// pointA.X = 0;
	/// pointA.Y = 0;
	/// pointA.Z = 0;
	/// openModel.AddObject(pointA);
	///
	/// Point3D pointB = new Point3D();
	/// pointB.X = 0;
	/// pointB.Y = 0;
	/// pointB.Z = 1.2;
	/// openModel.AddObject(pointB);
	///
	/// Point3D pointC = new Point3D();
	/// pointC.X = 0;
	/// pointC.Y = 0;
	/// pointC.Z = 2.4;
	/// openModel.AddObject(pointC);
	///
	/// //Line between nodes
	/// LineSegment3D line1 = new LineSegment3D();
	/// line1.StartPoint = new ReferenceElement(pointA);
	/// line1.EndPoint = new ReferenceElement(pointB);
	/// //LCS of line
	/// line1.LocalCoordinateSystem = new CoordSystemByZup();
	/// openModel.AddObject(line1);
	///
	/// //Line between nodes
	/// LineSegment3D line2 = new LineSegment3D();
	/// line2.StartPoint = new ReferenceElement(pointB);
	/// line2.EndPoint = new ReferenceElement(pointC);
	/// //LCS of line
	/// line2.LocalCoordinateSystem = new CoordSystemByZup();
	/// openModel.AddObject(line2);
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
	/// IdeaRS.OpenModel.CrossSection.CrossSectionComponent css = new IdeaRS.OpenModel.CrossSection.CrossSectionComponent();
	/// //...
	/// openModel.AddObject(css);
	///
	/// //Reinforced cross-section (referemnces cross-section and ads the bars)
	/// ReinforcedCrossSection rcs = new ReinforcedCrossSection();
	/// //...
	/// openModel.AddObject(rcs);
	///
	/// //Model element along the "line1" - part of member1D
	/// Element1D element1D1 = new Element1D();
	/// element1D1.CrossSectionBegin = new ReferenceElement(rcs);
	/// element1D1.CrossSectionEnd = new ReferenceElement(rcs);
	/// element1D1.Segment = new ReferenceElement(line1);
	/// openModel.AddObject(element1D1);
	///
	/// //Model element along the "line2" - part of member1D
	/// Element1D element1D2 = new Element1D();
	/// element1D2.CrossSectionBegin = new ReferenceElement(rcs);
	/// element1D2.CrossSectionEnd = new ReferenceElement(rcs);
	/// element1D2.Segment = new ReferenceElement(line2);
	/// openModel.AddObject(element1D2);
	///
	/// //Model member assembles element1D
	/// Member1D member1D = new Member1D();
	/// member1D.Elements1D.Add(new ReferenceElement(element1D1));
	/// member1D.Elements1D.Add(new ReferenceElement(element1D2));
	/// member1D.Member1DType = Member1DType.Column;
	/// member1D.Name = "COLUMN1";
	/// openModel.AddObject(member1D);
	/// </code>
	/// </example>
	[OpenModelClass("CI.StructModel.Structure.Member1D,CI.StructuralElements", "CI.StructModel.Structure.IMember1D,CI.BasicTypes")]
	public class Member1D : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Member1D()
		{
			Elements1D = new List<ReferenceElement>();
		}

		/// <summary>
		/// Name of Element
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Array of element1D
		/// </summary>
		/// <returns>List of Element1D</returns>
		public List<ReferenceElement> Elements1D { get; set; }

		/// <summary>
		/// Beam, column,...
		/// </summary>
		public Member1DType Member1DType { get; set; }

		/// <summary>
		/// Gets, sets hinge located in the begining of element
		/// </summary>
		public ReferenceElement HingeBegin { get; set; }

		/// <summary>
		/// Gets, sets hinge located in the end of element
		/// </summary>
		public ReferenceElement HingeEnd { get; set; }
	}
}