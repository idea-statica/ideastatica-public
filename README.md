![build & test](https://github.com/idea-statica/ideastatica-public/actions/workflows/dotnet.yml/badge.svg)

# IdeaStatiCa API
Documentation of IDEA StatiCA API

## Idea Open Model
This model contains information about geometry, materials and loading of a structure. It is used for exchanging data with IdeaStatica and any othe FE or CAD application. The source code of [IOM](./src/IdeaRS.OpenModel)

## IdeaStatiCa.Plugin
is the gateway to IDEA StatiCa. It includes classes which allows communication and controlling IDEA StatiCa applications. It also includes classes which provides IDEA StatiCa services to other applications. [Latest version (v21.0)](./src/IdeaStatiCa.Plugin)

## IdeaRS.Connections
The documentation of manufacturing operations. [IdeaRS.Connections.Data](https://idea-statica.github.io/iom/ideaconnections-api/latest/html/N_IdeaRS_Connections_Data.htm)


# Examples
Examples how to use IDEA StatiCa API

## Idea Open Model
These examples show how to create Idea Open Model for various types of structures.
### Steel Structures
[Steel Frame 1 Example](src/Examples/IOM)

## Calculations
These examples show how to calculate structures which were defined in Idea Open Model

### Calculation of steel connection
[ConHiddenCheckConsole](src/Examples/ConnHiddenCalc/ConHiddenCheckConsole)

[ConnectionHiddenCalculation](src/Examples/ConnHiddenCalc/ConnectionHiddenCalculation)

[Python Examples](src/Examples/ConnHiddenCalc/python-examples)

### Concrete Structures

[Concrete Column](docs/rcs/rcs-column.md)

[Reinforced Beam](docs/rcs/rcs-reinforced-beam.md)

[Prestressed Beam](docs/rcs/rcs-prestressed-beam.md)

## CheckBot
Examples of using - Idea StatiCa Code Check Manager API

[FEAppExample_1](src/Examples/CCM/FEAppExample_1) 
