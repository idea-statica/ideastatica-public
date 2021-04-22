![build & test](https://github.com/idea-statica/ideastatica-public/actions/workflows/dotnet.yml/badge.svg)

# IdeaStatiCA API
Documentation of IDEA StatiCA API

## Idea Open Model
This model contains information about geometry, materials and loading of a structure. It is used for exchanging data with IdeaStatica and any othe FE or CAD application. The source code of [IOM](./src/IdeaRS.OpenModel)

## IdeaStatiCa.Plugin
is the gateway to IDEA StatiCa. It includes classes which allows communication and controlling IDEA StatiCa applications. It also includes classes which provides IDEA StatiCa services to other applications. [Latest version (v21.0)](https://github.com/idea-statica/ideastatica-plugin)

## IdeaRS.Connections
The documentation of manufacturing operations. [IdeaRS.Connections.Data](https://idea-statica.github.io/iom/ideaconnections-api/latest/html/N_IdeaRS_Connections_Data.htm)


# Examples
Examples how to use IDEA StatiCa API

## Idea Open Model
These examples show how to create Idea Open Model for various types of structures.
### Steel Structures
[Steel Frame 1 Example](https://idea-statica.github.io/iom-examples/iom-steel-connections/steel-frame1.html)

## Calculations
These examples show how to calculate structures which were defined in Idea Open Model

### Calculation of steel connection
[ConHiddenCheckConsole](https://idea-statica.github.io/iom-examples/iom-steel-connections/cbfem-for-all.html)

[ConnectionHiddenCalculation](https://idea-statica.github.io/iom-examples/iom-steel-connections/cbfem-for-all.html)

[Python Examples](https://github.com/idea-statica/iom-examples/blob/release-21.0-beta/python-examples/docs/python-examples.md)

### Concrete Structures

[Concrete Column](https://idea-statica.github.io/iom-examples/rcs/rcs-column.html)

[Reinforced Beam](https://idea-statica.github.io/iom-examples/rcs/rcs-reinforced-beam.html)

[Prestressed Beam](https://idea-statica.github.io/iom-examples/rcs/rcs-prestressed-beam.html)

## CheckBot
Examples of using - Idea StatiCa Code Check Manager API

[FEAppExample_1](https://github.com/idea-statica/ccm-examples) 
