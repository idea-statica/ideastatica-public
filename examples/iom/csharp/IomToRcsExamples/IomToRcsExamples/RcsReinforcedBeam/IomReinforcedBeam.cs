using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Concrete.Load;
using IdeaRS.OpenModel.Concrete;
using IdeaRS.OpenModel.CrossSection;
using IdeaRS.OpenModel.Detail;
using IdeaRS.OpenModel.Geometry2D;
using IdeaRS.OpenModel.Material;
using IdeaRS.OpenModel.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;

namespace IomToRcsExamples
{
	public class IomReinforcedBeam : IRcsExample
	{
		public OpenModel BuildOpenModel()
		{
			//Create the OpenModel
			OpenModel openModel = new OpenModel();

			//## Create a new project
			//IOM data has to contain basic information of a new project, such as a project name, description, code type etc.

			//Common project data
			var projectData = new ProjectData();
			projectData.Name = "Column project";
			projectData.Date = new DateTime(2019, 6, 4);

			//Additionl data for Ec
			var projectDataEc = new ProjectDataEc();
			projectDataEc.AnnexCode = NationalAnnexCode.NoAnnex;
			projectDataEc.FatigueCheck = false;
			projectDataEc.FatigueAnnexNN = false;
			projectData.CodeDependentData = projectDataEc;

			openModel.ProjectData = projectData;

			//Concrete project data
			var projectDataConcrete = new ProjectDataConcreteEc2();
			projectDataConcrete.CodeEN1992_2 = false;
			projectDataConcrete.CodeEN1992_3 = false;
			openModel.ProjectDataConcrete = projectDataConcrete;


			//## Materials
			//To create a new project, these types of materials have to be defined:
			//-a new concrete material
			//Concrete material
			MatConcreteEc2 mat = new MatConcreteEc2();
			mat.Name = "C30/37";
			mat.UnitMass = 2500.0;
			mat.E = 32836.6e6;
			mat.G = 13667000000.0;
			mat.Poisson = 0.2;
			mat.SpecificHeat = 0.6;
			mat.ThermalExpansion = 0.00001;
			mat.ThermalConductivity = 45;
			mat.Fck = 30.0e6;
			mat.CalculateDependentValues = true;
			openModel.AddObject(mat);


			//- a new material of reinforcement
			//Reinforcement material
			MatReinforcementEc2 matR = new MatReinforcementEc2();
			matR.Name = "B 500B";
			matR.UnitMass = 7850.0;
			matR.E = 200e9;
			matR.Poisson = 0.2;
			matR.G = 83.333e9;
			matR.SpecificHeat = 0.6;
			matR.ThermalExpansion = 0.00001;
			matR.ThermalConductivity = 45;
			matR.Fyk = 500e6;
			matR.CoeffFtkByFyk = 1.08;
			matR.Epsuk = 0.025;
			matR.Type = ReinfType.Bars;
			matR.BarSurface = ReinfBarSurface.Ribbed;
			matR.Class = ReinfClass.B;
			matR.Fabrication = ReinfFabrication.HotRolled;
			matR.DiagramType = ReinfDiagramType.BilinerWithAnInclinedTopBranch;
			openModel.AddObject(matR);


			//## Cross-section
			//The next step is to define the shape and dimensions of cross-section and type of material.
			//After defining the concrete cross - section, reinforcement is set into this one.
			//The reinforced section is defined in this way and it is referenced to the concrete cross - section.
			CrossSectionParameter css = new CrossSectionParameter();
			css.Name = "CSS1";
			css.Id = openModel.GetMaxId(css) + 1;
			CrossSectionFactory.FillShapeT(css, 0.8, 0.7, 0.16, 0.25);
			openModel.AddObject(css);



			//## Reinforcement
			//Reinforcement is defined as stirrups and longitudinal bars.
			//Reinforced section - concrete with reinforcement
			ReinforcedCrossSection rcs = new ReinforcedCrossSection();
			rcs.Name = "R 1";
			rcs.CrossSection = new ReferenceElement(css);
			openModel.AddObject(rcs);


			//### Longitudinal reinforcement
			//Define position, material, diameter and quantity of longitudinal reinforcement.

			//Reinforced section - concrete with reinforcement
			ReinforcedBar bar = new ReinforcedBar();
			bar.Diameter = 0.012;
			bar.Material = new ReferenceElement(matR);
			bar.Point = new Point2D();
			bar.Point.X = 0.346;
			bar.Point.Y = 0.2157;
			rcs.Bars.Add(bar);

			bar = new ReinforcedBar();
			bar.Diameter = 0.012;
			bar.Material = new ReferenceElement(matR);
			bar.Point = new Point2D();
			bar.Point.X = 0.071;
			bar.Point.Y = 0.2157;
			rcs.Bars.Add(bar);

			bar = new ReinforcedBar();
			bar.Diameter = 0.012;
			bar.Material = new ReferenceElement(matR);
			bar.Point = new Point2D();
			bar.Point.X = -0.071;
			bar.Point.Y = 0.2157;
			rcs.Bars.Add(bar);

			bar = new ReinforcedBar();
			bar.Diameter = 0.012;
			bar.Material = new ReferenceElement(matR);
			bar.Point = new Point2D();
			bar.Point.X = -0.346;
			bar.Point.Y = 0.2157;
			rcs.Bars.Add(bar);

			bar = new ReinforcedBar();
			bar.Diameter = 0.020;
			bar.Material = new ReferenceElement(matR);
			bar.Point = new Point2D();
			bar.Point.X = -0.071;
			bar.Point.Y = -0.3923;
			rcs.Bars.Add(bar);

			bar = new ReinforcedBar();
			bar.Diameter = 0.020;
			bar.Material = new ReferenceElement(matR);
			bar.Point = new Point2D();
			bar.Point.X = 0.071;
			bar.Point.Y = -0.3923;
			rcs.Bars.Add(bar);

			bar = new ReinforcedBar();
			bar.Diameter = 0.012;
			bar.Material = new ReferenceElement(matR);
			bar.Point = new Point2D();
			bar.Point.X = -0.346;
			bar.Point.Y = 0.1437;
			rcs.Bars.Add(bar);

			bar = new ReinforcedBar();
			bar.Diameter = 0.012;
			bar.Material = new ReferenceElement(matR);
			bar.Point = new Point2D();
			bar.Point.X = 0.346;
			bar.Point.Y = 0.1437;
			rcs.Bars.Add(bar);

			bar = new ReinforcedBar();
			bar.Diameter = 0.012;
			bar.Material = new ReferenceElement(matR);
			bar.Point = new Point2D();
			bar.Point.X = -0.079;
			bar.Point.Y = -0.0063;
			rcs.Bars.Add(bar);

			bar = new ReinforcedBar();
			bar.Diameter = 0.012;
			bar.Material = new ReferenceElement(matR);
			bar.Point = new Point2D();
			bar.Point.X = -0.079;
			bar.Point.Y = -0.2343;
			rcs.Bars.Add(bar);

			bar = new ReinforcedBar();
			bar.Diameter = 0.012;
			bar.Material = new ReferenceElement(matR);
			bar.Point = new Point2D();
			bar.Point.X = 0.079;
			bar.Point.Y = -0.2343;
			rcs.Bars.Add(bar);

			bar = new ReinforcedBar();
			bar.Diameter = 0.012;
			bar.Material = new ReferenceElement(matR);
			bar.Point = new Point2D();
			bar.Point.X = 0.079;
			bar.Point.Y = -0.0063;
			rcs.Bars.Add(bar);

			//### Stirrups
			//Setting shape and material of stirrup.

			var stirrup = new Stirrup();
			stirrup.Diameter = 0.008;
			stirrup.DiameterOfMandrel = 4.0;
			stirrup.Distance = 0.15;
			stirrup.IsClosed = true;
			stirrup.Material = new ReferenceElement(matR);
			stirrup.ShearCheck = true;
			stirrup.TorsionCheck = true;
			var poly = new PolyLine2D();

			poly.StartPoint = new Point2D();
			poly.StartPoint.X = -0.091;
			poly.StartPoint.Y = 0.2257;

			var segment = new LineSegment2D();
			segment.EndPoint = new Point2D();
			segment.EndPoint.X = -0.091;
			segment.EndPoint.Y = -0.4063;
			poly.Segments.Add(segment);

			segment = new LineSegment2D();
			segment.EndPoint = new Point2D();
			segment.EndPoint.X = 0.091;
			segment.EndPoint.Y = -0.4063;
			poly.Segments.Add(segment);

			segment = new LineSegment2D();
			segment.EndPoint = new Point2D();
			segment.EndPoint.X = 0.091;
			segment.EndPoint.Y = 0.2257;
			poly.Segments.Add(segment);

			segment = new LineSegment2D();
			segment.EndPoint = new Point2D();
			segment.EndPoint.X = -0.091;
			segment.EndPoint.Y = 0.2257;
			poly.Segments.Add(segment);

			stirrup.Geometry = poly;
			rcs.Stirrups.Add(stirrup);

			stirrup = new Stirrup();
			stirrup.Diameter = 0.008;
			stirrup.DiameterOfMandrel = 4.0;
			stirrup.Distance = 0.15;
			stirrup.IsClosed = true;
			stirrup.ShearCheck = true;
			stirrup.TorsionCheck = false;
			stirrup.Material = new ReferenceElement(matR);
			poly = new PolyLine2D();

			poly.StartPoint = new Point2D();
			poly.StartPoint.X = -0.366;
			poly.StartPoint.Y = 0.2257;

			segment = new LineSegment2D();
			segment.EndPoint = new Point2D();
			segment.EndPoint.X = -0.366;
			segment.EndPoint.Y = 0.1337;
			poly.Segments.Add(segment);

			segment = new LineSegment2D();
			segment.EndPoint = new Point2D();
			segment.EndPoint.X = 0.366;
			segment.EndPoint.Y = 0.1337;
			poly.Segments.Add(segment);
			segment = new LineSegment2D();
			segment.EndPoint = new Point2D();
			segment.EndPoint.X = 0.366;
			segment.EndPoint.Y = 0.2257;
			poly.Segments.Add(segment);

			segment = new LineSegment2D();
			segment.EndPoint = new Point2D();
			segment.EndPoint.X = -0.366;
			segment.EndPoint.Y = 0.2257;
			poly.Segments.Add(segment);
			stirrup.Geometry = poly;
			rcs.Stirrups.Add(stirrup);


			//## Design member
			//Design member contains information about whole checked member.
			//In the first step, it is required to define design member and then the member data are set into the design member.

			//Check member == Design member in the RCS
			var checkMember = new CheckMember1D();
			openModel.AddObject(checkMember);


			//### Member data
			//etting of exposure classes, humidity and other important factors for the calculations (for example creep).

			//Concrete meber data
			var memberData = new ConcreteMemberDataEc2();
			memberData.MemberType = ConcreteMemberType.Beam;
			memberData.RelativeHumidity = 0.65;
			memberData.CreepCoeffInfinityValue = InputValue.Calculated;
			memberData.MemberImportance = MemberImportance.Major;

			memberData.ColumnData = new ColumnDataEc2();

			memberData.ExposureClassesData = new ExposureClassesDataEc2();
			memberData.ExposureClassesData.NoCorrosionCheck = false;
			memberData.ExposureClassesData.CarbonationCheck = true;
			memberData.ExposureClassesData.Carbonation = ExposureClassEc2.XC3;
			memberData.ExposureClassesData.ChloridesCheck = true;
			memberData.ExposureClassesData.Chlorides = ExposureClassEc2.XD1;
			memberData.ExposureClassesData.ChloridesFromSeaCheck = false;
			memberData.ExposureClassesData.FreezeAttackCheck = false;
			memberData.ExposureClassesData.ChemicalAttackCheck = false;

			memberData.Element = new ReferenceElement(checkMember);
			openModel.AddObject(memberData);

			//### Flectural slendeness
			//In this dialog, it is required to set clear distance between faces of the supports and support conditions to check deflection of the beam.

			memberData.BeamData = new BeamDataEc2();
			memberData.BeamData.Ln = 1.0;
			memberData.BeamData.TypeOfSupportLeft = TypeOfSupportConditions.NonContinuous;
			memberData.BeamData.TypeOfSupportRight = TypeOfSupportConditions.NonContinuous;
			memberData.BeamData.WidthOfSupportLeft = 0.4;
			memberData.BeamData.WidthOfSupportRight = 0.4;


			//## Sections, Extremes, Internal forces
			//The reinforced cross - section and the check member are defined for the checked section.
			//Extremes of internal forces (for ULS and SLS calculation) are set in the checked section data there.
			//For assessment of limit states, actual internal forces into the analyzed cross-section need to be insert.

			//Standard section
			var singleCheckSection = new StandardCheckSection();
			singleCheckSection.Description = "S 1";
			singleCheckSection.ReinfSection = new ReferenceElement(rcs);
			singleCheckSection.CheckMember = new ReferenceElement(checkMember);

			//add extreme to section
			var sectionExtreme = new StandardCheckSectionExtreme();
			//sectionExtreme.Name = "S1 - E1";
			sectionExtreme.Fundamental = new LoadingULS();
			sectionExtreme.Fundamental.InternalForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 150.0e3, Mz = 20.0e3, Qy = 5.0e3, Qz = 50.0e3 };

			sectionExtreme.Characteristic = new LoadingSLS();
			sectionExtreme.Characteristic.InternalForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 105.0e3, Mz = 14.0e3 };

			sectionExtreme.QuasiPermanent = new LoadingSLS();
			sectionExtreme.QuasiPermanent.InternalForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 80.0e3, Mz = 7.0e3 };
			singleCheckSection.Extremes.Add(sectionExtreme);

			openModel.AddObject(singleCheckSection);


			//## Calculation control
			//This setting define, which type of assessment will be used and corresponding results will be displayed.

			memberData.CalculationSetup = new CalculationSetup();
			memberData.CalculationSetup.UlsDiagram = true;
			memberData.CalculationSetup.UlsShear = true;
			memberData.CalculationSetup.UlsTorsion = true;
			memberData.CalculationSetup.UlsInteraction = true;
			memberData.CalculationSetup.SlsStressLimitation = true;
			memberData.CalculationSetup.SlsCrack = true;
			memberData.CalculationSetup.Detailing = true;
			memberData.CalculationSetup.UlsResponse = true;
			memberData.CalculationSetup.SlsStiffnesses = false;
			memberData.CalculationSetup.MNKappaDiagram = false;


			//## Concrete setup
			//Creating the code setup used for assessment of cross-section including national annex settings.

			//Concrete setup
			var setup = new ConcreteSetupEc2();
			setup.Annex = NationalAnnexCode.NoAnnex;
			openModel.ConcreteSetup = setup;

			return openModel;

		}
	}
}
