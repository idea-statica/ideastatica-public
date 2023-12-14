# IOM Example - Prestressed Beam

This example describes how to define a prestressed beam in IOM (IDEA StatiCa Open Model).


Let's create a standard console application in MS Visual Studio. Select __File__ > __New__ > __Project__ from the menu bar. In the dialog, select the __Visual C#__ node followed by the __Get Started__ node. Then select the __Console App__ project template.

## Add the IdeaRS.OpenModel NuGet package

OpenModel is published as [the nuget package](https://www.nuget.org/packages/IdeaStatiCa.OpenModel/). To install this package, you can use either the Package Manager UI or the Package Manager Console.

For more information, see [Install and use a package in Visual Studio](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio)

There is also documentation related to [IdeaRS.OpenModel](https://idea-statica.github.io/ideastatica-public/docs/latest/api-iom/index.html) on Github.

## Create a new project
IOM data has to contain basic information of a new project, such as a project name, description, code type etc.
```csharp
//Common project data
var projectData = new ProjectData();
projectData.Name = "Prestressed beam";
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
```

![alt text][projdata]


## Materials
To create a new project, these types of materials have to be defined:
-	a new concrete material
```csharp
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
mat.Fck = Conversions.MPaToSystem(30.0);
mat.CalculateDependentValues = true;
openModel.AddObject(mat);
```

![alt text][concreteprop]


-   a new material of reinforcement
```csharp
//Reinforcement material
MatReinforcementEc2 matR = new MatReinforcementEc2();
matR.Name = "B 500B";
matR.UnitMass = 7850.0;
matR.E = Conversions.GPaToSystem(200);
matR.Poisson = 0.2;
matR.G = Conversions.GPaToSystem(83.333);
matR.SpecificHeat = 0.6;
matR.ThermalExpansion = 0.00001;
matR.ThermalConductivity = 45;
matR.Fyk = Conversions.MPaToSystem(500);
matR.CoeffFtkByFyk = 1.08;
matR.Epsuk = 0.025;
matR.Type = ReinfType.Bars;
matR.BarSurface = ReinfBarSurface.Ribbed;
matR.Class = ReinfClass.B;
matR.Fabrication = ReinfFabrication.HotRolled;
matR.DiagramType = ReinfDiagramType.BilinerWithAnInclinedTopBranch;
openModel.AddObject(matR);
```

![alt text][reinforcementprop]


-   a new material of prestressing reinforcement
```csharp
//Prestressing material
MatPrestressSteelEc2 matP = new MatPrestressSteelEc2();
matP.Name = "Y1860S7-15.7";
matP.UnitMass = 7850.0;
matP.E = Conversions.GPaToSystem(195);
matP.Poisson = 0.2;
matP.G = Conversions.GPaToSystem(83.333);
matP.SpecificHeat = 0.6;
matP.ThermalExpansion = 0.00001;
matP.ThermalConductivity = 45;
matP.Diameter = 0.0157;
matP.Area = 0.000150;
matP.NumberOfWires = 7;
matP.Fpk = Conversions.MPaToSystem(1860);
matP.Fp01k = Conversions.MPaToSystem(1640);
matP.Epsuk = 0.035;
matP.Type = PrestressSteelType.Strand;
matP.SurfaceCharacteristic = SurfaceCharacteristicType.Plain;
matP.Production = ProductionType.LowRelaxation;
matP.DiagramType = ReinfDiagramType.BilinerWithAnInclinedTopBranch;
openModel.AddObject(matP);
```

![alt text][prestressingreinforcementprop]


## Cross-section
The next step is to define the shape and dimensions of cross-section and type of material.
```csharp
CrossSectionParameter css = new CrossSectionParameter();
css.Name = "S 1";
css.Id = openModel.GetMaxId(css) + 1;

css.CrossSectionType = CrossSectionType.BeamShapeIHaunchChamferAssym;
css.Parameters.Add(new ParameterDouble() { Name = "Bbf", Value = 0.4 });
css.Parameters.Add(new ParameterDouble() { Name = "Hbf", Value = 0.2 });
css.Parameters.Add(new ParameterDouble() { Name = "Hbfh", Value = 0.075 });
css.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = 0.2 });
css.Parameters.Add(new ParameterDouble() { Name = "Bwh", Value = 0.05 });
css.Parameters.Add(new ParameterDouble() { Name = "H", Value = 1.2 });
css.Parameters.Add(new ParameterDouble() { Name = "Htfh", Value = 0.05 });
css.Parameters.Add(new ParameterDouble() { Name = "Htf", Value = 0.175 });
css.Parameters.Add(new ParameterDouble() { Name = "Btfl", Value = 0.5 });
css.Parameters.Add(new ParameterDouble() { Name = "Btfr", Value = 0.5 });
openModel.AddObject(css);
```

![alt text][cross-section]


## Reinforced cross-sections
After defining the concrete cross-section, reinforcement is set into this one. The reinforced section is defined in this way and it is referenced to the concrete cross-section.
```csharp
//Reinforced section - concrete with reinforcement
ReinforcedCrossSection rcs = new ReinforcedCrossSection();
rcs.Name = "RS 1";
rcs.CrossSection = new ReferenceElement(css);
```

![alt text][rcs]


## Reinforcement
Reinforcement is defined as stirrups and longitudinal bars.
```csharp
```

![alt text][reinforcement]

### Longitudinal reinforcement
Define position, material, diameter and quantity of longitudinal reinforcement.
```csharp
//Reinforcement
ReinforcedBar bar = new ReinforcedBar();
bar.Diameter = 0.025;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = -0.146;
bar.Point.Y = -0.7088;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.025;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = -0.046;
bar.Point.Y = -0.7088;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.025;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = 0.046;
bar.Point.Y = -0.7088;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.025;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = 0.146;
bar.Point.Y = -0.7088;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.020;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = -0.150;
bar.Point.Y = -0.5893;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.020;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = 0.150;
bar.Point.Y = -0.5893;
rcs.Bars.Add(bar);


bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = -0.056;
bar.Point.Y = 0.2847;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = -0.056;
bar.Point.Y = 0.094;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = -0.056;
bar.Point.Y = -0.0968;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = -0.056;
bar.Point.Y = -0.2876;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = -0.056;
bar.Point.Y = -0.4783;
rcs.Bars.Add(bar);


bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = 0.056;
bar.Point.Y = -0.4783;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = 0.056;
bar.Point.Y = -0.2876;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = 0.056;
bar.Point.Y = -0.0968;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = 0.056;
bar.Point.Y = 0.094;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = 0.056;
bar.Point.Y = 0.2847;
rcs.Bars.Add(bar);


bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = 0.596;
bar.Point.Y = 0.3967;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = 0.321;
bar.Point.Y = 0.3967;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = 0.046;
bar.Point.Y = 0.3967;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = -0.046;
bar.Point.Y = 0.3967;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = -0.321;
bar.Point.Y = 0.3967;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = -0.596;
bar.Point.Y = 0.3967;
rcs.Bars.Add(bar);


bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = -0.599;
bar.Point.Y = 0.3097;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = -0.324;
bar.Point.Y = 0.3097;
rcs.Bars.Add(bar);


bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = 0.324;
bar.Point.Y = 0.3097;
rcs.Bars.Add(bar);

bar = new ReinforcedBar();
bar.Diameter = 0.012;
bar.Material = new ReferenceElement(matR);
bar.Point = new Point2D();
bar.Point.X = 0.599;
bar.Point.Y = 0.3097;
rcs.Bars.Add(bar);
```

![alt text][longreinforcement]

### Stirrups
Setting shape and material of stirrup.
```csharp
var stirrup = new Stirrup();
stirrup.Diameter = 0.008;
stirrup.DiameterOfMandrel = 4.0;
stirrup.Distance = 0.2;
stirrup.IsClosed = true;
stirrup.Material = new ReferenceElement(matR);
stirrup.ShearCheck = true;
stirrup.TorsionCheck = true;
var poly = new PolyLine2D();

poly.StartPoint = new Point2D();
poly.StartPoint.X = -0.066;
poly.StartPoint.Y = 0.4067;

var segment = new LineSegment2D();
segment.EndPoint = new Point2D();
segment.EndPoint.X = -0.066;
segment.EndPoint.Y = -0.7253;
poly.Segments.Add(segment);

segment = new LineSegment2D();
segment.EndPoint = new Point2D();
segment.EndPoint.X = 0.066;
segment.EndPoint.Y = -0.7253;
poly.Segments.Add(segment);

segment = new LineSegment2D();
segment.EndPoint = new Point2D();
segment.EndPoint.X = 0.066;
segment.EndPoint.Y = 0.4067;
poly.Segments.Add(segment);

segment = new LineSegment2D();
segment.EndPoint = new Point2D();
segment.EndPoint.X = -0.066;
segment.EndPoint.Y = 0.4067;
poly.Segments.Add(segment);

stirrup.Geometry = poly;
rcs.Stirrups.Add(stirrup);

stirrup = new Stirrup();
stirrup.Diameter = 0.008;
stirrup.DiameterOfMandrel = 4.0;
stirrup.Distance = 0.2;
stirrup.IsClosed = true;
stirrup.ShearCheck = false;
stirrup.TorsionCheck = false;
stirrup.Material = new ReferenceElement(matR);
poly = new PolyLine2D();

poly.StartPoint = new Point2D();
poly.StartPoint.X = -0.616;
poly.StartPoint.Y = 0.4067;

segment = new LineSegment2D();
segment.EndPoint = new Point2D();
segment.EndPoint.X = 0.616;
segment.EndPoint.Y = 0.4067;
poly.Segments.Add(segment);

segment = new LineSegment2D();
segment.EndPoint = new Point2D();
segment.EndPoint.X = 0.616;
segment.EndPoint.Y = 0.2997;
poly.Segments.Add(segment);

segment = new LineSegment2D();
segment.EndPoint = new Point2D();
segment.EndPoint.X = -0.616;
segment.EndPoint.Y = 0.2997;
poly.Segments.Add(segment);

segment = new LineSegment2D();
segment.EndPoint = new Point2D();
segment.EndPoint.X = -0.616;
segment.EndPoint.Y = 0.4067;
poly.Segments.Add(segment);

stirrup.Geometry = poly;
rcs.Stirrups.Add(stirrup);


stirrup = new Stirrup();
stirrup.Diameter = 0.008;
stirrup.DiameterOfMandrel = 4.0;
stirrup.Distance = 0.2;
stirrup.IsClosed = false;
stirrup.ShearCheck = false;
stirrup.TorsionCheck = false;
stirrup.Material = new ReferenceElement(matR);
poly = new PolyLine2D();

poly.StartPoint = new Point2D();
poly.StartPoint.X = 0.066;
poly.StartPoint.Y = -0.4023;

segment = new LineSegment2D();
segment.EndPoint = new Point2D();
segment.EndPoint.X = -0.166;
segment.EndPoint.Y = -0.5763;
poly.Segments.Add(segment);

segment = new LineSegment2D();
segment.EndPoint = new Point2D();
segment.EndPoint.X = -0.166;
segment.EndPoint.Y = -0.7253;
poly.Segments.Add(segment);

segment = new LineSegment2D();
segment.EndPoint = new Point2D();
segment.EndPoint.X = 0.166;
segment.EndPoint.Y = -0.7253;
poly.Segments.Add(segment);

segment = new LineSegment2D();
segment.EndPoint = new Point2D();
segment.EndPoint.X = 0.166;
segment.EndPoint.Y = -0.5763;
poly.Segments.Add(segment);

segment = new LineSegment2D();
segment.EndPoint = new Point2D();
segment.EndPoint.X = -0.066;
segment.EndPoint.Y = -0.4023;

poly.Segments.Add(segment);
stirrup.Geometry = poly;
rcs.Stirrups.Add(stirrup);
openModel.AddObject(rcs);
```

![alt text][stirrups]


## Prestressing
For prestressed member, it is necessary to set position and type of tendons.
Material and quantity of strands in tendon, material and diameter of ducts is need to be set.
```csharp
//Tendon
TendonBar tendonBar = new TendonBar();
tendonBar.Material = new ReferenceElement(matP);
tendonBar.NumStrandsInTendon = 5;
tendonBar.Phase = 0;
tendonBar.PrestressingOrder = 1;
tendonBar.PrestressReinforcementType = FatigueTypeOfPrestressingSteel.PostTensioningSingleStrandsInPlasticDucts;
tendonBar.TendonType = TendonBarType.Posttensioned;
tendonBar.Id = 1;
tendonBar.Point = new Point2D() { X = 0.0, Y = -0.6493 };
rcs.TendonBars.Add(tendonBar);

TendonDuct tendonDuct = new TendonDuct();
tendonDuct.Diameter = 0.05;
tendonDuct.MaterialDuct = MaterialDuct.Plastic;
tendonDuct.Id = 1;
tendonDuct.Point = new Point2D() { X = 0.0, Y = -0.6493 };
tendonBar.TendonDuct = tendonDuct;
```

![alt text][prestressing]


## Design member
Design member contains information about whole checked member. In the first step, it is required to define design member and then the member data are set into the design member.
```csharp
//Check member == Design member in the RCS
var checkMember = new CheckMember1D();
openModel.AddObject(checkMember);
```

### Member data
Setting of exposure classes, humidity and other important factors for the calculations (for example creep).
```csharp
//Concrete meber data
var memberData = new ConcreteMemberDataEc2();
memberData.Element = new ReferenceElement(checkMember);
openModel.AddObject(memberData);

memberData.MemberType = ConcreteMemberType.Beam;
memberData.RelativeHumidity = 0.65;
memberData.CreepCoeffInfinityValue = InputValue.Calculated;
memberData.MemberImportance = MemberImportance.Major;

memberData.CalculationSetup = new CalculationSetup();
memberData.CalculationSetup.UlsDiagram = true;
memberData.CalculationSetup.UlsShear = true;
memberData.CalculationSetup.UlsTorsion = true;
memberData.CalculationSetup.UlsInteraction = true;
memberData.CalculationSetup.SlsStressLimitation = true;
memberData.CalculationSetup.SlsCrack = true;
memberData.CalculationSetup.Detailing = true;
memberData.CalculationSetup.UlsResponse = false;
memberData.CalculationSetup.SlsStiffnesses = false;
memberData.CalculationSetup.MNKappaDiagram = false;

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

memberData.BeamData = new BeamDataEc2();
memberData.BeamData.Ln = 1.0;
memberData.BeamData.TypeOfSupportLeft = TypeOfSupportConditions.NonContinuous;
memberData.BeamData.TypeOfSupportRight = TypeOfSupportConditions.NonContinuous;
memberData.BeamData.WidthOfSupportLeft = 0.4;
memberData.BeamData.WidthOfSupportRight = 0.4;
```

![alt text][member data]

### Construction stages
In this tab, it is necessary to set each time of construction stages to calculate creep, loses in tendons etc. It is very important to set time of prestressing for prestressed members.
```csharp
memberData.TimeAxis = new TimeAxis();

TimePoint tp = new TimePoint();
tp.Age = 0.0;
tp.Name = "Casting";
tp.Stage = true;
tp.Prestressing = false;
tp.Time = Conversions.DaysToSystem(0.0);
memberData.TimeAxis.Times.Add(tp);

tp = new TimePoint();
tp.Age = 0.0;
tp.Name = "Post-tensioning";
tp.Stage = false;
tp.Prestressing = true;
tp.Time = Conversions.DaysToSystem(28.0);
memberData.TimeAxis.Times.Add(tp);

tp = new TimePoint();
tp.Age = 0.0;
tp.Name = "Superimposed dead load";
tp.Stage = false;
tp.Prestressing = false;
tp.Time = Conversions.DaysToSystem(60.0);
memberData.TimeAxis.Times.Add(tp);

tp = new TimePoint();
tp.Age = 0.0;
tp.Name = "End of design working life";
tp.Stage = false;
tp.Prestressing = false;
tp.Time = Conversions.DaysToSystem(18250.0);
memberData.TimeAxis.Times.Add(tp);
```

![alt text][constructionstages]


## Sections, Extremes, Internal forces
The reinforced cross-section and the check member are defined for the checked section. 
Extremes of internal forces (for ULS and SLS calculation) are set in to the checked section data there.

For assessment of limit states, actual internal forces into the analyzed cross-section need to be insert.
Effect of prestressing is calculated automatically.
```csharp
//Staged section
var stagedCheckSection = new StagedCheckSection();
stagedCheckSection.Description = "Section 1";
stagedCheckSection.ReinfSection = new ReferenceElement(rcs);
stagedCheckSection.CheckMember = new ReferenceElement(checkMember);
openModel.AddObject(stagedCheckSection);
```
![alt text][sections]


## Action stages
In the next step it is required to insert initial state of cross-section. It is defined as total effects of characteristic permanent load, prestressing, and creep and shrinkage of concrete related to origin of coordinate systém. In case of statically indeterminate structure is necessary to insert secondary effects of prestressing.
```csharp
//Stages
var stagesLoading = new StagesLoading();
stagedCheckSection.StagesLoading = stagesLoading;

stagesLoading.PrestressInputType = PrestressInputType.AfterLongTermLosses;
stagesLoading.CssEffectType = CssEffectType.InternalForce;

//component loading
var cssComponentLoading = new CssComponentLoading();
cssComponentLoading.Id = 1;
var cssComponentTimeLoading = new CssComponentTimeLoading();
cssComponentTimeLoading.Time = Conversions.DaysToSystem(28.0);
cssComponentTimeLoading.Loading = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = -996.4e3, My = -317.7e3 };
cssComponentLoading.Loading.Add(cssComponentTimeLoading);

cssComponentTimeLoading = new CssComponentTimeLoading();
cssComponentTimeLoading.Time = Conversions.DaysToSystem(60.0);
cssComponentTimeLoading.Loading = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = -996.1e3, My = -32.7e3 };
cssComponentLoading.Loading.Add(cssComponentTimeLoading);

cssComponentTimeLoading = new CssComponentTimeLoading();
cssComponentTimeLoading.Time = Conversions.DaysToSystem(18250.0);
cssComponentTimeLoading.Loading = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = -846.8e3, My = 64.3e3 };
cssComponentLoading.Loading.Add(cssComponentTimeLoading);
stagesLoading.CssComponentLoad.Add(cssComponentLoading);

//prestress loading
var prestressLoading = new PrestressLoading();
prestressLoading.Time = Conversions.DaysToSystem(28.0);
prestressLoading.PrimaryForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces();
prestressLoading.SecondaryForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = -0.1e3, My = -3.6e3 };
stagesLoading.PrestressLoad.Add(prestressLoading);

prestressLoading = new PrestressLoading();
prestressLoading.Time = Conversions.DaysToSystem(60.0);
prestressLoading.PrimaryForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces();
prestressLoading.SecondaryForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 0.0 };
stagesLoading.PrestressLoad.Add(prestressLoading);

prestressLoading = new PrestressLoading();
prestressLoading.Time = Conversions.DaysToSystem(18250.0);
prestressLoading.PrimaryForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces();
prestressLoading.SecondaryForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 0.0 };
stagesLoading.PrestressLoad.Add(prestressLoading);

var tendonComponentLoading = new TendonComponentLoading();
tendonComponentLoading.Id = 1;
var tendonStressLoading = new TendonStressLoading();

var tendonTimeStressLoading = new TendonTimeStressLoading();
tendonTimeStressLoading.Time = Conversions.DaysToSystem(28.0);
tendonTimeStressLoading.Stress = Conversions.MPaToSystem(1328.4);
tendonStressLoading.Stresses.Add(tendonTimeStressLoading);

tendonTimeStressLoading = new TendonTimeStressLoading();
tendonTimeStressLoading.Time = Conversions.DaysToSystem(60.0);
tendonTimeStressLoading.Stress = Conversions.MPaToSystem(1328.1);
tendonStressLoading.Stresses.Add(tendonTimeStressLoading);

tendonTimeStressLoading = new TendonTimeStressLoading();
tendonTimeStressLoading.Time = Conversions.DaysToSystem(18250.0);
tendonTimeStressLoading.Stress = Conversions.MPaToSystem(1129.1);
tendonStressLoading.Stresses.Add(tendonTimeStressLoading);

tendonComponentLoading.Loading = tendonStressLoading;
stagesLoading.TendonComponentLoad.Add(tendonComponentLoading);
```

![alt text][actionstages]

Creating extremes
```csharp
//add extreme to section
var sectionExtreme = new StagedCheckSectionExtreme();
sectionExtreme.Time = Conversions.DaysToSystem(28.0);
sectionExtreme.Fundamental = new LoadingULS();
sectionExtreme.Fundamental.InternalForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 332.8e3, Qz = 0.0 };
sectionExtreme.Fundamental.InternalForcesVariable = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 0.0, Qz = 0.0 };
sectionExtreme.Characteristic = new LoadingSLS();
sectionExtreme.Characteristic.InternalForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 332.8e3, Qz = 0.0 };
sectionExtreme.Characteristic.InternalForcesVariable = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 0.0, Qz = 0.0 };
sectionExtreme.Frequent = new LoadingSLS();
sectionExtreme.Frequent.InternalForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 332.8e3, Qz = 0.0 };
sectionExtreme.Frequent.InternalForcesVariable = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 0.0, Qz = 0.0 };
sectionExtreme.QuasiPermanent = new LoadingSLS();
sectionExtreme.QuasiPermanent.InternalForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 332.8e3, Qz = 0.0 };
sectionExtreme.QuasiPermanent.InternalForcesVariable = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 0.0, Qz = 0.0 };
stagedCheckSection.Extremes.Add(sectionExtreme);

sectionExtreme = new StagedCheckSectionExtreme();
sectionExtreme.Time = Conversions.DaysToSystem(28.0);
sectionExtreme.Fundamental = new LoadingULS();
sectionExtreme.Fundamental.InternalForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 449.3e3, Qz = 0.0 };
sectionExtreme.Fundamental.InternalForcesVariable = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 0.0, Qz = 0.0 };
stagedCheckSection.Extremes.Add(sectionExtreme);

sectionExtreme = new StagedCheckSectionExtreme();
sectionExtreme.Time = Conversions.DaysToSystem(18250.0);
sectionExtreme.Fundamental = new LoadingULS();
sectionExtreme.Fundamental.InternalForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 614.1e3, Qz = 0.0 };
sectionExtreme.Fundamental.InternalForcesVariable = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 0.0, Qz = 0.0 };
sectionExtreme.Characteristic = new LoadingSLS();
sectionExtreme.Characteristic.InternalForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 614.1e3, Qz = 0.0 };
sectionExtreme.Characteristic.InternalForcesVariable = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 0.0, Qz = 0.0 };
sectionExtreme.Frequent = new LoadingSLS();
sectionExtreme.Frequent.InternalForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 614.1e3, Qz = 0.0 };
sectionExtreme.Frequent.InternalForcesVariable = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 0.0, Qz = 0.0 };
sectionExtreme.QuasiPermanent = new LoadingSLS();
sectionExtreme.QuasiPermanent.InternalForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 614.1e3, Qz = 0.0 };
sectionExtreme.QuasiPermanent.InternalForcesVariable = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 0.0, Qz = 0.0 };
stagedCheckSection.Extremes.Add(sectionExtreme);

sectionExtreme = new StagedCheckSectionExtreme();
sectionExtreme.Time = Conversions.DaysToSystem(18250.0);
sectionExtreme.Fundamental = new LoadingULS();
sectionExtreme.Fundamental.InternalForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 704.6e3, Qz = 0.0 };
sectionExtreme.Fundamental.InternalForcesVariable = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 632.8e3, Qz = 0.0 };
sectionExtreme.Characteristic = new LoadingSLS();
sectionExtreme.Characteristic.InternalForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 614.1e3, Qz = 0.0 };
sectionExtreme.Characteristic.InternalForcesVariable = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 421.9e3, Qz = 0.0 };
sectionExtreme.Frequent = new LoadingSLS();
sectionExtreme.Frequent.InternalForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 614.1e3, Qz = 0.0 };
sectionExtreme.Frequent.InternalForcesVariable = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 210.9e3, Qz = 0.0 };
sectionExtreme.QuasiPermanent = new LoadingSLS();
sectionExtreme.QuasiPermanent.InternalForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 614.1e3, Qz = 0.0 };
sectionExtreme.QuasiPermanent.InternalForcesVariable = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 126.6e3, Qz = 0.0 };
stagedCheckSection.Extremes.Add(sectionExtreme);

sectionExtreme = new StagedCheckSectionExtreme();
sectionExtreme.Time = Conversions.DaysToSystem(18250.0);
sectionExtreme.Fundamental = new LoadingULS();
sectionExtreme.Fundamental.InternalForces = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 614.1e3, Qz = 0.0 };
sectionExtreme.Fundamental.InternalForcesVariable = new IdeaRS.OpenModel.Result.ResultOfInternalForces() { N = 0.0, My = 632.8e3, Qz = 0.0 };
stagedCheckSection.Extremes.Add(sectionExtreme);
```

![alt text][forces]


## Calculation control
This setting define, which type of assessment will be used and corresponding results will be displayed.
```csharp
memberData.CalculationSetup = new CalculationSetup();
memberData.CalculationSetup.UlsDiagram = true;
memberData.CalculationSetup.UlsShear = true;
memberData.CalculationSetup.UlsTorsion = true;
memberData.CalculationSetup.UlsInteraction = true;
memberData.CalculationSetup.SlsStressLimitation = true;
memberData.CalculationSetup.SlsCrack = true;
memberData.CalculationSetup.Detailing = true;
memberData.CalculationSetup.UlsResponse = false;
memberData.CalculationSetup.SlsStiffnesses = false;
memberData.CalculationSetup.MNKappaDiagram = false;
```

![alt text][calccontrol]


## Concrete setup
Creating the code setup used for assessment of the cross-section including national annex settings.
```csharp
//Concrete setup
var setup = new ConcreteSetupEc2();
setup.Annex = NationalAnnexCode.NoAnnex;
openModel.ConcreteSetup = setup;
```

![alt text][concretesetup]

## Results
In the followed example there is way how to run the check and the get results. Results are stored in the object with considered values for each assessment.
```csharp
//Creating instance of Rcs controller
var rcsController = new IdeaStatiCa.RcsController.IdeaRcsController();

//Open rcs project from IOM
IdeaRS.OpenModel.Message.OpenMessages messages;
var ok = rcsController.OpenIdeaProjectFromIdeaOpenModel(openModel, "ProjectName", out messages);

//Calculate project
ok = rcsController.Calculate(null);
//gets the results
var result = rcsController.GetResultOnSection(null);

var sectionResult = result.FirstOrDefault(it => it.SectionId == stagedCheckSection.Id);
foreach (var extremeResult in sectionResult.ExtremeResults)
{
    foreach (var checkResult in extremeResult.CheckResults)
    {
        var checkType = checkResult.ResultType;
        foreach (var checkResult1 in checkResult.CheckResults)
        {
            var res = checkResult1.Result;

            switch (checkResult.ResultType)
            {
                case IdeaRS.OpenModel.Concrete.CheckResult.CheckResultType.Capacity:
                    var resultCapacity = checkResult1 as IdeaRS.OpenModel.Concrete.CheckResult.ConcreteULSCheckResultDiagramCapacityEc2;
                    var fu1 = resultCapacity.Fu1;
                    var fu2 = resultCapacity.Fu2;
                    //....
                    break;

                case IdeaRS.OpenModel.Concrete.CheckResult.CheckResultType.Interaction:
                    var resultInteraction = checkResult1 as IdeaRS.OpenModel.Concrete.CheckResult.ConcreteULSCheckResultInteractionEc2;
                    var checkVT = resultInteraction.CheckValueShearAndTorsion;
                    var checkVTB = resultInteraction.CheckValueShearTorsionAndBending;
                    //....
                    break;

                case IdeaRS.OpenModel.Concrete.CheckResult.CheckResultType.Shear:
                    var resultShear = checkResult1 as IdeaRS.OpenModel.Concrete.CheckResult.ConcreteULSCheckResultShearEc2;
                    var v_ed = resultShear.Ved;
                    var v_rdc = resultShear.Vrdc;
                    //....
                    break;

                case IdeaRS.OpenModel.Concrete.CheckResult.CheckResultType.CrackWidth:
                    var resultCrackWidth = checkResult1 as IdeaRS.OpenModel.Concrete.CheckResult.ConcreteSLSCheckResultCrackWidthEc2;
                    var w = resultCrackWidth.W;
                    var wlim = resultCrackWidth.Wlim;
                    //....
                    break;
            }

            if (checkResult1.NonConformities.Count > 0)
            {
                var issues = rcsController.GetNonConformityIssues(checkResult1.NonConformities.Select(it => it.Guid).ToList());
                foreach (var issue in issues)
                {
                    var text = issue.Description;
                    //....
                }
            }
        }
    }
}

rcsController.Dispose();
```

![alt text][results]



[projdata]: ../images/StagedBeam/1.PNG "Project data"
[concreteprop]: ../images/StagedBeam/16.PNG "Concrete"
[reinforcementprop]: ../images/StagedBeam/17.PNG "Reinforcement"
[prestressingreinforcementprop]: ../images/StagedBeam/18.PNG "Prestressing reinforcement"
[cross-section]: ../images/StagedBeam/2.PNG "Cross-section"
[prestressing]: ../images/StagedBeam/3.PNG "Prestressing"
[member data]: ../images/StagedBeam/4.PNG "Member data"
[constructionstages]: ../images/StagedBeam/5.PNG "Consruction stages"
[actionstages]: ../images/StagedBeam/6.PNG "Action stages"
[forces]: ../images/StagedBeam/7.PNG "Internal forces"
[reinforcement]: ../images/StagedBeam/8.PNG "Reinforcement"
[stirrups]: ../images/StagedBeam/9.PNG "Stirrups"
[longreinforcement]: ../images/StagedBeam/10.PNG "Longitudinal reinforcement"
[calccontrol]: ../images/StagedBeam/11.PNG "Calulation control"
[results]: ../images/StagedBeam/12.PNG "Results"
[sections]: ../images/StagedBeam/13.PNG "Sections"
[members]: ../images/StagedBeam/14.PNG "Members"
[rcs]: ../images/StagedBeam/15.PNG "Reinforced cross-sections"
[concretesetup]: ../images/StagedBeam/19.PNG "Concrete setup"