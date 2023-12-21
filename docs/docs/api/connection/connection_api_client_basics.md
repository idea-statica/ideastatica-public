
> [!WARNING]
> THIS IS WORK IN PROGRESS


# Project Operations

All example files can be downloaded here:

## Open Existing Project

Here we will open an existing IDEA StatiCa Connection Project. Once open we can start to extract and perform modifications on it. The project will remain open until we close the project.

# [Linux](#tab/linux)

Content for Linux...

# [Windows](#tab/windows)

Content for Windows...

---


### C# (.Net)

```csharp



```

### Python

```python

client.OpenProject('intro_baseplate_start.ideaCon')

```
## Save Existing Project

### C# (.Net)

```csharp



```

### Python

```python



```

## Save As Existing Project

### C# (.Net)

```csharp



```

### Python

```python



```

## Close Project

### C# (.Net)

```csharp



```

### Python

```python



```

## Get Project Library Items

We can access Materials, Bolts, and Cross-Sections which are available in the Project. Project Items are the same across all Connections in a Project, Therefore they are not dependent on a specific connection in the project.
    

### C# (.Net)

```csharp



```

### Python

```python
bolts = client.GetBoltAssembliesInProject()
crossSections = client.GetCrossSectionsInProject()
materials = client.GetMaterialsInProject()


print('Avaliable Bolt Assemblies:')
for bolt in bolts:
    print("Identifier= {0}, Name= {1}".format(bolt.Identifier, bolt.Name))
print('Avaliable CrossSections:')
for cross in crossSections:
    print("Identifier= {0}, Name= {1}".format(cross.Identifier, cross.Name))
print('Avaliable Materials:')
for material in materials:
    print("Identifier= {0}, Name= {1}".format(material.Identifier, material.Name))
```

    Avaliable Bolt Assemblies:
    Identifier= 1, Name= M16 8.8
    Avaliable CrossSections:
    Identifier= 1, Name= HEB300
    Identifier= 2, Name= IPE240
    Identifier= 3, Name= IPE360
    Avaliable Materials:
    Identifier= 1, Name= S 355
    Identifier= 2, Name= 8.8
    Identifier= 3, Name= C25/30
    Identifier= 4, Name= S 235
    Identifier= 5, Name= S 275


## Add Project Library Items

### C# (.Net)

```csharp



```

### Python

```python



```


## Starting the Plugin (API)


From the Plugin _Namespace_ we need to import the ConnHiddenClientFactory. The Client Factory allows us to safely _create_ connection clients that we can use to interact with IDEA Connection. 


```python
from IdeaStatiCa.Plugin import ConnHiddenClientFactory
```

We then again need to provide the path to the Client Factory and then safely create a client.


```python
factory = ConnHiddenClientFactory(idea_path)
client = factory.Create()
```

Now that we have a client created we can see what calls are avaliable to us to use. There is also a snap of these in the slides.

## Create and Idea Connection file from an IOM

## Opening an Existing IDEA StatiCa file

Here we will open an existing IDEA StatiCa Connection Project. Once open we can start to extract and perform modifications on it.




## Project Libraries

We can access Materials, Bolts, Cross-Sections which are avaliable in the Project. Project Items are the same accross all Connections in a Project, Therefore they are not dependent on a specific connection in the project.


```python
bolts = client.GetBoltAssembliesInProject()
crossSections = client.GetCrossSectionsInProject()
materials = client.GetMaterialsInProject()
```


```python
print('Avaliable Bolt Assemblies:')
for bolt in bolts:
    print("Identifier= {0}, Name= {1}".format(bolt.Identifier, bolt.Name))
print('Avaliable CrossSections:')
for cross in crossSections:
    print("Identifier= {0}, Name= {1}".format(cross.Identifier, cross.Name))
print('Avaliable Materials:')
for material in materials:
    print("Identifier= {0}, Name= {1}".format(material.Identifier, material.Name))
```

    [Avaliable Bolt Assemblies:
    Identifier= 1, Name= M16 8.8
    Avaliable CrossSections:
    Identifier= 1, Name= HEB300
    Identifier= 2, Name= IPE240
    Identifier= 3, Name= IPE360
    Avaliable Materials:
    Identifier= 1, Name= S 355
    Identifier= 2, Name= 8.8
    Identifier= 3, Name= C25/30
    Identifier= 4, Name= S 235
    Identifier= 5, Name= S 275]
    

### Adding Project Items

You can currently add Bolt Assemblies to the current Project. We will look to add further functionallity on this soon.


```python
#Add a list of new bolt assemblies to the Project
addAssemblies = ['M20 8.8', 'M24 8.8', 'M30 8.8']
for assem in addAssemblies:
    client.AddBoltAssembly(assem)
```


```python
bolts = client.GetBoltAssembliesInProject()
for bolt in bolts:
    print(bolt.Name)
```

    M16 8.8
    M20 8.8
    M24 8.8
    M30 8.8
    

## Connection Specific Items

Retrieving a connection from the Project. In the connection we have opened there is only 1 Connection avaliable. 


```python
projectInfo = client.GetProjectInfo()
connections = projectInfo.Connections
for conn in connections:
    print(conn.Identifier)
    
# we will select the first connection to continue with the Connection Specific Items.    
connection = connections[0].Identifier
connection2 = connections[1].Identifier
```

    0de79bf2-0035-4e66-8ae2-93df34436800
    


    ---------------------------------------------------------------------------

    ArgumentOutOfRangeException               Traceback (most recent call last)

    ~\AppData\Local\Temp\ipykernel_12880\4101456631.py in <module>
          6 # we will select the first connection to continue with the Connection Specific Items.
          7 connection = connections[0].Identifier
    ----> 8 connection2 = connections[1].Identifier
    

    ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
    Parameter name: index
       at System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
       at System.Collections.Generic.List`1.get_Item(Int32 index)


## Member Loads

Member load case be retrieved and then updated through the API using JSON

#### Getting Loads

If "percentageLoad" is false than "forcesOnSegments" is used, else "forcesPercentageOnSegments" is used.

**Note** for now checkEquilibrium and percentageLoad should not be through here.


```python
# We can retrive the loads in JSON
loads_input = client.GetConnectionLoadingJSON(connection)
loads_json = json.loads(loads_input)
```

##### Quick Look at JSON


```python
loads_output = json.dumps(loads_json, indent=4)
print(loads_output)
```

    [
        {
            "id": 1,
            "name": "LE1",
            "checkEquilibrium": true,
            "percentageLoad": false,
            "forcesOnSegments": [
                {
                    "beamSegmentId": 1,
                    "position": 1,
                    "n": -300000.0,
                    "qy": 0.0,
                    "qz": 0.0,
                    "mx": 0.0,
                    "my": 70000.0,
                    "mz": 8000.0,
                    "absPosition": 0.0,
                    "forceIn": 0
                }
            ],
            "forcesPercentageOnSegments": [
                {
                    "beamSegmentId": 1,
                    "position": 1,
                    "n": -0.0567,
                    "qy": 0.0,
                    "qz": 0.0,
                    "mx": 0.0,
                    "my": 0.1056,
                    "mz": 0.0258,
                    "absPosition": 0.0,
                    "forceIn": 0
                }
            ]
        }
    ]
    

### Update Loads

Lets update the axial force on the Column Member. Using Python dictionary notation we can set updated values to loads 


```python
loads_json[0]['forcesOnSegments'][0]['n'] = -450000
```


```python
loads_output = json.dumps(loads_json, indent=4)
print(loads_output)
```

    [
        {
            "id": 1,
            "name": "LE1",
            "checkEquilibrium": true,
            "percentageLoad": false,
            "forcesOnSegments": [
                {
                    "beamSegmentId": 1,
                    "position": 1,
                    "n": -450000,
                    "qy": 0.0,
                    "qz": 0.0,
                    "mx": 0.0,
                    "my": 70000.0,
                    "mz": 8000.0,
                    "absPosition": 0.0,
                    "forceIn": 0
                }
            ],
            "forcesPercentageOnSegments": [
                {
                    "beamSegmentId": 1,
                    "position": 1,
                    "n": -0.0567,
                    "qy": 0.0,
                    "qz": 0.0,
                    "mx": 0.0,
                    "my": 0.1056,
                    "mz": 0.0258,
                    "absPosition": 0.0,
                    "forceIn": 0
                }
            ]
        }
    ]
    


```python
client.UpdateLoadingFromJson(connection, loads_output)
```




    'OK'



## Code Setup

The code setup of the connection can be changed through the API using JSON as well.

### Get Code Setup


```python
code_input = client.GetCodeSetupJSON()
code_json = json.loads(code_input)
code_output = json.dumps(code_json, indent=4)
print(code_output)
```

    {
        "steelSetup": {
            "gammaM0": 1.0,
            "gammaM1": 1.0,
            "gammaM2": 1.25,
            "gammaMfi": 1.0,
            "gammaMu": 1.1
        },
        "concreteSetup": null,
        "stopAtLimitStrain": false,
        "weldEvaluationData": 4,
        "checkDetailing": false,
        "applyConeBreakoutCheck": 0,
        "pretensionForceFpc": 0.7,
        "gammaInst": 1.2,
        "gammaC": 1.5,
        "gammaM3": 1.25,
        "anchorLengthForStiffness": 8,
        "jointBetaFactor": 0.67,
        "effectiveAreaStressCoeff": 0.1,
        "effectiveAreaStressCoeffAISC": 0.1,
        "frictionCoefficient": 0.25,
        "limitPlasticStrain": 0.05,
        "limitDeformation": 0.03,
        "limitDeformationCheck": false,
        "analysisGNL": true,
        "warnPlasticStrain": 0.03,
        "warnCheckLevel": 0.95,
        "optimalCheckLevel": 0.6,
        "distanceBetweenBolts": 2.2,
        "distanceDiameterBetweenBP": 4.0,
        "distanceBetweenBoltsEdge": 1.2,
        "bearingAngle": 0.4637339822548933,
        "decreasingFtrd": 0.85,
        "bracedSystem": false,
        "bearingCheck": false,
        "applyBetapInfluence": false,
        "memberLengthRatio": 2.0,
        "divisionOfSurfaceOfCHS": 64,
        "divisionOfArcsOfRHS": 3,
        "numElement": 8,
        "numberIterations": 25,
        "mdiv": 3,
        "minSize": 0.01,
        "maxSize": 0.05,
        "numElementRhs": 16,
        "rigidBP": false,
        "alphaCC": 1.0,
        "crackedConcrete": true,
        "developedFillers": false,
        "deformationBoltHole": true,
        "extensionLengthRationOpenSections": 1.25,
        "extensionLengthRationCloseSections": 1.25,
        "factorPreloadBolt": 0.7,
        "baseMetalCapacity": false,
        "applyBearingCheck": true,
        "frictionCoefficientPbolt": 0.3,
        "crtCompCheckIS": 0,
        "boltMaxGripLengthCoeff": 0.0,
        "fatigueSectionOffset": 1.5,
        "condensedElementLengthFactor": 4.0,
        "gammaMu": 1.1
    }
    

### Update Code Setup


```python
code_json['stopAtLimitStrain'] = True
code_json['numElementRhs'] = 24
code_output = json.dumps(code_json, indent=4)
print(code_output)
```

    {
        "steelSetup": {
            "gammaM0": 1.0,
            "gammaM1": 1.0,
            "gammaM2": 1.25,
            "gammaMfi": 1.0,
            "gammaMu": 1.1
        },
        "concreteSetup": null,
        "stopAtLimitStrain": true,
        "weldEvaluationData": 4,
        "checkDetailing": false,
        "applyConeBreakoutCheck": 0,
        "pretensionForceFpc": 0.7,
        "gammaInst": 1.2,
        "gammaC": 1.5,
        "gammaM3": 1.25,
        "anchorLengthForStiffness": 8,
        "jointBetaFactor": 0.67,
        "effectiveAreaStressCoeff": 0.1,
        "effectiveAreaStressCoeffAISC": 0.1,
        "frictionCoefficient": 0.25,
        "limitPlasticStrain": 0.05,
        "limitDeformation": 0.03,
        "limitDeformationCheck": false,
        "analysisGNL": true,
        "warnPlasticStrain": 0.03,
        "warnCheckLevel": 0.95,
        "optimalCheckLevel": 0.6,
        "distanceBetweenBolts": 2.2,
        "distanceDiameterBetweenBP": 4.0,
        "distanceBetweenBoltsEdge": 1.2,
        "bearingAngle": 0.4637339822548933,
        "decreasingFtrd": 0.85,
        "bracedSystem": false,
        "bearingCheck": false,
        "applyBetapInfluence": false,
        "memberLengthRatio": 2.0,
        "divisionOfSurfaceOfCHS": 64,
        "divisionOfArcsOfRHS": 3,
        "numElement": 8,
        "numberIterations": 25,
        "mdiv": 3,
        "minSize": 0.01,
        "maxSize": 0.05,
        "numElementRhs": 24,
        "rigidBP": false,
        "alphaCC": 1.0,
        "crackedConcrete": true,
        "developedFillers": false,
        "deformationBoltHole": true,
        "extensionLengthRationOpenSections": 1.25,
        "extensionLengthRationCloseSections": 1.25,
        "factorPreloadBolt": 0.7,
        "baseMetalCapacity": false,
        "applyBearingCheck": true,
        "frictionCoefficientPbolt": 0.3,
        "crtCompCheckIS": 0,
        "boltMaxGripLengthCoeff": 0.0,
        "fatigueSectionOffset": 1.5,
        "condensedElementLengthFactor": 4.0,
        "gammaMu": 1.1
    }
    


```python
client.UpdateCodeSetupJSON(code_output)
```




    'OK'



## Other Modifications

### Update Section Material Section Size

As the cross section is a Project parameter you only need to provide the Cross-Section Id and the Material Id you want to use. 
I will change the cross-section material from S355 to S275.


```python
client.SetCrossSectionMaterial(1, 5)
```

### Update Member Section Size

When updating a member section size I need to provide the connection in which the member is apart of, the member Id and the **new** cross-section Id.


```python
client.SetMemberCrossSection(connection, 1, 3)
```

## Parameters

Parameters allow for a user to define parameters in a connection which can change operation values. Parameters can also be updated through the API for a given connection.

Currently parameters must be defined either in a template or the connection file manually. Once a parameters is defined we can retrieve and modify them through the API.

### Get Parameters


```python
parameters_input = client.GetParametersJSON(connection)
parameters_json = json.loads(parameters_input)
parameters_output = json.dumps(parameters_json)
print(parameters_output)
```

    [{"id": 1, "identifier": "plate_thk", "description": "plate_thk", "parameterType": "Float", "value": "0.016"}]
    

### Update Parameters

Lets change the plate thickness parameter value to 20mm 


```python
parameters_json[0]['value'] = 0.02
parameters_output = json.dumps(parameters_json)
print(parameters_output)
```

    [{"id": 1, "identifier": "plate_thk", "description": "plate_thk", "parameterType": "Float", "value": 0.02}]
    


```python
client.ApplyParameters(connection, parameters_output)
```




    'OK'



## Evaluating Expressions

We can evaluate certain The section dimensions will be used to understand how the dimensions may determine which template we want to adopt or how parameters will be affected.

We can use some of the expressions avaliable to do this. https://github.com/idea-statica/ideastatica-public/wiki/Reference-Guide-Expression-Parameters


```python
height = client.EvaluateExpression(connection, "GetValue('COL', 'CrossSection.Bounds.Height')", None)
width = client.EvaluateExpression(connection, "GetValue('COL', 'CrossSection.Bounds.Width')", None)
```


```python
print(height)
print(width)
```

    0.30000000000000004
    0.3
    

We can also evaluate what is avaliable at a lower level.


```python
typeof = client.EvaluateExpression(connection, "GetValue('COL','Parameters')", None)
print(typeof)
```

    {
      "operationId": 1,
      "imported": false,
      "name": "COL",
      "isActive": true,
      "crossSectionType": "#1",
      "cssWidth": 0.1,
      "cssHeight": 0.2,
      "flangeThickness": 0.005,
      "webThickness": 0.005,
      "materialName": "S 235",
      "originalModelId": null,
      "insertPointOnRefLine": 0.0,
      "rigidLinkX": 0.0,
      "rigidLinkY": 0.0,
      "rigidLinkZ": 0.0,
      "placementDefinition": 2,
      "dirVectX": 6.1230317691118863E-17,
      "dirVectY": 0.0,
      "dirVectZ": 1.0,
      "lcsyVectX": 0.0,
      "lcsyVectY": 0.0,
      "lcsyVectZ": 0.0,
      "lcszVectX": 1.0,
      "lcszVectY": 0.0,
      "lcszVectZ": 0.0,
      "angleAlpha": 0.0,
      "angleBeta": -1.5707963267948966,
      "eccentricityBeginX": 0.0,
      "eccentricityBeginY": 0.0,
      "eccentricityBeginZ": 0.0,
      "eccentricityEndX": 0.0,
      "eccentricityEndY": 0.0,
      "eccentricityEndZ": 0.0,
      "length": 6.0,
      "rotationRx": 0.0,
      "supportCode": 0,
      "segmentEndBehavior": 0,
      "theoreticalLengthY": 6.0,
      "theoreticalLengthZ": 6.0,
      "mirrorZ": false,
      "mirrorY": false,
      "forceIn": 1,
      "forceAbsPosition": 0.0,
      "alignment": 0,
      "alignedPlateIndex": 0,
      "alignedToPlate": {
        "memberId": 0,
        "plateIndex": 0
      },
      "alignedPlateSide": 0,
      "alignedToPlateSide": 0,
      "myData": {
        "crossSectionType": "#1",
        "cssWidth": 0.1,
        "cssHeight": 0.2,
        "flangeThickness": 0.005,
        "webThickness": 0.005,
        "materialName": "S 235",
        "originalModelId": null,
        "insertPointOnRefLine": 0.0,
        "rigidLinkX": 0.0,
        "rigidLinkY": 0.0,
        "rigidLinkZ": 0.0,
        "placementDefinition": 2,
        "dirVectX": 6.1230317691118863E-17,
        "dirVectY": 0.0,
        "dirVectZ": 1.0,
        "lcsyVectX": 0.0,
        "lcsyVectY": 0.0,
        "lcsyVectZ": 0.0,
        "lcszVectX": 1.0,
        "lcszVectY": 0.0,
        "lcszVectZ": 0.0,
        "angleAlpha": 0.0,
        "angleBeta": -1.5707963267948966,
        "eccentricityBeginX": 0.0,
        "eccentricityBeginY": 0.0,
        "eccentricityBeginZ": 0.0,
        "eccentricityEndX": 0.0,
        "eccentricityEndY": 0.0,
        "eccentricityEndZ": 0.0,
        "length": 6.0,
        "rotationRx": 0.0,
        "supportCode": 0,
        "segmentEndBehavior": 0,
        "theoreticalLengthY": 6.0,
        "theoreticalLengthZ": 6.0,
        "mirrorZ": false,
        "mirrorY": false,
        "forceIn": 1,
        "forceAbsPosition": 0.0,
        "alignment": 0,
        "alignedPlateIndex": 0,
        "alignedToPlate": {
          "memberId": 0,
          "plateIndex": 0
        },
        "alignedPlateSide": 0,
        "alignedToPlateSide": 0,
        "operationId": 1,
        "imported": false,
        "name": "COL"
      }
    }
    

## Saving the Connection

Once modifications have been made to the connection, the connection can either be Saved or Saved As a new Connection. Lets SaveAs the connection


```python
#client.Save()
```


```python
client.SaveAsProject('intro_baseplate_modified.ideaCon')
```

Lets open the Saved connection file to view the changes

## Calculation and Results 

Once modifications have been made, the calculation can be triggered from the API and results retrieved.


```python
result = client.Calculate(connection)
print(result)
```

    IdeaRS.OpenModel.Connection.ConnectionResultsData
    


```python
results_input = client.GetCheckResultsJSON(connection)
```


```python
results_json = json.loads(results_input)

#print(json.dumps(results_json, indent=4))

summary_json = results_json['summary']

for item in summary_json:
    print('{0}: {1}, {2}'.format(summary_json[item]['name'], summary_json[item]['checkStatus'] , summary_json[item]['checkValue']))
```

    Analysis: True, 1.0
    Plates: True, 0.0
    Anchors: True, 0.5681421256583504
    Welds: True, 0.3028398428571428
    Concrete block: True, 0.24209571747896802
    Shear: True, 7.62577929945549e-05
    Buckling: True, -1.0
    

## Get Connection Cost

Connection Costs are also provided as JSON output


```python
cost_input = client.GetConnectionCost(connection)
cost_json = json.loads(cost_input)
cost_output = json.dumps(cost_json)

total_cost = cost_json['TotalEstimatedCost']

print(total_cost)
```

    145.47599720049766
    

## Get IOM Geometry and Results

We can also get IOM Geometry from the IDEA Connection file


```python
xmlModel = client.GetConnectionModelXML(connection)
print(xmlModel)
```

    <?xml version="1.0" encoding="utf-16"?>
    <ConnectionData xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
      <ConenctionPointId>1</ConenctionPointId>
      <Beams>
        <BeamData>
          <Id>1</Id>
          <Name>COL</Name>
          <Plates>
            <PlateData>
              <Id>1</Id>
              <Name />
              <Thickness>0.012699999809265138</Thickness>
              <Material>S 355</Material>
              <Origin>
                <Id>0</Id>
                <X>0.1736485380017706</X>
                <Y>-1.1925141429891539E-11</Y>
                <Z>-1.0632555148446741E-17</Z>
              </Origin>
              <AxisX>
                <X>6.1679056923619811E-17</X>
                <Y>0</Y>
                <Z>1</Z>
              </AxisX>
              <AxisY>
                <X>0</X>
                <Y>1</Y>
                <Z>0</Z>
              </AxisY>
              <AxisZ>
                <X>-1</X>
                <Y>0</Y>
                <Z>6.1679056923619811E-17</Z>
              </AxisZ>
              <Region>M 1.06325551484467E-17 0.085 L 0 -0.085 L 0.6 -0.085 L 0.6 0.085 L 1.06325551484467E-17 0.085</Region>
              <Geometry>
                <Outline>
                  <StartPoint>
                    <X>1.0632555148446741E-17</X>
                    <Y>0.085</Y>
                  </StartPoint>
                  <Segments>
                    <Segment2D xsi:type="LineSegment2D">
                      <EndPoint>
                        <X>0</X>
                        <Y>-0.085</Y>
                      </EndPoint>
                    </Segment2D>
                    <Segment2D xsi:type="LineSegment2D">
                      <EndPoint>
                        <X>0.59999999999999964</X>
                        <Y>-0.085</Y>
                      </EndPoint>
                    </Segment2D>
                    <Segment2D xsi:type="LineSegment2D">
                      <EndPoint>
                        <X>0.59999999999999964</X>
                        <Y>0.085</Y>
                      </EndPoint>
                    </Segment2D>
                    <Segment2D xsi:type="LineSegment2D">
                      <EndPoint>
                        <X>1.0632555148446741E-17</X>
                        <Y>0.085</Y>
                      </EndPoint>
                    </Segment2D>
                  </Segments>
                </Outline>
              </Geometry>
              <OriginalModelId>Plate1</OriginalModelId>
              <IsNegativeObject>false</IsNegativeObject>
            </PlateData>
            <PlateData>
              <Id>2</Id>
              <Name />
              <Thickness>0.012699999809265138</Thickness>
              <Material>S 355</Material>
              <Origin>
                <Id>0</Id>
                <X>-0.17365146218896427</X>
                <Y>-1.1925141429891539E-11</Y>
                <Z>1.0632734197357596E-17</Z>
              </Origin>
              <AxisX>
                <X>6.1679056923619811E-17</X>
                <Y>0</Y>
                <Z>1</Z>
              </AxisX>
              <AxisY>
                <X>0</X>
                <Y>1</Y>
                <Z>0</Z>
              </AxisY>
              <AxisZ>
                <X>-1</X>
                <Y>0</Y>
                <Z>6.1679056923619811E-17</Z>
              </AxisZ>
              <Region>M -1.06327341973576E-17 0.085 L 0 -0.085 L 0.6 -0.085 L 0.6 0.085 L -1.06327341973576E-17 0.085</Region>
              <Geometry>
                <Outline>
                  <StartPoint>
                    <X>-1.0632734197357596E-17</X>
                    <Y>0.085</Y>
                  </StartPoint>
                  <Segments>
                    <Segment2D xsi:type="LineSegment2D">
                      <EndPoint>
                        <X>0</X>
                        <Y>-0.085</Y>
                      </EndPoint>
                    </Segment2D>
                    <Segment2D xsi:type="LineSegment2D">
                      <EndPoint>
                        <X>0.59999999999999964</X>
                        <Y>-0.085</Y>
                      </EndPoint>
                    </Segment2D>
                    <Segment2D xsi:type="LineSegment2D">
                      <EndPoint>
                        <X>0.59999999999999964</X>
                        <Y>0.085</Y>
                      </EndPoint>
                    </Segment2D>
                    <Segment2D xsi:type="LineSegment2D">
                      <EndPoint>
                        <X>-1.0632734197357596E-17</X>
                        <Y>0.085</Y>
                      </EndPoint>
                    </Segment2D>
                  </Segments>
                </Outline>
              </Geometry>
              <OriginalModelId>Plate2</OriginalModelId>
              <IsNegativeObject>false</IsNegativeObject>
            </PlateData>
            <PlateData>
              <Id>3</Id>
              <Name />
              <Thickness>0.008</Thickness>
              <Material>S 355</Material>
              <Origin>
                <Id>0</Id>
                <X>-1.462093596832359E-06</X>
                <Y>-1.1925141429891539E-11</Y>
                <Z>8.9524455427837144E-23</Z>
              </Origin>
              <AxisX>
                <X>6.1679056923619811E-17</X>
                <Y>0</Y>
                <Z>1</Z>
              </AxisX>
              <AxisY>
                <X>1</X>
                <Y>0</Y>
                <Z>-6.1230317691118863E-17</Z>
              </AxisY>
              <AxisZ>
                <X>-0</X>
                <Y>1</Y>
                <Z>0</Z>
              </AxisZ>
              <Region>M 1.07104787162133E-17 0.173650000095367 L -7.79235677665848E-20 -0.173650000095367 L 0.6 -0.173650000095367 L 0.6 0.173650000095367 L 1.07104787162133E-17 0.173650000095367</Region>
              <Geometry>
                <Outline>
                  <StartPoint>
                    <X>1.0710478716213326E-17</X>
                    <Y>0.17365000009536743</Y>
                  </StartPoint>
                  <Segments>
                    <Segment2D xsi:type="LineSegment2D">
                      <EndPoint>
                        <X>-7.79235677665848E-20</X>
                        <Y>-0.17365000009536743</Y>
                      </EndPoint>
                    </Segment2D>
                    <Segment2D xsi:type="LineSegment2D">
                      <EndPoint>
                        <X>0.59999999999999964</X>
                        <Y>-0.17365000009536743</Y>
                      </EndPoint>
                    </Segment2D>
                    <Segment2D xsi:type="LineSegment2D">
                      <EndPoint>
                        <X>0.59999999999999964</X>
                        <Y>0.17365000009536743</Y>
                      </EndPoint>
                    </Segment2D>
                    <Segment2D xsi:type="LineSegment2D">
                      <EndPoint>
                        <X>1.0710478716213326E-17</X>
                        <Y>0.17365000009536743</Y>
                      </EndPoint>
                    </Segment2D>
                  </Segments>
                </Outline>
              </Geometry>
              <OriginalModelId>Plate3</OriginalModelId>
              <IsNegativeObject>false</IsNegativeObject>
            </PlateData>
          </Plates>
          <CrossSectionType>RolledI</CrossSectionType>
          <MprlName>IPE360</MprlName>
          <OriginalModelId>1</OriginalModelId>
          <Cuts>
            <CutData>
              <PlanePoint>
                <Id>0</Id>
                <X>0</X>
                <Y>0</Y>
                <Z>0</Z>
              </PlanePoint>
              <NormalVector>
                <X>6.1230317691118863E-17</X>
                <Y>-0</Y>
                <Z>1</Z>
              </NormalVector>
              <Direction>Default</Direction>
              <Offset>0</Offset>
            </CutData>
          </Cuts>
          <IsAdded>false</IsAdded>
          <AddedMemberLength>0</AddedMemberLength>
          <IsNegativeObject>false</IsNegativeObject>
          <MirrorY>false</MirrorY>
          <RefLineInCenterOfGravity>false</RefLineInCenterOfGravity>
          <IsBearingMember>false</IsBearingMember>
          <AutoAddCutByWorkplane>false</AutoAddCutByWorkplane>
        </BeamData>
      </Beams>
      <Plates>
        <PlateData>
          <Id>4</Id>
          <Name>BP1</Name>
          <Thickness>0.02</Thickness>
          <Material>S 355</Material>
          <Origin>
            <Id>0</Id>
            <X>-6.1230317691118863E-19</X>
            <Y>0</Y>
            <Z>-0.01</Z>
          </Origin>
          <AxisX>
            <X>0</X>
            <Y>1</Y>
            <Z>0</Z>
          </AxisX>
          <AxisY>
            <X>-1</X>
            <Y>0</Y>
            <Z>6.1230317691118863E-17</Z>
          </AxisY>
          <AxisZ>
            <X>6.1230317691118863E-17</X>
            <Y>-0</Y>
            <Z>1</Z>
          </AxisZ>
          <Region>M -0.165000000011925 -0.259998537906403 L 0.164999999988075 -0.259998537906403 L 0.164999999988075 0.260001462093597 L -0.165000000011925 0.260001462093597 L -0.165000000011925 -0.259998537906403</Region>
          <Geometry>
            <Outline>
              <StartPoint>
                <X>-0.16500000001192516</X>
                <Y>-0.25999853790640315</Y>
              </StartPoint>
              <Segments>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>0.16499999998807485</X>
                    <Y>-0.25999853790640315</Y>
                  </EndPoint>
                </Segment2D>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>0.16499999998807485</X>
                    <Y>0.26000146209359681</Y>
                  </EndPoint>
                </Segment2D>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>-0.16500000001192516</X>
                    <Y>0.26000146209359681</Y>
                  </EndPoint>
                </Segment2D>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>-0.16500000001192516</X>
                    <Y>-0.25999853790640315</Y>
                  </EndPoint>
                </Segment2D>
              </Segments>
            </Outline>
          </Geometry>
          <OriginalModelId>Plate4</OriginalModelId>
          <IsNegativeObject>false</IsNegativeObject>
        </PlateData>
        <PlateData>
          <Id>5</Id>
          <Name>WID1a</Name>
          <Thickness>0.02</Thickness>
          <Material>S 355</Material>
          <Origin>
            <Id>0</Id>
            <X>0.1736485380017706</X>
            <Y>-0.085000000011925148</Y>
            <Z>-1.0632555148446741E-17</Z>
          </Origin>
          <AxisX>
            <X>0</X>
            <Y>-1</Y>
            <Z>-6.25444420496867E-17</Z>
          </AxisX>
          <AxisY>
            <X>6.1508200810257972E-17</X>
            <Y>-6.25444420496867E-17</Y>
            <Z>1</Z>
          </AxisY>
          <AxisZ>
            <X>-1</X>
            <Y>-3.8469961011576724E-33</Y>
            <Z>6.1508200810257972E-17</Z>
          </AxisZ>
          <Region>M 0 0 L 0.08 1.54074395550979E-33 L -9.38166630745301E-18 0.15 L 0 0</Region>
          <Geometry>
            <Outline>
              <StartPoint>
                <X>0</X>
                <Y>0</Y>
              </StartPoint>
              <Segments>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>0.080000000000000016</X>
                    <Y>1.5407439555097887E-33</Y>
                  </EndPoint>
                </Segment2D>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>-9.3816663074530061E-18</X>
                    <Y>0.15</Y>
                  </EndPoint>
                </Segment2D>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>0</X>
                    <Y>0</Y>
                  </EndPoint>
                </Segment2D>
              </Segments>
            </Outline>
          </Geometry>
          <OriginalModelId>Plate5</OriginalModelId>
          <IsNegativeObject>false</IsNegativeObject>
        </PlateData>
        <PlateData>
          <Id>6</Id>
          <Name>WID1b</Name>
          <Thickness>0.02</Thickness>
          <Material>S 355</Material>
          <Origin>
            <Id>0</Id>
            <X>0.1736485380017706</X>
            <Y>0.084999999988074865</Y>
            <Z>0</Z>
          </Origin>
          <AxisX>
            <X>-0</X>
            <Y>1</Y>
            <Z>6.25444420496867E-17</Z>
          </AxisX>
          <AxisY>
            <X>6.1508200810257984E-17</X>
            <Y>-6.25444420496867E-17</Y>
            <Z>1</Z>
          </AxisY>
          <AxisZ>
            <X>1</X>
            <Y>3.8469961011576731E-33</Y>
            <Z>-6.1508200810257984E-17</Z>
          </AxisZ>
          <Region>M 0 0 L 0.08 7.70371977754894E-34 L 9.38166630745301E-18 0.15 L 0 0</Region>
          <Geometry>
            <Outline>
              <StartPoint>
                <X>0</X>
                <Y>0</Y>
              </StartPoint>
              <Segments>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>0.079999999999999988</X>
                    <Y>7.7037197775489434E-34</Y>
                  </EndPoint>
                </Segment2D>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>9.3816663074530061E-18</X>
                    <Y>0.15</Y>
                  </EndPoint>
                </Segment2D>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>0</X>
                    <Y>0</Y>
                  </EndPoint>
                </Segment2D>
              </Segments>
            </Outline>
          </Geometry>
          <OriginalModelId>Plate6</OriginalModelId>
          <IsNegativeObject>false</IsNegativeObject>
        </PlateData>
        <PlateData>
          <Id>7</Id>
          <Name>WID1c</Name>
          <Thickness>0.02</Thickness>
          <Material>S 355</Material>
          <Origin>
            <Id>0</Id>
            <X>-0.17365146218896427</X>
            <Y>-0.085000000011925148</Y>
            <Z>1.0632734197357596E-17</Z>
          </Origin>
          <AxisX>
            <X>0</X>
            <Y>-1</Y>
            <Z>6.25454952785741E-17</Z>
          </AxisX>
          <AxisY>
            <X>6.1508200810257984E-17</X>
            <Y>6.25454952785741E-17</Y>
            <Z>1</Z>
          </AxisY>
          <AxisZ>
            <X>-1</X>
            <Y>3.847060883371578E-33</Y>
            <Z>6.1508200810257984E-17</Z>
          </AxisZ>
          <Region>M 0 0 L 0.08 -7.70371977754894E-34 L 9.38182429178611E-18 0.15 L 0 0</Region>
          <Geometry>
            <Outline>
              <StartPoint>
                <X>0</X>
                <Y>0</Y>
              </StartPoint>
              <Segments>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>0.080000000000000016</X>
                    <Y>-7.7037197775489434E-34</Y>
                  </EndPoint>
                </Segment2D>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>9.3818242917861144E-18</X>
                    <Y>0.15</Y>
                  </EndPoint>
                </Segment2D>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>0</X>
                    <Y>0</Y>
                  </EndPoint>
                </Segment2D>
              </Segments>
            </Outline>
          </Geometry>
          <OriginalModelId>Plate7</OriginalModelId>
          <IsNegativeObject>false</IsNegativeObject>
        </PlateData>
        <PlateData>
          <Id>8</Id>
          <Name>WID1d</Name>
          <Thickness>0.02</Thickness>
          <Material>S 355</Material>
          <Origin>
            <Id>0</Id>
            <X>-0.17365146218896427</X>
            <Y>0.084999999988074865</Y>
            <Z>0</Z>
          </Origin>
          <AxisX>
            <X>-0</X>
            <Y>1</Y>
            <Z>-6.25454952785741E-17</Z>
          </AxisX>
          <AxisY>
            <X>6.1508200810257984E-17</X>
            <Y>6.25454952785741E-17</Y>
            <Z>1</Z>
          </AxisY>
          <AxisZ>
            <X>1</X>
            <Y>-3.847060883371578E-33</Y>
            <Z>-6.1508200810257984E-17</Z>
          </AxisZ>
          <Region>M 0 0 L 0.08 -7.70371977754894E-34 L -9.38182429178611E-18 0.15 L 0 0</Region>
          <Geometry>
            <Outline>
              <StartPoint>
                <X>0</X>
                <Y>0</Y>
              </StartPoint>
              <Segments>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>0.079999999999999988</X>
                    <Y>-7.7037197775489434E-34</Y>
                  </EndPoint>
                </Segment2D>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>-9.3818242917861144E-18</X>
                    <Y>0.15</Y>
                  </EndPoint>
                </Segment2D>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>0</X>
                    <Y>0</Y>
                  </EndPoint>
                </Segment2D>
              </Segments>
            </Outline>
          </Geometry>
          <OriginalModelId>Plate8</OriginalModelId>
          <IsNegativeObject>false</IsNegativeObject>
        </PlateData>
        <PlateData>
          <Id>9</Id>
          <Name>WID1e</Name>
          <Thickness>0.02</Thickness>
          <Material>S 355</Material>
          <Origin>
            <Id>0</Id>
            <X>-0.18000146209359683</X>
            <Y>-1.1925141429891539E-11</Y>
            <Z>1.0827142089963769E-17</Z>
          </Origin>
          <AxisX>
            <X>-1</X>
            <Y>0</Y>
            <Z>3.061541661824984E-17</Z>
          </AxisX>
          <AxisY>
            <X>3.061541661824984E-17</X>
            <Y>0</Y>
            <Z>1</Z>
          </AxisY>
          <AxisZ>
            <X>0</X>
            <Y>1</Y>
            <Z>-0</Z>
          </AxisZ>
          <Region>M 0 0 L 0.08 3.85185988877447E-34 L 4.59231249273748E-18 0.15 L 0 0</Region>
          <Geometry>
            <Outline>
              <StartPoint>
                <X>0</X>
                <Y>0</Y>
              </StartPoint>
              <Segments>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>0.079999999999999988</X>
                    <Y>3.8518598887744717E-34</Y>
                  </EndPoint>
                </Segment2D>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>4.592312492737476E-18</X>
                    <Y>0.15</Y>
                  </EndPoint>
                </Segment2D>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>0</X>
                    <Y>0</Y>
                  </EndPoint>
                </Segment2D>
              </Segments>
            </Outline>
          </Geometry>
          <OriginalModelId>Plate9</OriginalModelId>
          <IsNegativeObject>false</IsNegativeObject>
        </PlateData>
        <PlateData>
          <Id>10</Id>
          <Name>WID1f</Name>
          <Thickness>0.02</Thickness>
          <Material>S 355</Material>
          <Origin>
            <Id>0</Id>
            <X>0.17999853790640316</X>
            <Y>-1.1925141429891539E-11</Y>
            <Z>-1.9440789260617284E-19</Z>
          </Origin>
          <AxisX>
            <X>1</X>
            <Y>-0</Y>
            <Z>-3.061541661824984E-17</Z>
          </AxisX>
          <AxisY>
            <X>3.061541661824984E-17</X>
            <Y>0</Y>
            <Z>1</Z>
          </AxisY>
          <AxisZ>
            <X>0</X>
            <Y>-1</Y>
            <Z>0</Z>
          </AxisZ>
          <Region>M 0 0 L 0.08 -3.85185988877447E-34 L -4.59231249273748E-18 0.15 L 0 0</Region>
          <Geometry>
            <Outline>
              <StartPoint>
                <X>0</X>
                <Y>0</Y>
              </StartPoint>
              <Segments>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>0.079999999999999988</X>
                    <Y>-3.8518598887744717E-34</Y>
                  </EndPoint>
                </Segment2D>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>-4.592312492737476E-18</X>
                    <Y>0.15</Y>
                  </EndPoint>
                </Segment2D>
                <Segment2D xsi:type="LineSegment2D">
                  <EndPoint>
                    <X>0</X>
                    <Y>0</Y>
                  </EndPoint>
                </Segment2D>
              </Segments>
            </Outline>
          </Geometry>
          <OriginalModelId>Plate10</OriginalModelId>
          <IsNegativeObject>false</IsNegativeObject>
        </PlateData>
      </Plates>
      <FoldedPlates />
      <BoltGrids>
        <BoltGrid>
          <Id>1</Id>
          <IsAnchor>true</IsAnchor>
          <AnchorLen>0</AnchorLen>
          <HoleDiameter>0.018000000000000002</HoleDiameter>
          <Diameter>0.016</Diameter>
          <HeadDiameter>0</HeadDiameter>
          <DiagonalHeadDiameter>0</DiagonalHeadDiameter>
          <HeadHeight>0</HeadHeight>
          <BoreHole>0</BoreHole>
          <TensileStressArea>0</TensileStressArea>
          <NutThickness>0</NutThickness>
          <BoltAssemblyName>M16 8.8</BoltAssemblyName>
          <Origin>
            <Id>0</Id>
            <X>-6.1230317691118863E-19</X>
            <Y>0</Y>
            <Z>-0.01</Z>
          </Origin>
          <AxisX>
            <X>0</X>
            <Y>1</Y>
            <Z>0</Z>
          </AxisX>
          <AxisY>
            <X>-1</X>
            <Y>0</Y>
            <Z>6.1230317691118863E-17</Z>
          </AxisY>
          <AxisZ>
            <X>6.1230317691118863E-17</X>
            <Y>-0</Y>
            <Z>1</Z>
          </AxisZ>
          <Positions>
            <Point3D>
              <Id>0</Id>
              <X>0.12499999998807487</X>
              <Y>0.22000146209359683</Y>
              <Z>0</Z>
            </Point3D>
            <Point3D>
              <Id>0</Id>
              <X>-0.12500000001192516</X>
              <Y>0.22000146209359683</Y>
              <Z>0</Z>
            </Point3D>
            <Point3D>
              <Id>0</Id>
              <X>0.12499999998807487</X>
              <Y>-0.21999853790640317</Y>
              <Z>0</Z>
            </Point3D>
            <Point3D>
              <Id>0</Id>
              <X>-0.12500000001192516</X>
              <Y>-0.21999853790640317</Y>
              <Z>0</Z>
            </Point3D>
          </Positions>
          <ConnectedPlates>
            <int>4</int>
          </ConnectedPlates>
          <ShearInThread>false</ShearInThread>
          <BoltInteraction>Bearing</BoltInteraction>
        </BoltGrid>
      </BoltGrids>
      <AnchorGrids />
      <Welds>
        <WeldData>
          <Id>3</Id>
          <Name />
          <Thickness>0.008</Thickness>
          <Material>S 355</Material>
          <WeldType>NotSpecified</WeldType>
          <ConnectedPartIds>
            <string>4</string>
            <string>1</string>
          </ConnectedPartIds>
          <Start>
            <Id>0</Id>
            <X>0.1736485380017706</X>
            <Y>0.084999999988074865</Y>
            <Z>0</Z>
          </Start>
          <End>
            <Id>0</Id>
            <X>0.1736485380017706</X>
            <Y>-0.085000000011925148</Y>
            <Z>-1.0632555148446741E-17</Z>
          </End>
        </WeldData>
        <WeldData>
          <Id>4</Id>
          <Name />
          <Thickness>0.008</Thickness>
          <Material>S 355</Material>
          <WeldType>NotSpecified</WeldType>
          <ConnectedPartIds>
            <string>4</string>
            <string>1</string>
          </ConnectedPartIds>
          <Start>
            <Id>0</Id>
            <X>-0.17365146218896427</X>
            <Y>0.084999999988074865</Y>
            <Z>0</Z>
          </Start>
          <End>
            <Id>0</Id>
            <X>-0.17365146218896427</X>
            <Y>-0.085000000011925148</Y>
            <Z>1.0632734197357596E-17</Z>
          </End>
        </WeldData>
        <WeldData>
          <Id>5</Id>
          <Name />
          <Thickness>0.006</Thickness>
          <Material>S 355</Material>
          <WeldType>NotSpecified</WeldType>
          <ConnectedPartIds>
            <string>4</string>
            <string>1</string>
          </ConnectedPartIds>
          <Start>
            <Id>0</Id>
            <X>0.1736485380017706</X>
            <Y>-1.1925141429891539E-11</Y>
            <Z>0</Z>
          </Start>
          <End>
            <Id>0</Id>
            <X>-0.17365146218896427</X>
            <Y>-1.1925141429891539E-11</Y>
            <Z>1.0632734197357596E-17</Z>
          </End>
        </WeldData>
        <WeldData>
          <Id>6</Id>
          <Name />
          <Thickness>0.008</Thickness>
          <Material>S 355</Material>
          <WeldType>NotSpecified</WeldType>
          <ConnectedPartIds>
            <string>4</string>
            <string>5</string>
          </ConnectedPartIds>
          <Start>
            <Id>0</Id>
            <X>0.1736485380017706</X>
            <Y>-0.085000000011925148</Y>
            <Z>-1.0632555148446741E-17</Z>
          </Start>
          <End>
            <Id>0</Id>
            <X>0.1736485380017706</X>
            <Y>-0.16500000001192516</Y>
            <Z>-1.5636110512421676E-17</Z>
          </End>
        </WeldData>
        <WeldData>
          <Id>18</Id>
          <Name />
          <Thickness>0.008</Thickness>
          <Material>S 355</Material>
          <WeldType>NotSpecified</WeldType>
          <ConnectedPartIds>
            <string>1</string>
            <string>5</string>
          </ConnectedPartIds>
          <Start>
            <Id>0</Id>
            <X>0.1736485380017706</X>
            <Y>-0.085000000011925148</Y>
            <Z>0.15</Z>
          </Start>
          <End>
            <Id>0</Id>
            <X>0.1736485380017706</X>
            <Y>-0.085000000011925148</Y>
            <Z>-1.0632555148446741E-17</Z>
          </End>
        </WeldData>
        <WeldData>
          <Id>8</Id>
          <Name />
          <Thickness>0.008</Thickness>
          <Material>S 355</Material>
          <WeldType>NotSpecified</WeldType>
          <ConnectedPartIds>
            <string>4</string>
            <string>6</string>
          </ConnectedPartIds>
          <Start>
            <Id>0</Id>
            <X>0.1736485380017706</X>
            <Y>0.084999999988074865</Y>
            <Z>0</Z>
          </Start>
          <End>
            <Id>0</Id>
            <X>0.1736485380017706</X>
            <Y>0.16499999998807485</Y>
            <Z>5.0035553639749363E-18</Z>
          </End>
        </WeldData>
        <WeldData>
          <Id>19</Id>
          <Name />
          <Thickness>0.008</Thickness>
          <Material>S 355</Material>
          <WeldType>NotSpecified</WeldType>
          <ConnectedPartIds>
            <string>1</string>
            <string>6</string>
          </ConnectedPartIds>
          <Start>
            <Id>0</Id>
            <X>0.1736485380017706</X>
            <Y>0.084999999988074865</Y>
            <Z>0.15</Z>
          </Start>
          <End>
            <Id>0</Id>
            <X>0.1736485380017706</X>
            <Y>0.084999999988074865</Y>
            <Z>0</Z>
          </End>
        </WeldData>
        <WeldData>
          <Id>10</Id>
          <Name />
          <Thickness>0.008</Thickness>
          <Material>S 355</Material>
          <WeldType>NotSpecified</WeldType>
          <ConnectedPartIds>
            <string>4</string>
            <string>7</string>
          </ConnectedPartIds>
          <Start>
            <Id>0</Id>
            <X>-0.17365146218896427</X>
            <Y>-0.085000000011925148</Y>
            <Z>1.0632734197357596E-17</Z>
          </Start>
          <End>
            <Id>0</Id>
            <X>-0.17365146218896427</X>
            <Y>-0.16500000001192516</Y>
            <Z>1.5636373819643525E-17</Z>
          </End>
        </WeldData>
        <WeldData>
          <Id>20</Id>
          <Name />
          <Thickness>0.008</Thickness>
          <Material>S 355</Material>
          <WeldType>NotSpecified</WeldType>
          <ConnectedPartIds>
            <string>1</string>
            <string>7</string>
          </ConnectedPartIds>
          <Start>
            <Id>0</Id>
            <X>-0.17365146218896427</X>
            <Y>-0.085000000011925148</Y>
            <Z>0.15</Z>
          </Start>
          <End>
            <Id>0</Id>
            <X>-0.17365146218896427</X>
            <Y>-0.085000000011925148</Y>
            <Z>1.0632734197357596E-17</Z>
          </End>
        </WeldData>
        <WeldData>
          <Id>12</Id>
          <Name />
          <Thickness>0.008</Thickness>
          <Material>S 355</Material>
          <WeldType>NotSpecified</WeldType>
          <ConnectedPartIds>
            <string>4</string>
            <string>8</string>
          </ConnectedPartIds>
          <Start>
            <Id>0</Id>
            <X>-0.17365146218896427</X>
            <Y>0.084999999988074865</Y>
            <Z>0</Z>
          </Start>
          <End>
            <Id>0</Id>
            <X>-0.17365146218896427</X>
            <Y>0.16499999998807485</Y>
            <Z>-5.0036396222859281E-18</Z>
          </End>
        </WeldData>
        <WeldData>
          <Id>21</Id>
          <Name />
          <Thickness>0.008</Thickness>
          <Material>S 355</Material>
          <WeldType>NotSpecified</WeldType>
          <ConnectedPartIds>
            <string>1</string>
            <string>8</string>
          </ConnectedPartIds>
          <Start>
            <Id>0</Id>
            <X>-0.17365146218896427</X>
            <Y>0.084999999988074865</Y>
            <Z>0.15</Z>
          </Start>
          <End>
            <Id>0</Id>
            <X>-0.17365146218896427</X>
            <Y>0.084999999988074865</Y>
            <Z>0</Z>
          </End>
        </WeldData>
        <WeldData>
          <Id>14</Id>
          <Name />
          <Thickness>0.008</Thickness>
          <Material>S 355</Material>
          <WeldType>NotSpecified</WeldType>
          <ConnectedPartIds>
            <string>4</string>
            <string>9</string>
          </ConnectedPartIds>
          <Start>
            <Id>0</Id>
            <X>-0.18000146209359683</X>
            <Y>-1.1925141429891539E-11</Y>
            <Z>1.0827142089963769E-17</Z>
          </Start>
          <End>
            <Id>0</Id>
            <X>-0.26000146209359681</X>
            <Y>-1.1925141429891539E-11</Y>
            <Z>1.3276375419423757E-17</Z>
          </End>
        </WeldData>
        <WeldData>
          <Id>15</Id>
          <Name />
          <Thickness>0.008</Thickness>
          <Material>S 355</Material>
          <WeldType>NotSpecified</WeldType>
          <ConnectedPartIds>
            <string>1</string>
            <string>9</string>
          </ConnectedPartIds>
          <Start>
            <Id>0</Id>
            <X>-0.18000146209359683</X>
            <Y>-1.1925141429891539E-11</Y>
            <Z>0.15</Z>
          </Start>
          <End>
            <Id>0</Id>
            <X>-0.18000146209359683</X>
            <Y>-1.1925141429891539E-11</Y>
            <Z>1.0827142089963769E-17</Z>
          </End>
        </WeldData>
        <WeldData>
          <Id>16</Id>
          <Name />
          <Thickness>0.008</Thickness>
          <Material>S 355</Material>
          <WeldType>NotSpecified</WeldType>
          <ConnectedPartIds>
            <string>4</string>
            <string>10</string>
          </ConnectedPartIds>
          <Start>
            <Id>0</Id>
            <X>0.17999853790640316</X>
            <Y>-1.1925141429891539E-11</Y>
            <Z>-1.9440789260617284E-19</Z>
          </Start>
          <End>
            <Id>0</Id>
            <X>0.25999853790640315</X>
            <Y>-1.1925141429891539E-11</Y>
            <Z>-2.64364122206616E-18</Z>
          </End>
        </WeldData>
        <WeldData>
          <Id>17</Id>
          <Name />
          <Thickness>0.008</Thickness>
          <Material>S 355</Material>
          <WeldType>NotSpecified</WeldType>
          <ConnectedPartIds>
            <string>1</string>
            <string>10</string>
          </ConnectedPartIds>
          <Start>
            <Id>0</Id>
            <X>0.17999853790640316</X>
            <Y>-1.1925141429891539E-11</Y>
            <Z>0.15</Z>
          </Start>
          <End>
            <Id>0</Id>
            <X>0.17999853790640316</X>
            <Y>-1.1925141429891539E-11</Y>
            <Z>-1.9440789260617284E-19</Z>
          </End>
        </WeldData>
      </Welds>
      <ConcreteBlocks>
        <ConcreteBlockData>
          <Id>1</Id>
          <Name>CB 1</Name>
          <Depth>0.6</Depth>
          <Material>C25/30</Material>
          <Center>
            <Id>0</Id>
            <X>-6.1230317691118863E-19</X>
            <Y>0</Y>
            <Z>-0.01</Z>
          </Center>
          <OutlinePoints>
            <Point2D>
              <X>0.46499999998807479</X>
              <Y>0.56000146209359691</Y>
            </Point2D>
            <Point2D>
              <X>-0.46500000001192515</X>
              <Y>0.56000146209359691</Y>
            </Point2D>
            <Point2D>
              <X>-0.46500000001192515</X>
              <Y>-0.55999853790640319</Y>
            </Point2D>
            <Point2D>
              <X>0.46499999998807479</X>
              <Y>-0.55999853790640319</Y>
            </Point2D>
            <Point2D>
              <X>0.46499999998807479</X>
              <Y>0.56000146209359691</Y>
            </Point2D>
          </OutlinePoints>
          <Origin>
            <Id>0</Id>
            <X>-1.2246063538223773E-18</X>
            <Y>0</Y>
            <Z>-0.02</Z>
          </Origin>
          <AxisX>
            <X>0</X>
            <Y>1</Y>
            <Z>0</Z>
          </AxisX>
          <AxisY>
            <X>-1</X>
            <Y>0</Y>
            <Z>6.1230317691118863E-17</Z>
          </AxisY>
          <AxisZ>
            <X>6.1230317691118863E-17</X>
            <Y>-0</Y>
            <Z>1</Z>
          </AxisZ>
          <Region>M 0.464999999988075 0.560001462093597 L -0.465000000011925 0.560001462093597 L -0.465000000011925 -0.559998537906403 L 0.464999999988075 -0.559998537906403 L 0.464999999988075 0.560001462093597</Region>
        </ConcreteBlockData>
      </ConcreteBlocks>
      <CutBeamByBeams />
    </ConnectionData>
    


```python
connDataModel = client.GetAllConnectionData(connection)
print(connDataModel)
```

    <?xml version="1.0" encoding="utf-16"?>
    <OpenModelContainer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
      <OpenModel>
        <Version>2</Version>
        <OriginSettings>
          <ProjectName>CON1</ProjectName>
          <ProjectDescription />
          <DateOfCreate>2022-11-23T12:21:58.2196109+01:00</DateOfCreate>
          <CrossSectionConversionTable>NoUsed</CrossSectionConversionTable>
          <CountryCode>ECEN</CountryCode>
          <ImportRecommendedWelds>false</ImportRecommendedWelds>
          <CheckEquilibrium>true</CheckEquilibrium>
        </OriginSettings>
        <Point3D>
          <Point3D>
            <Id>1</Id>
            <Name>1</Name>
            <X>0</X>
            <Y>0</Y>
            <Z>0</Z>
          </Point3D>
          <Point3D>
            <Id>2</Id>
            <Name>2</Name>
            <X>3.6738190614671314E-17</X>
            <Y>0</Y>
            <Z>0.6</Z>
          </Point3D>
        </Point3D>
        <LineSegment3D>
          <LineSegment3D>
            <Id>1</Id>
            <StartPoint>
              <TypeName>Point3D</TypeName>
              <Id>1</Id>
            </StartPoint>
            <EndPoint>
              <TypeName>Point3D</TypeName>
              <Id>2</Id>
            </EndPoint>
            <LocalCoordinateSystem xsi:type="CoordSystemByVector">
              <VecX>
                <X>6.1230317691118863E-17</X>
                <Y>0</Y>
                <Z>1</Z>
              </VecX>
              <VecY>
                <X>0</X>
                <Y>1</Y>
                <Z>0</Z>
              </VecY>
              <VecZ>
                <X>-1</X>
                <Y>0</Y>
                <Z>6.1230317691118863E-17</Z>
              </VecZ>
            </LocalCoordinateSystem>
          </LineSegment3D>
        </LineSegment3D>
        <ArcSegment3D />
        <PolyLine3D>
          <PolyLine3D>
            <Id>1</Id>
            <Segments>
              <ReferenceElement>
                <TypeName>LineSegment3D</TypeName>
                <Id>1</Id>
              </ReferenceElement>
            </Segments>
          </PolyLine3D>
        </PolyLine3D>
        <Region3D />
        <MatConcrete>
          <MatConcrete xsi:type="MatConcreteEc2">
            <Id>3</Id>
            <Name>C25/30</Name>
            <LoadFromLibrary>false</LoadFromLibrary>
            <E>31475806210.019348</E>
            <G>13114919254.174728</G>
            <Poisson>0.2</Poisson>
            <UnitMass>2500</UnitMass>
            <SpecificHeat>750</SpecificHeat>
            <ThermalExpansion>1E-05</ThermalExpansion>
            <ThermalConductivity>0.8</ThermalConductivity>
            <IsDefaultMaterial>true</IsDefaultMaterial>
            <OrderInCode>3</OrderInCode>
            <StateOfThermalExpansion>Code</StateOfThermalExpansion>
            <StateOfThermalConductivity>Code</StateOfThermalConductivity>
            <StateOfThermalSpecificHeat>Code</StateOfThermalSpecificHeat>
            <StateOfThermalStressStrain>Code</StateOfThermalStressStrain>
            <StateOfThermalStrain>Code</StateOfThermalStrain>
            <UserThermalSpecificHeatCurvature>
              <Points />
            </UserThermalSpecificHeatCurvature>
            <UserThermalConductivityCurvature>
              <Points />
            </UserThermalConductivityCurvature>
            <UserThermalExpansionCurvature>
              <Points />
            </UserThermalExpansionCurvature>
            <UserThermalStrainCurvature>
              <Points />
            </UserThermalStrainCurvature>
            <UserThermalStressStrainCurvature />
            <CalculateDependentValues>true</CalculateDependentValues>
            <Fck>25000000</Fck>
            <Ecm>31475806210.019348</Ecm>
            <Epsc1>0.0020693662482105194</Epsc1>
            <Epsc2>0.002</Epsc2>
            <Epsc3>0.00175</Epsc3>
            <Epscu1>0.0035</Epscu1>
            <Epscu2>0.0035</Epscu2>
            <Epscu3>0.0035</Epscu3>
            <Fctm>2564963.920015045</Fctm>
            <Fctk_0_05>1795474.7440105313</Fctk_0_05>
            <Fctk_0_95>3334453.0960195586</Fctk_0_95>
            <NFactor>2</NFactor>
            <Fcm>33000000</Fcm>
            <StoneDiameter>0.016</StoneDiameter>
            <CementClass>R</CementClass>
            <AggregateType>Quartzite</AggregateType>
            <DiagramType>Parabolic</DiagramType>
            <SilicaFume>false</SilicaFume>
            <PlainConcreteDiagram>false</PlainConcreteDiagram>
            <UserDiagram>
              <Points />
            </UserDiagram>
          </MatConcrete>
        </MatConcrete>
        <MatReinforcement />
        <MatSteel>
          <MatSteel xsi:type="MatSteelEc2">
            <Id>1</Id>
            <Name>S 355</Name>
            <LoadFromLibrary>false</LoadFromLibrary>
            <E>210000000000</E>
            <G>80769230769.230774</G>
            <Poisson>0.3</Poisson>
            <UnitMass>7850</UnitMass>
            <SpecificHeat>490</SpecificHeat>
            <ThermalExpansion>1.2E-05</ThermalExpansion>
            <ThermalConductivity>50.199999999999996</ThermalConductivity>
            <IsDefaultMaterial>true</IsDefaultMaterial>
            <OrderInCode>0</OrderInCode>
            <StateOfThermalExpansion>Code</StateOfThermalExpansion>
            <StateOfThermalConductivity>Code</StateOfThermalConductivity>
            <StateOfThermalSpecificHeat>Code</StateOfThermalSpecificHeat>
            <StateOfThermalStressStrain>Code</StateOfThermalStressStrain>
            <StateOfThermalStrain>Code</StateOfThermalStrain>
            <UserThermalSpecificHeatCurvature>
              <Points />
            </UserThermalSpecificHeatCurvature>
            <UserThermalConductivityCurvature>
              <Points />
            </UserThermalConductivityCurvature>
            <UserThermalExpansionCurvature>
              <Points />
            </UserThermalExpansionCurvature>
            <UserThermalStrainCurvature>
              <Points />
            </UserThermalStrainCurvature>
            <UserThermalStressStrainCurvature />
            <fy>355000000</fy>
            <fu>490000000</fu>
            <fy40>335000000</fy40>
            <fu40>470000000</fu40>
            <DiagramType>Bilinear</DiagramType>
            <UserDiagram>
              <Points />
            </UserDiagram>
            <MaterialStrength>
              <List />
            </MaterialStrength>
          </MatSteel>
        </MatSteel>
        <MatPrestressSteel />
        <CrossSection>
          <CrossSection xsi:type="CrossSectionParameter">
            <Id>3</Id>
            <Name>IPE360</Name>
            <CrossSectionRotation>0</CrossSectionRotation>
            <CrossSectionType>RolledI</CrossSectionType>
            <Parameters>
              <Parameter xsi:type="ParameterDouble">
                <Name>B</Name>
                <Value>0.17</Value>
              </Parameter>
              <Parameter xsi:type="ParameterDouble">
                <Name>H</Name>
                <Value>0.36</Value>
              </Parameter>
              <Parameter xsi:type="ParameterDouble">
                <Name>s</Name>
                <Value>0.008</Value>
              </Parameter>
              <Parameter xsi:type="ParameterDouble">
                <Name>t</Name>
                <Value>0.012699999809265138</Value>
              </Parameter>
              <Parameter xsi:type="ParameterDouble">
                <Name>r2</Name>
                <Value>0.018000000000000002</Value>
              </Parameter>
              <Parameter xsi:type="ParameterDouble">
                <Name>tapperF</Name>
                <Value>0</Value>
              </Parameter>
              <Parameter xsi:type="ParameterDouble">
                <Name>r1</Name>
                <Value>0.0001</Value>
              </Parameter>
            </Parameters>
            <Material>
              <TypeName>MatSteel</TypeName>
              <Id>1</Id>
            </Material>
          </CrossSection>
        </CrossSection>
        <ReinforcedCrossSection />
        <HingeElement1D />
        <Opening />
        <DappedEnd />
        <PatchDevice />
        <Element1D>
          <Element1D>
            <Id>1</Id>
            <Name>EB1</Name>
            <CrossSectionBegin>
              <TypeName>CrossSection</TypeName>
              <Id>3</Id>
            </CrossSectionBegin>
            <CrossSectionEnd>
              <TypeName>CrossSection</TypeName>
              <Id>3</Id>
            </CrossSectionEnd>
            <Segment>
              <TypeName>LineSegment3D</TypeName>
              <Id>1</Id>
            </Segment>
            <RotationRx>0</RotationRx>
            <EccentricityBeginX>0</EccentricityBeginX>
            <EccentricityBeginY>0</EccentricityBeginY>
            <EccentricityBeginZ>0</EccentricityBeginZ>
            <EccentricityEndX>0</EccentricityEndX>
            <EccentricityEndY>0</EccentricityEndY>
            <EccentricityEndZ>0</EccentricityEndZ>
          </Element1D>
        </Element1D>
        <Beam />
        <Member1D>
          <Member1D>
            <Id>1</Id>
            <Name>COL</Name>
            <Elements1D>
              <ReferenceElement>
                <TypeName>Element1D</TypeName>
                <Id>1</Id>
              </ReferenceElement>
            </Elements1D>
            <Member1DType>Beam</Member1DType>
            <Alignment>Center</Alignment>
            <MirrorY>false</MirrorY>
            <MirrorZ>false</MirrorZ>
          </Member1D>
        </Member1D>
        <Element2D />
        <Wall />
        <Member2D />
        <RigidLink />
        <PointOnLine3D />
        <PointSupportNode />
        <LineSupportSegment />
        <LoadsInPoint />
        <LoadsOnLine />
        <StrainLoadsOnLine />
        <PointLoadsOnLine />
        <LoadsOnSurface />
        <Settlements />
        <TemperatureLoadsOnLine />
        <LoadGroup>
          <LoadGroup xsi:type="LoadGroupEC">
            <Id>1</Id>
            <Name>LG1</Name>
            <Relation>Standard</Relation>
            <GroupType>Permanent</GroupType>
            <GammaQ>1.35</GammaQ>
            <Dzeta>0.85</Dzeta>
            <GammaGInf>1</GammaGInf>
            <GammaGSup>1.35</GammaGSup>
            <Psi0>0</Psi0>
            <Psi1>0</Psi1>
            <Psi2>0</Psi2>
          </LoadGroup>
        </LoadGroup>
        <LoadCase>
          <LoadCase>
            <Id>1</Id>
            <Name>LE1</Name>
            <LoadType>Permanent</LoadType>
            <Type>PermanentStandard</Type>
            <Variable>Standard</Variable>
            <LoadGroup>
              <TypeName>LoadGroup</TypeName>
              <Id>1</Id>
            </LoadGroup>
            <LoadsInPoint />
            <LoadsOnLine />
            <StrainLoadsOnLine />
            <PointLoadsOnLine />
            <LoadsOnSurface />
            <Settlements />
            <TemperatureLoadsOnLine />
          </LoadCase>
        </LoadCase>
        <CombiInput />
        <Attribute />
        <ConnectionPoint>
          <ConnectionPoint>
            <Id>1</Id>
            <Name>CON1</Name>
            <Node>
              <TypeName>Point3D</TypeName>
              <Id>1</Id>
            </Node>
            <ConnectedMembers>
              <ConnectedMember>
                <Id>1</Id>
                <MemberId>
                  <TypeName>Member1D</TypeName>
                  <Id>1</Id>
                </MemberId>
                <IsContinuous>false</IsContinuous>
                <Length>0</Length>
              </ConnectedMember>
            </ConnectedMembers>
            <NodeId>0</NodeId>
          </ConnectionPoint>
        </ConnectionPoint>
        <Connections>
          <ConnectionData>
            <ConenctionPointId>1</ConenctionPointId>
            <Beams>
              <BeamData>
                <Id>1</Id>
                <Name>COL</Name>
                <OriginalModelId>Beam1</OriginalModelId>
                <IsAdded>false</IsAdded>
                <AddedMemberLength>0</AddedMemberLength>
                <IsNegativeObject>false</IsNegativeObject>
                <MirrorY>false</MirrorY>
                <RefLineInCenterOfGravity>true</RefLineInCenterOfGravity>
                <IsBearingMember>false</IsBearingMember>
                <AutoAddCutByWorkplane>false</AutoAddCutByWorkplane>
              </BeamData>
            </Beams>
            <Plates>
              <PlateData>
                <Id>4</Id>
                <Name>BP1</Name>
                <Thickness>0.02</Thickness>
                <Material>S 355</Material>
                <Origin>
                  <Id>0</Id>
                  <X>-6.1230317691118863E-19</X>
                  <Y>0</Y>
                  <Z>-0.01</Z>
                </Origin>
                <AxisX>
                  <X>0</X>
                  <Y>1</Y>
                  <Z>0</Z>
                </AxisX>
                <AxisY>
                  <X>-1</X>
                  <Y>0</Y>
                  <Z>6.1230317691118863E-17</Z>
                </AxisY>
                <AxisZ>
                  <X>6.1230317691118863E-17</X>
                  <Y>-0</Y>
                  <Z>1</Z>
                </AxisZ>
                <Geometry>
                  <Outline>
                    <StartPoint>
                      <X>-0.16500000001192516</X>
                      <Y>-0.25999853790640315</Y>
                    </StartPoint>
                    <Segments>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>0.16499999998807485</X>
                          <Y>-0.25999853790640315</Y>
                        </EndPoint>
                      </Segment2D>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>0.16499999998807485</X>
                          <Y>0.26000146209359681</Y>
                        </EndPoint>
                      </Segment2D>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>-0.16500000001192516</X>
                          <Y>0.26000146209359681</Y>
                        </EndPoint>
                      </Segment2D>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>-0.16500000001192516</X>
                          <Y>-0.25999853790640315</Y>
                        </EndPoint>
                      </Segment2D>
                    </Segments>
                  </Outline>
                </Geometry>
                <OriginalModelId>Plate4</OriginalModelId>
                <IsNegativeObject>false</IsNegativeObject>
              </PlateData>
              <PlateData>
                <Id>5</Id>
                <Name>WID1a</Name>
                <Thickness>0.02</Thickness>
                <Material>S 355</Material>
                <Origin>
                  <Id>0</Id>
                  <X>0.1736485380017706</X>
                  <Y>-0.085000000011925148</Y>
                  <Z>-1.0632555148446741E-17</Z>
                </Origin>
                <AxisX>
                  <X>0</X>
                  <Y>-1</Y>
                  <Z>-6.25444420496867E-17</Z>
                </AxisX>
                <AxisY>
                  <X>6.1508200810257972E-17</X>
                  <Y>-6.25444420496867E-17</Y>
                  <Z>1</Z>
                </AxisY>
                <AxisZ>
                  <X>-1</X>
                  <Y>-3.8469961011576724E-33</Y>
                  <Z>6.1508200810257972E-17</Z>
                </AxisZ>
                <Geometry>
                  <Outline>
                    <StartPoint>
                      <X>0</X>
                      <Y>0</Y>
                    </StartPoint>
                    <Segments>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>0.080000000000000016</X>
                          <Y>1.5407439555097887E-33</Y>
                        </EndPoint>
                      </Segment2D>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>-9.3816663074530061E-18</X>
                          <Y>0.15</Y>
                        </EndPoint>
                      </Segment2D>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>0</X>
                          <Y>0</Y>
                        </EndPoint>
                      </Segment2D>
                    </Segments>
                  </Outline>
                </Geometry>
                <OriginalModelId>Plate5</OriginalModelId>
                <IsNegativeObject>false</IsNegativeObject>
              </PlateData>
              <PlateData>
                <Id>6</Id>
                <Name>WID1b</Name>
                <Thickness>0.02</Thickness>
                <Material>S 355</Material>
                <Origin>
                  <Id>0</Id>
                  <X>0.1736485380017706</X>
                  <Y>0.084999999988074865</Y>
                  <Z>0</Z>
                </Origin>
                <AxisX>
                  <X>-0</X>
                  <Y>1</Y>
                  <Z>6.25444420496867E-17</Z>
                </AxisX>
                <AxisY>
                  <X>6.1508200810257984E-17</X>
                  <Y>-6.25444420496867E-17</Y>
                  <Z>1</Z>
                </AxisY>
                <AxisZ>
                  <X>1</X>
                  <Y>3.8469961011576731E-33</Y>
                  <Z>-6.1508200810257984E-17</Z>
                </AxisZ>
                <Geometry>
                  <Outline>
                    <StartPoint>
                      <X>0</X>
                      <Y>0</Y>
                    </StartPoint>
                    <Segments>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>0.079999999999999988</X>
                          <Y>7.7037197775489434E-34</Y>
                        </EndPoint>
                      </Segment2D>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>9.3816663074530061E-18</X>
                          <Y>0.15</Y>
                        </EndPoint>
                      </Segment2D>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>0</X>
                          <Y>0</Y>
                        </EndPoint>
                      </Segment2D>
                    </Segments>
                  </Outline>
                </Geometry>
                <OriginalModelId>Plate6</OriginalModelId>
                <IsNegativeObject>false</IsNegativeObject>
              </PlateData>
              <PlateData>
                <Id>7</Id>
                <Name>WID1c</Name>
                <Thickness>0.02</Thickness>
                <Material>S 355</Material>
                <Origin>
                  <Id>0</Id>
                  <X>-0.17365146218896427</X>
                  <Y>-0.085000000011925148</Y>
                  <Z>1.0632734197357596E-17</Z>
                </Origin>
                <AxisX>
                  <X>0</X>
                  <Y>-1</Y>
                  <Z>6.25454952785741E-17</Z>
                </AxisX>
                <AxisY>
                  <X>6.1508200810257984E-17</X>
                  <Y>6.25454952785741E-17</Y>
                  <Z>1</Z>
                </AxisY>
                <AxisZ>
                  <X>-1</X>
                  <Y>3.847060883371578E-33</Y>
                  <Z>6.1508200810257984E-17</Z>
                </AxisZ>
                <Geometry>
                  <Outline>
                    <StartPoint>
                      <X>0</X>
                      <Y>0</Y>
                    </StartPoint>
                    <Segments>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>0.080000000000000016</X>
                          <Y>-7.7037197775489434E-34</Y>
                        </EndPoint>
                      </Segment2D>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>9.3818242917861144E-18</X>
                          <Y>0.15</Y>
                        </EndPoint>
                      </Segment2D>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>0</X>
                          <Y>0</Y>
                        </EndPoint>
                      </Segment2D>
                    </Segments>
                  </Outline>
                </Geometry>
                <OriginalModelId>Plate7</OriginalModelId>
                <IsNegativeObject>false</IsNegativeObject>
              </PlateData>
              <PlateData>
                <Id>8</Id>
                <Name>WID1d</Name>
                <Thickness>0.02</Thickness>
                <Material>S 355</Material>
                <Origin>
                  <Id>0</Id>
                  <X>-0.17365146218896427</X>
                  <Y>0.084999999988074865</Y>
                  <Z>0</Z>
                </Origin>
                <AxisX>
                  <X>-0</X>
                  <Y>1</Y>
                  <Z>-6.25454952785741E-17</Z>
                </AxisX>
                <AxisY>
                  <X>6.1508200810257984E-17</X>
                  <Y>6.25454952785741E-17</Y>
                  <Z>1</Z>
                </AxisY>
                <AxisZ>
                  <X>1</X>
                  <Y>-3.847060883371578E-33</Y>
                  <Z>-6.1508200810257984E-17</Z>
                </AxisZ>
                <Geometry>
                  <Outline>
                    <StartPoint>
                      <X>0</X>
                      <Y>0</Y>
                    </StartPoint>
                    <Segments>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>0.079999999999999988</X>
                          <Y>-7.7037197775489434E-34</Y>
                        </EndPoint>
                      </Segment2D>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>-9.3818242917861144E-18</X>
                          <Y>0.15</Y>
                        </EndPoint>
                      </Segment2D>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>0</X>
                          <Y>0</Y>
                        </EndPoint>
                      </Segment2D>
                    </Segments>
                  </Outline>
                </Geometry>
                <OriginalModelId>Plate8</OriginalModelId>
                <IsNegativeObject>false</IsNegativeObject>
              </PlateData>
              <PlateData>
                <Id>9</Id>
                <Name>WID1e</Name>
                <Thickness>0.02</Thickness>
                <Material>S 355</Material>
                <Origin>
                  <Id>0</Id>
                  <X>-0.18000146209359683</X>
                  <Y>-1.1925141429891539E-11</Y>
                  <Z>1.0827142089963769E-17</Z>
                </Origin>
                <AxisX>
                  <X>-1</X>
                  <Y>0</Y>
                  <Z>3.061541661824984E-17</Z>
                </AxisX>
                <AxisY>
                  <X>3.061541661824984E-17</X>
                  <Y>0</Y>
                  <Z>1</Z>
                </AxisY>
                <AxisZ>
                  <X>0</X>
                  <Y>1</Y>
                  <Z>-0</Z>
                </AxisZ>
                <Geometry>
                  <Outline>
                    <StartPoint>
                      <X>0</X>
                      <Y>0</Y>
                    </StartPoint>
                    <Segments>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>0.079999999999999988</X>
                          <Y>3.8518598887744717E-34</Y>
                        </EndPoint>
                      </Segment2D>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>4.592312492737476E-18</X>
                          <Y>0.15</Y>
                        </EndPoint>
                      </Segment2D>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>0</X>
                          <Y>0</Y>
                        </EndPoint>
                      </Segment2D>
                    </Segments>
                  </Outline>
                </Geometry>
                <OriginalModelId>Plate9</OriginalModelId>
                <IsNegativeObject>false</IsNegativeObject>
              </PlateData>
              <PlateData>
                <Id>10</Id>
                <Name>WID1f</Name>
                <Thickness>0.02</Thickness>
                <Material>S 355</Material>
                <Origin>
                  <Id>0</Id>
                  <X>0.17999853790640316</X>
                  <Y>-1.1925141429891539E-11</Y>
                  <Z>-1.9440789260617284E-19</Z>
                </Origin>
                <AxisX>
                  <X>1</X>
                  <Y>-0</Y>
                  <Z>-3.061541661824984E-17</Z>
                </AxisX>
                <AxisY>
                  <X>3.061541661824984E-17</X>
                  <Y>0</Y>
                  <Z>1</Z>
                </AxisY>
                <AxisZ>
                  <X>0</X>
                  <Y>-1</Y>
                  <Z>0</Z>
                </AxisZ>
                <Geometry>
                  <Outline>
                    <StartPoint>
                      <X>0</X>
                      <Y>0</Y>
                    </StartPoint>
                    <Segments>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>0.079999999999999988</X>
                          <Y>-3.8518598887744717E-34</Y>
                        </EndPoint>
                      </Segment2D>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>-4.592312492737476E-18</X>
                          <Y>0.15</Y>
                        </EndPoint>
                      </Segment2D>
                      <Segment2D xsi:type="LineSegment2D">
                        <EndPoint>
                          <X>0</X>
                          <Y>0</Y>
                        </EndPoint>
                      </Segment2D>
                    </Segments>
                  </Outline>
                </Geometry>
                <OriginalModelId>Plate10</OriginalModelId>
                <IsNegativeObject>false</IsNegativeObject>
              </PlateData>
            </Plates>
            <AnchorGrids>
              <AnchorGrid>
                <Id>1</Id>
                <IsAnchor>true</IsAnchor>
                <AnchorLen>0.3</AnchorLen>
                <HoleDiameter>0.018000000000000002</HoleDiameter>
                <Diameter>0.016</Diameter>
                <HeadDiameter>0.024</HeadDiameter>
                <DiagonalHeadDiameter>0.02675</DiagonalHeadDiameter>
                <HeadHeight>0.01</HeadHeight>
                <BoreHole>0.018000000000000002</BoreHole>
                <TensileStressArea>0.000157</TensileStressArea>
                <NutThickness>0.01445</NutThickness>
                <BoltAssemblyName>M16 8.8</BoltAssemblyName>
                <Material>8.8</Material>
                <Origin>
                  <Id>0</Id>
                  <X>-6.1230317691118863E-19</X>
                  <Y>0</Y>
                  <Z>-0.01</Z>
                </Origin>
                <AxisX>
                  <X>0</X>
                  <Y>1</Y>
                  <Z>0</Z>
                </AxisX>
                <AxisY>
                  <X>-1</X>
                  <Y>0</Y>
                  <Z>6.1230317691118863E-17</Z>
                </AxisY>
                <AxisZ>
                  <X>-6.1230317691118863E-17</X>
                  <Y>0</Y>
                  <Z>-1</Z>
                </AxisZ>
                <Positions>
                  <Point3D>
                    <Id>0</Id>
                    <X>-0.22000146209359683</X>
                    <Y>0.12499999998807487</Y>
                    <Z>-0.0099999999999999863</Z>
                  </Point3D>
                  <Point3D>
                    <Id>0</Id>
                    <X>-0.22000146209359683</X>
                    <Y>-0.12500000001192516</Y>
                    <Z>-0.0099999999999999863</Z>
                  </Point3D>
                  <Point3D>
                    <Id>0</Id>
                    <X>0.21999853790640317</X>
                    <Y>0.12499999998807487</Y>
                    <Z>-0.010000000000000014</Z>
                  </Point3D>
                  <Point3D>
                    <Id>0</Id>
                    <X>0.21999853790640317</X>
                    <Y>-0.12500000001192516</Y>
                    <Z>-0.010000000000000014</Z>
                  </Point3D>
                </Positions>
                <ConnectedPlates>
                  <int>4</int>
                </ConnectedPlates>
                <ConnectedPartIds>
                  <string>Plate4</string>
                </ConnectedPartIds>
                <ShearInThread>true</ShearInThread>
                <BoltInteraction>Bearing</BoltInteraction>
                <ConcreteBlock>
                  <Lenght>0.59999999999999987</Lenght>
                  <Width>0.60000000000000009</Width>
                  <Height>0.6</Height>
                  <Material>C25/30</Material>
                </ConcreteBlock>
                <AnchorType>Straight</AnchorType>
                <WasherSize>0.1</WasherSize>
              </AnchorGrid>
            </AnchorGrids>
            <Welds>
              <WeldData>
                <Id>3</Id>
                <Name />
                <Thickness>0.008</Thickness>
                <Material>S 355</Material>
                <WeldType>DoubleFillet</WeldType>
                <ConnectedPartIds>
                  <string>Plate4</string>
                  <string>Beam1</string>
                </ConnectedPartIds>
                <Start>
                  <Id>0</Id>
                  <X>0.1736485380017706</X>
                  <Y>0.084999999988074865</Y>
                  <Z>0</Z>
                </Start>
                <End>
                  <Id>0</Id>
                  <X>0.1736485380017706</X>
                  <Y>-0.085000000011925148</Y>
                  <Z>-1.0632555148446741E-17</Z>
                </End>
              </WeldData>
              <WeldData>
                <Id>4</Id>
                <Name />
                <Thickness>0.008</Thickness>
                <Material>S 355</Material>
                <WeldType>DoubleFillet</WeldType>
                <ConnectedPartIds>
                  <string>Plate4</string>
                  <string>Beam1</string>
                </ConnectedPartIds>
                <Start>
                  <Id>0</Id>
                  <X>-0.17365146218896427</X>
                  <Y>0.084999999988074865</Y>
                  <Z>0</Z>
                </Start>
                <End>
                  <Id>0</Id>
                  <X>-0.17365146218896427</X>
                  <Y>-0.085000000011925148</Y>
                  <Z>1.0632734197357596E-17</Z>
                </End>
              </WeldData>
              <WeldData>
                <Id>5</Id>
                <Name />
                <Thickness>0.006</Thickness>
                <Material>S 355</Material>
                <WeldType>DoubleFillet</WeldType>
                <ConnectedPartIds>
                  <string>Plate4</string>
                  <string>Beam1</string>
                </ConnectedPartIds>
                <Start>
                  <Id>0</Id>
                  <X>0.1736485380017706</X>
                  <Y>-1.1925141429891539E-11</Y>
                  <Z>0</Z>
                </Start>
                <End>
                  <Id>0</Id>
                  <X>-0.17365146218896427</X>
                  <Y>-1.1925141429891539E-11</Y>
                  <Z>1.0632734197357596E-17</Z>
                </End>
              </WeldData>
              <WeldData>
                <Id>6</Id>
                <Name />
                <Thickness>0.008</Thickness>
                <Material>S 355</Material>
                <WeldType>DoubleFillet</WeldType>
                <ConnectedPartIds>
                  <string>Plate4</string>
                  <string>Plate5</string>
                </ConnectedPartIds>
                <Start>
                  <Id>0</Id>
                  <X>0.1736485380017706</X>
                  <Y>-0.085000000011925148</Y>
                  <Z>-1.0632555148446741E-17</Z>
                </Start>
                <End>
                  <Id>0</Id>
                  <X>0.1736485380017706</X>
                  <Y>-0.16500000001192516</Y>
                  <Z>-1.5636110512421676E-17</Z>
                </End>
              </WeldData>
              <WeldData>
                <Id>18</Id>
                <Name />
                <Thickness>0.008</Thickness>
                <Material>S 355</Material>
                <WeldType>Bevel</WeldType>
                <ConnectedPartIds>
                  <string>Beam1</string>
                  <string>Plate5</string>
                </ConnectedPartIds>
                <Start>
                  <Id>0</Id>
                  <X>0.1736485380017706</X>
                  <Y>-0.085000000011925148</Y>
                  <Z>0.15</Z>
                </Start>
                <End>
                  <Id>0</Id>
                  <X>0.1736485380017706</X>
                  <Y>-0.085000000011925148</Y>
                  <Z>-1.0632555148446741E-17</Z>
                </End>
              </WeldData>
              <WeldData>
                <Id>8</Id>
                <Name />
                <Thickness>0.008</Thickness>
                <Material>S 355</Material>
                <WeldType>DoubleFillet</WeldType>
                <ConnectedPartIds>
                  <string>Plate4</string>
                  <string>Plate6</string>
                </ConnectedPartIds>
                <Start>
                  <Id>0</Id>
                  <X>0.1736485380017706</X>
                  <Y>0.084999999988074865</Y>
                  <Z>0</Z>
                </Start>
                <End>
                  <Id>0</Id>
                  <X>0.1736485380017706</X>
                  <Y>0.16499999998807485</Y>
                  <Z>5.0035553639749363E-18</Z>
                </End>
              </WeldData>
              <WeldData>
                <Id>19</Id>
                <Name />
                <Thickness>0.008</Thickness>
                <Material>S 355</Material>
                <WeldType>Bevel</WeldType>
                <ConnectedPartIds>
                  <string>Beam1</string>
                  <string>Plate6</string>
                </ConnectedPartIds>
                <Start>
                  <Id>0</Id>
                  <X>0.1736485380017706</X>
                  <Y>0.084999999988074865</Y>
                  <Z>0.15</Z>
                </Start>
                <End>
                  <Id>0</Id>
                  <X>0.1736485380017706</X>
                  <Y>0.084999999988074865</Y>
                  <Z>0</Z>
                </End>
              </WeldData>
              <WeldData>
                <Id>10</Id>
                <Name />
                <Thickness>0.008</Thickness>
                <Material>S 355</Material>
                <WeldType>DoubleFillet</WeldType>
                <ConnectedPartIds>
                  <string>Plate4</string>
                  <string>Plate7</string>
                </ConnectedPartIds>
                <Start>
                  <Id>0</Id>
                  <X>-0.17365146218896427</X>
                  <Y>-0.085000000011925148</Y>
                  <Z>1.0632734197357596E-17</Z>
                </Start>
                <End>
                  <Id>0</Id>
                  <X>-0.17365146218896427</X>
                  <Y>-0.16500000001192516</Y>
                  <Z>1.5636373819643525E-17</Z>
                </End>
              </WeldData>
              <WeldData>
                <Id>20</Id>
                <Name />
                <Thickness>0.008</Thickness>
                <Material>S 355</Material>
                <WeldType>Bevel</WeldType>
                <ConnectedPartIds>
                  <string>Beam1</string>
                  <string>Plate7</string>
                </ConnectedPartIds>
                <Start>
                  <Id>0</Id>
                  <X>-0.17365146218896427</X>
                  <Y>-0.085000000011925148</Y>
                  <Z>0.15</Z>
                </Start>
                <End>
                  <Id>0</Id>
                  <X>-0.17365146218896427</X>
                  <Y>-0.085000000011925148</Y>
                  <Z>1.0632734197357596E-17</Z>
                </End>
              </WeldData>
              <WeldData>
                <Id>12</Id>
                <Name />
                <Thickness>0.008</Thickness>
                <Material>S 355</Material>
                <WeldType>DoubleFillet</WeldType>
                <ConnectedPartIds>
                  <string>Plate4</string>
                  <string>Plate8</string>
                </ConnectedPartIds>
                <Start>
                  <Id>0</Id>
                  <X>-0.17365146218896427</X>
                  <Y>0.084999999988074865</Y>
                  <Z>0</Z>
                </Start>
                <End>
                  <Id>0</Id>
                  <X>-0.17365146218896427</X>
                  <Y>0.16499999998807485</Y>
                  <Z>-5.0036396222859281E-18</Z>
                </End>
              </WeldData>
              <WeldData>
                <Id>21</Id>
                <Name />
                <Thickness>0.008</Thickness>
                <Material>S 355</Material>
                <WeldType>Bevel</WeldType>
                <ConnectedPartIds>
                  <string>Beam1</string>
                  <string>Plate8</string>
                </ConnectedPartIds>
                <Start>
                  <Id>0</Id>
                  <X>-0.17365146218896427</X>
                  <Y>0.084999999988074865</Y>
                  <Z>0.15</Z>
                </Start>
                <End>
                  <Id>0</Id>
                  <X>-0.17365146218896427</X>
                  <Y>0.084999999988074865</Y>
                  <Z>0</Z>
                </End>
              </WeldData>
              <WeldData>
                <Id>14</Id>
                <Name />
                <Thickness>0.008</Thickness>
                <Material>S 355</Material>
                <WeldType>DoubleFillet</WeldType>
                <ConnectedPartIds>
                  <string>Plate4</string>
                  <string>Plate9</string>
                </ConnectedPartIds>
                <Start>
                  <Id>0</Id>
                  <X>-0.18000146209359683</X>
                  <Y>-1.1925141429891539E-11</Y>
                  <Z>1.0827142089963769E-17</Z>
                </Start>
                <End>
                  <Id>0</Id>
                  <X>-0.26000146209359681</X>
                  <Y>-1.1925141429891539E-11</Y>
                  <Z>1.3276375419423757E-17</Z>
                </End>
              </WeldData>
              <WeldData>
                <Id>15</Id>
                <Name />
                <Thickness>0.008</Thickness>
                <Material>S 355</Material>
                <WeldType>DoubleFillet</WeldType>
                <ConnectedPartIds>
                  <string>Beam1</string>
                  <string>Plate9</string>
                </ConnectedPartIds>
                <Start>
                  <Id>0</Id>
                  <X>-0.18000146209359683</X>
                  <Y>-1.1925141429891539E-11</Y>
                  <Z>0.15</Z>
                </Start>
                <End>
                  <Id>0</Id>
                  <X>-0.18000146209359683</X>
                  <Y>-1.1925141429891539E-11</Y>
                  <Z>1.0827142089963769E-17</Z>
                </End>
              </WeldData>
              <WeldData>
                <Id>16</Id>
                <Name />
                <Thickness>0.008</Thickness>
                <Material>S 355</Material>
                <WeldType>DoubleFillet</WeldType>
                <ConnectedPartIds>
                  <string>Plate4</string>
                  <string>Plate10</string>
                </ConnectedPartIds>
                <Start>
                  <Id>0</Id>
                  <X>0.17999853790640316</X>
                  <Y>-1.1925141429891539E-11</Y>
                  <Z>-1.9440789260617284E-19</Z>
                </Start>
                <End>
                  <Id>0</Id>
                  <X>0.25999853790640315</X>
                  <Y>-1.1925141429891539E-11</Y>
                  <Z>-2.64364122206616E-18</Z>
                </End>
              </WeldData>
              <WeldData>
                <Id>17</Id>
                <Name />
                <Thickness>0.008</Thickness>
                <Material>S 355</Material>
                <WeldType>DoubleFillet</WeldType>
                <ConnectedPartIds>
                  <string>Beam1</string>
                  <string>Plate10</string>
                </ConnectedPartIds>
                <Start>
                  <Id>0</Id>
                  <X>0.17999853790640316</X>
                  <Y>-1.1925141429891539E-11</Y>
                  <Z>0.15</Z>
                </Start>
                <End>
                  <Id>0</Id>
                  <X>0.17999853790640316</X>
                  <Y>-1.1925141429891539E-11</Y>
                  <Z>-1.9440789260617284E-19</Z>
                </End>
              </WeldData>
            </Welds>
            <ConcreteBlocks>
              <ConcreteBlockData>
                <Id>1</Id>
                <Name>CB 1</Name>
                <Depth>0.6</Depth>
                <Material>C25/30</Material>
                <Center>
                  <Id>0</Id>
                  <X>-6.1230317691118863E-19</X>
                  <Y>0</Y>
                  <Z>-0.01</Z>
                </Center>
                <OutlinePoints>
                  <Point2D>
                    <X>0.46499999998807479</X>
                    <Y>0.56000146209359691</Y>
                  </Point2D>
                  <Point2D>
                    <X>-0.46500000001192515</X>
                    <Y>0.56000146209359691</Y>
                  </Point2D>
                  <Point2D>
                    <X>-0.46500000001192515</X>
                    <Y>-0.55999853790640319</Y>
                  </Point2D>
                  <Point2D>
                    <X>0.46499999998807479</X>
                    <Y>-0.55999853790640319</Y>
                  </Point2D>
                  <Point2D>
                    <X>0.46499999998807479</X>
                    <Y>0.56000146209359691</Y>
                  </Point2D>
                </OutlinePoints>
                <Origin>
                  <Id>0</Id>
                  <X>-1.2246063538223773E-18</X>
                  <Y>0</Y>
                  <Z>-0.02</Z>
                </Origin>
                <AxisX>
                  <X>0</X>
                  <Y>1</Y>
                  <Z>0</Z>
                </AxisX>
                <AxisY>
                  <X>-1</X>
                  <Y>0</Y>
                  <Z>6.1230317691118863E-17</Z>
                </AxisY>
                <AxisZ>
                  <X>6.1230317691118863E-17</X>
                  <Y>-0</Y>
                  <Z>1</Z>
                </AxisZ>
                <Region>M 0.464999999988075 0.560001462093597 L -0.465000000011925 0.560001462093597 L -0.465000000011925 -0.559998537906403 L 0.464999999988075 -0.559998537906403 L 0.464999999988075 0.560001462093597</Region>
              </ConcreteBlockData>
            </ConcreteBlocks>
            <CutBeamByBeams />
          </ConnectionData>
        </Connections>
        <Reinforcement />
        <ISDModel />
        <InitialImperfectionOfPoint />
        <Tendon />
        <ResultClass />
        <DesignMember />
        <SubStructure />
        <ConnectionSetup>
          <SteelSetup xsi:type="SteelSetupECEN">
            <GammaM0>1</GammaM0>
            <GammaM1>1</GammaM1>
            <GammaM2>1.25</GammaM2>
            <GammaMfi>1</GammaMfi>
            <GammaMu>1.1</GammaMu>
          </SteelSetup>
          <StopAtLimitStrain>true</StopAtLimitStrain>
          <WeldEvaluationData>ApplyPlasticWelds</WeldEvaluationData>
          <CheckDetailing>false</CheckDetailing>
          <ApplyConeBreakoutCheck>Both</ApplyConeBreakoutCheck>
          <PretensionForceFpc>0.7</PretensionForceFpc>
          <GammaInst>1.2</GammaInst>
          <GammaC>1.5</GammaC>
          <GammaM3>1.25</GammaM3>
          <AnchorLengthForStiffness>8</AnchorLengthForStiffness>
          <JointBetaFactor>0.67</JointBetaFactor>
          <EffectiveAreaStressCoeff>0.1</EffectiveAreaStressCoeff>
          <EffectiveAreaStressCoeffAISC>0.1</EffectiveAreaStressCoeffAISC>
          <FrictionCoefficient>0.25</FrictionCoefficient>
          <LimitPlasticStrain>0.05</LimitPlasticStrain>
          <LimitDeformation>0.03</LimitDeformation>
          <LimitDeformationCheck>false</LimitDeformationCheck>
          <AnalysisGNL>true</AnalysisGNL>
          <WarnPlasticStrain>0.03</WarnPlasticStrain>
          <WarnCheckLevel>0.95</WarnCheckLevel>
          <OptimalCheckLevel>0.6</OptimalCheckLevel>
          <DistanceBetweenBolts>2.2</DistanceBetweenBolts>
          <DistanceDiameterBetweenBP>4</DistanceDiameterBetweenBP>
          <DistanceBetweenBoltsEdge>1.2</DistanceBetweenBoltsEdge>
          <BearingAngle>0.46373398225489332</BearingAngle>
          <DecreasingFtrd>0.85</DecreasingFtrd>
          <BracedSystem>false</BracedSystem>
          <BearingCheck>false</BearingCheck>
          <ApplyBetapInfluence>false</ApplyBetapInfluence>
          <MemberLengthRatio>2</MemberLengthRatio>
          <DivisionOfSurfaceOfCHS>64</DivisionOfSurfaceOfCHS>
          <DivisionOfArcsOfRHS>3</DivisionOfArcsOfRHS>
          <NumElement>8</NumElement>
          <NumberIterations>25</NumberIterations>
          <Mdiv>3</Mdiv>
          <MinSize>0.01</MinSize>
          <MaxSize>0.05</MaxSize>
          <NumElementRhs>24</NumElementRhs>
          <RigidBP>false</RigidBP>
          <AlphaCC>1</AlphaCC>
          <CrackedConcrete>true</CrackedConcrete>
          <DevelopedFillers>false</DevelopedFillers>
          <DeformationBoltHole>true</DeformationBoltHole>
          <ExtensionLengthRationOpenSections>1.25</ExtensionLengthRationOpenSections>
          <ExtensionLengthRationCloseSections>1.25</ExtensionLengthRationCloseSections>
          <FactorPreloadBolt>0.7</FactorPreloadBolt>
          <BaseMetalCapacity>false</BaseMetalCapacity>
          <ApplyBearingCheck>false</ApplyBearingCheck>
          <FrictionCoefficientPbolt>0.3</FrictionCoefficientPbolt>
          <CrtCompCheckIS>IS800_Cl_7_4</CrtCompCheckIS>
          <BoltMaxGripLengthCoeff>0</BoltMaxGripLengthCoeff>
          <FatigueSectionOffset>1.5</FatigueSectionOffset>
          <CondensedElementLengthFactor>4</CondensedElementLengthFactor>
          <GammaMu>1.1</GammaMu>
        </ConnectionSetup>
        <CheckMember />
        <ConcreteCheckSection />
        <RebarShape />
        <RebarGeneral />
        <RebarSingle />
        <RebarStirrups />
        <Taper />
        <Span />
      </OpenModel>
      <OpenModelResult>
        <ResultOnMembers>
          <ResultOnMembers>
            <Members>
              <ResultOnMember>
                <Member>
                  <MemberType>Member1D</MemberType>
                  <Id>1</Id>
                </Member>
                <ResultType>InternalForces</ResultType>
                <LocalSystemType>Principle</LocalSystemType>
                <Results>
                  <ResultBase xsi:type="ResultOnSection">
                    <AbsoluteRelative>Relative</AbsoluteRelative>
                    <Position>0</Position>
                    <Results>
                      <SectionResultBase xsi:type="ResultOfInternalForces">
                        <Loading>
                          <LoadingType>LoadCase</LoadingType>
                          <Id>1</Id>
                          <Items>
                            <ResultOfLoadingItem>
                              <Coefficient>1</Coefficient>
                            </ResultOfLoadingItem>
                          </Items>
                        </Loading>
                        <N>-300000</N>
                        <Qy>-0</Qy>
                        <Qz>-0</Qz>
                        <Mx>0</Mx>
                        <My>-70000</My>
                        <Mz>8000</Mz>
                      </SectionResultBase>
                    </Results>
                  </ResultBase>
                  <ResultBase xsi:type="ResultOnSection">
                    <AbsoluteRelative>Relative</AbsoluteRelative>
                    <Position>1</Position>
                    <Results>
                      <SectionResultBase xsi:type="ResultOfInternalForces">
                        <Loading>
                          <LoadingType>LoadCase</LoadingType>
                          <Id>1</Id>
                          <Items>
                            <ResultOfLoadingItem>
                              <Coefficient>1</Coefficient>
                            </ResultOfLoadingItem>
                          </Items>
                        </Loading>
                        <N>-300000</N>
                        <Qy>-0</Qy>
                        <Qz>-0</Qz>
                        <Mx>0</Mx>
                        <My>-70000</My>
                        <Mz>8000</Mz>
                      </SectionResultBase>
                    </Results>
                  </ResultBase>
                </Results>
              </ResultOnMember>
            </Members>
          </ResultOnMembers>
        </ResultOnMembers>
      </OpenModelResult>
    </OpenModelContainer>
    

## Apply Templates

We will learn more about applying templates in the next session 