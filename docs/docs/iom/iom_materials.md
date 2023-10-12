# Materials

All materials in IOM derive from the `Material` base class. Materials are then defined by a Type Class (Steel, Concrete, Reinforcement) and then by country (EC, US, AUS).

All materials in an OpenModel **MUST** reference the same Country Code. To ensure this it is best to pass the IOM Open Model Settings Model Country Code on the creation of materials.  

## Library Materials

IDEA StatiCa has a library of materials that can be searched for without the requirement to specify all material information in the code. 

* Make sure the name exactly matches that of the material name in IDEA StatiCa. 
* The material type and material code also need to be created correctly.

If the material is not found in the IDEA Database the name will be created with default properties provided. 

```csharp
public static void Example_AddLibraryMaterialSteel(OpenModel openModel, CountryCode code)
{
    if (code == CountryCode.American)
    {
	//Define Name as an American Steel to Reference
	//Set Load From Library
	MatSteelAISC steelAISC = new MatSteelAISC() { Name = "A36", LoadFromLibrary = true };

	openModel.AddObject(steelAISC);
    }

    //Switch between other codes as required.
}
```


## Steel Materials

Steel Materials for different codes have differing parameters for creation. Although, still a lot share common parameters as well.

The below shows how to create a Eurocode Steel Material. 

```csharp
public static void Example_UserMaterialSteelECEN(OpenModel openModel, CountryCode code)
{
    if (code == CountryCode.ECEN)
    {
        MatSteelEc2 matS = new MatSteelEc2();
        matS.Name = "S275";
        matS.UnitMass = 7850.0;
        matS.E = 200e9;
        matS.Poisson = 0.2;
        matS.G = 83.333e9;
        matS.SpecificHeat = 0.6;
        matS.ThermalExpansion = 0.00001;
        matS.ThermalConductivity = 45;
	matS.fy = 235e6;
	matS.fu = 360e6;
	matS.fy40 = 215e6;
	matS.fu40 = 340e6;

	matS.StateOfThermalConductivity = ThermalConductivityState.Code;
	matS.StateOfThermalExpansion = ThermalExpansionState.Code;
	matS.StateOfThermalSpecificHeat = ThermalSpecificHeatState.Code;
	matS.StateOfThermalStressStrain = ThermalStressStrainState.Code;

	matS.DiagramType = SteelDiagramType.Bilinear;
    }
    openModel.AddObject(matS);
}
```

## Concrete Materials

The below shows an example of how to create a Eurocode Concrete Material.

```csharp
public static void Example_CreateAndAddUserMaterialConcreteWithDiagram(OpenModel openModel, CountryCode code)
{
    if (code == CountryCode.ECEN)
    {
	MatConcreteEc2 mat = new MatConcreteEc2();
	mat.Name = "Concrete1";
	mat.UnitMass = 2500.0;
	mat.E = 32.8e9;
	mat.G = 13.667e9;
	mat.Poisson = 0.2;
	mat.SpecificHeat = 0.6;
	mat.ThermalExpansion = 0.00001;
	mat.ThermalConductivity = 45;
	mat.Fck = 25.5e6;
	mat.CalculateDependentValues = true;

	//Set s-s diagram as a default parabolic
	mat.DiagramType = ConcDiagramType.Parabolic;

	//Set s-s diagram as an user defined
	mat.DiagramType = ConcDiagramType.DefinedByUser;
	var userDiagram = new Polygon2D();
	mat.UserDiagram = userDiagram;
	userDiagram.Points.Add(new Point2D() { X = -0.021, Y = 0 });
	userDiagram.Points.Add(new Point2D() { X = -0.02, Y = 0 });
	userDiagram.Points.Add(new Point2D() { X = -0.0025, Y = -80000000 });
	userDiagram.Points.Add(new Point2D() { X = -0.0024, Y = -79868660.43 });
	userDiagram.Points.Add(new Point2D() { X = -0.0023, Y = -79461961.9 });
	userDiagram.Points.Add(new Point2D() { X = -0.0022, Y = -78762709.44 });
	userDiagram.Points.Add(new Point2D() { X = -0.0021, Y = -77756658.69 });
	userDiagram.Points.Add(new Point2D() { X = -0.0019, Y = -74785483.74 });
	userDiagram.Points.Add(new Point2D() { X = -0.0017, Y = -70514061.33 });
	userDiagram.Points.Add(new Point2D() { X = -0.0015, Y = -64981949.46 });
	userDiagram.Points.Add(new Point2D() { X = -0.001, Y = -46511627.91 });
	userDiagram.Points.Add(new Point2D() { X = -0.0005, Y = -23904382.47 });
	userDiagram.Points.Add(new Point2D() { X = 0, Y = 0 });
	userDiagram.Points.Add(new Point2D() { X = 0.001, Y = 0 });

	//Setting thermal characteristcs of material (in this case by the code)
	mat.StateOfThermalConductivity = ThermalConductivityState.Code;
	mat.StateOfThermalExpansion = ThermalExpansionState.Code;
	mat.StateOfThermalSpecificHeat = ThermalSpecificHeatState.Code;
	mat.StateOfThermalStressStrain = ThermalStressStrainState.Code;

	openModel.AddObject(mat);
    }
}
```
## Timber Materials

> Timber materials cannot currently be defined through the IOM.


