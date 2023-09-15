**WIP: 👷‍♂️ Needs testing and confirmation.**

## Bolt Grades

A bolt grade can now be added to IOM using the [`MaterialBoltGrade`](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Libraries/Material/MaterialBoltGrade.cs) class.

In most instances, you may want to reference a library-defined bolt grade from the IDEA StatiCa Library. This can be done using a similar method to other materials. 

Where a specific code BoltGrade Material is not provided make sure to set the code parameter on the bolt grade. 

```csharp
public static void Example_AddMaterialBoltGrade(OpenModel openModel, CountryCode code)
{
    if (code == CountryCode.Australia)
    {
	//Define Name Australian bolt grade to Reference
	//Set Load From Library
	MaterialBoltGrade boltGrade = new MaterialBoltGrade { Name = "8.8", LoadFromLibrary = true, Code = CountryCode.Australia };

	openModel.AddObject(boltGrade);
    }
}
```

## Bolt Assemblies

A bolt assembly can now be added to IOM using the [`BoltAssembly`](https://github.com/idea-statica/ideastatica-public/blob/main/src/IdeaRS.OpenModel/Libraries/Material/BoltAssembly.cs) class:



A bolt assembly can be referenced to a `BoltGrid` or `AnchorGrid` by specifying the BoltAssembleyName parameter on the respective objects. Refer connection data pages.

## Weld Electrode / Grade

**Currently, weld electrodes and materials relating to welds are defined as a steel material and referenced to a weld by Name.**   

## 
