# Expression Parameter Reference

The below provides further information on using expressions in IDEA StatiCa Connection. More general information on using parameters can be found [here](api_parameters_getting_started.md).

## Introduction

We use a third-party expression module called NCalc to enable the evaluation of expressions both in the Parameters functionality (developer tab) and special evaluation calls available in the Connection API. 

 > The below is an extension to the [ncalc documentation](https://github.com/ncalc/ncalc/wiki) where all standard math, logical operators and functions are explained.

On top of the out-of-the-box functions that are provided, several additional expressions functions are available within the Connection App to extend functionally specific to the IDEA StatiCa connection. These include providing functions that can reference existing geometric properties in a given connection, which can allow the driving of other specific connection detailing parameters.

## Simple Functions

The below table provides the basic expression functions typically required to enable the use of parameters: 

| **Expression** | **Description** | **Example Input** | **Example Ouput** |
| --- | -- | ---| --- |
| **ToString(..)** | Converts a number to a string |  ToString(2.1578) | `'2.1578'`
| **ToString(..)** | Concatenates multiple strings into one output |  ToString('M',20,' ','8.8') | `'M20 8.8'`
| **ToString(..)** | Each comma delineated input is also separately evaluated. `spacing` and `bolts` are previously defined parameters |  ToString([spacing],'*',[bolts]-1) | `'.075*3'`
| **Round(..)** | rounds a decimal number to an Integer |  Round(2.1578, 0) | `2` |
| **Round(..)** | rounds a decimal number to a precision three |  Round(2.1578, 3) | `2.158` |


## Model Property Reference Functions

Model property functions are an advanced feature, and should be limited where possible. Further enhancement of these will be made in the future.

### Member Property Functions

The `GetValue()` Function can be used to extract model properties from the connection model. This enables the extraction of member information such as cross-section information, member geometry, etc. 

| **Expression** | **Description** | **Example Input** | **Example Ouput** |
| --- | -- | ---| --- |
| **GetValue(..)** | Gets a property associated with a member in the model, where B is the Member Name  |  GetValue('B', 'cssWidth') | `89`

> In all examples below 'B' represents the Name of the referenced member in the Connection Project

#### Member Cross Section Bounds

Item | Expression | Comment
---------|----------|---------
Height | `GetValue('B', 'CrossSection.Bounds.Height')` | 
Width | `GetValue('B', 'CrossSection.Bounds.Width')` | 
Bottom | `GetValue('B', 'CrossSection.Bounds.Bottom')` | 	 
Left | `GetValue('B', 'CrossSection.Bounds.Left')` | 	  		
Right | `GetValue('B', 'CrossSection.Bounds.Right')` | 	  		 		
Top | `GetValue('B', 'CrossSection.Bounds.Top')` |  		

#### Direct Access Member Properties

Item | Expression | Comment
---------|----------|---------
Flange Thickness | `GetValue('B', 'FlangeThickness')` | 
Web Thickness | `GetValue('B', 'WebThickness')` | 
InsertPointOnRefLine | `GetValue('B', 'InsertPointOnRefLine.X')` | 
DirVectX | `GetValue('B', 'DirVectX')` | 
DirVectY | `GetValue('B', 'DirVectY')` | 
DirVectZ | `GetValue('B', 'DirVectZ')` | 
Length | `GetValue('B', 'Length')` | 
Support Code | `GetValue('B', 'SupportCode')` | 
Material Name | `GetValue('B','MaterialName')` | 
Angle Alpha | `GetValue('B', 'AngleAlpha')` | 
Angel Beta | `GetValue('B','AngleBeta')` | 
Theoretical Length Y | `GetValue('B', 'TheoreticalLengthY')` | 
Theoretical Length Z | `GetValue('B', 'TheoreticalLengthZ')` | 

#### Further Member Properties 

Item | Expression | Comment
---------|----------|---------
Is Bearing Member | `GetValue('B','IsBearingMember')` | Gets the flag of bearing member
Connection Point - X | `GetValue('B', 'ConnectionPoint.X')` | Gets the point of connection of this segment to the joint
Connection Point - Y | `GetValue('B', 'ConnectionPoint.Y')` | 
Connection Point - Z | `GetValue('B', 'ConnectionPoint.Z')` | 
Position On Ref Line | `GetValue('B', 'PositionOnRefLine')` | Gets the relative position of the ConnectionPoint on the ReferenceLine
Calculated Pin Position | `GetValue('B', 'CalculatedPinPosition')` | Gets the position of the calculated pinned connection. The position is measured on the local X-axis of the beam. The calculation is done according to the sizes and positions of bolts on this member
Bounding Box in LCS - Height  | `GetValue('B', 'BoundingBoxInLcs.Height')` | Gets bounding box in the local coordinate system of the beam
Bounding Box in LCS - Width  | `GetValue('B', 'BoundingBoxInLcs.Width)` | 
Bounding Box in LCS - Length  | `GetValue('B', 'BoundingBoxInLcs.Length)` | 

#### Beam Relating Section Properties

Item | Expression | Comment
---------|----------|---------
Area | `GetValue('B', 'CrossSection.CssCharact.A')` | Area of Cross Section
Av1 |  `GetValue('B','CrossSection.CssCharact.Av1')` | Shear area Av1 (major principal axis)
Av2 | `GetValue('B','CrossSection.CssCharact.Av2')` | Shear area Av2 (minor principal axis)
Sx | `GetValue('B', 'CrossSection.CssCharact.Sx')` | The first moment of area related to the X axis
Sy | `GetValue('B', 'CrossSection.CssCharact.Sy')` | The first moment of area related to the Y axis
Ix | `GetValue('B', 'CrossSection.CssCharact.Ix')` | The second moment of area related to the X axis 		
Iy | `GetValue('B', 'CrossSection.CssCharact.Iy')` | The second moment of area related to the Y axis 		 		
I1 | `GetValue('B', 'CrossSection.CssCharact.I1')` | The principal second moment of area related to the first principal axis
I2 | `GetValue('B', 'CrossSection.CssCharact.I2')` | The principal second moment of area related to the second principal axis
Alpha | `GetValue('B', 'CrossSection.CssCharact.Alpha')` | Gets the angle, in radians, between system and principal axes		
It | `GetValue('B', 'CrossSection.CssCharact.It')` | Sant Vennant torsional constant   
Ixy | `GetValue('B', 'CrossSection.CssCharact.Ixy')` | The product moment of area
Cgx | `GetValue('B', 'CrossSection.CssCharact.Cgx')` | Gets the centre of gravity related to the X axis
Cgy | `GetValue('B', 'CrossSection.CssCharact.Cgy')` | Gets the centre of gravity related to the Y axis
Painting Surface | `GetValue('B', 'CrossSection.CssCharact.PaintingSurface')` | Painting surface as surface of 1m long part of beam with that cros section
Wpl1 | `GetValue('B', 'CrossSection.CssCharact.Wpl1')` | Plastic modulus related to major principal axis
Wpl2 | `GetValue('B', 'CrossSection.CssCharact.Wpl2')` | Plastic modulus related to minor principal axis
Wel1 | `GetValue('B', 'CrossSection.CssCharact.Wel1')` | Section modulus related to major principal axis
Wel2 | `GetValue('B', 'CrossSection.CssCharact.Wel2')` | Section modulus related to minor principal axis
Iw | `GetValue('B', 'CrossSection.CssCharact.Iw')` | Warping Constant
C | `GetValue('B', 'CrossSection.CssCharact.C')` | Torsion Constant
Rgx | `GetValue('B', 'CrossSection.CssCharact.Rgx')` | Gets the radius of gyration related to the X axis
Rgy | `GetValue('B', 'CrossSection.CssCharact.Rgy')` | Gets the radius of gyration related to the Y axis
Rg1 | `GetValue('B', 'CrossSection.CssCharact.Rg1')` | Gets the radius of gyration related to the first principal axis
Rg2 | `GetValue('B', 'CrossSection.CssCharact.Rg2')` | Gets the radius of gyration related to the second principal axis
x0 | `GetValue('B', 'CrossSection.CssCharact.x0')` | Shear centre distance from centroidal point in X direction
y0 | `GetValue('B', 'CrossSection.CssCharact.y0')` | Shear centre distance from centroidal point in Y direction

#### Beam Relating Section Material

Item | Expression | Comment
---------|----------|---------
E | `GetValue('B', 'Material.E')` | Young's modulus
G | `GetValue('B', 'Material.G')` | Shear modulus
Poisson | `GetValue('B', 'Material.Poissons')` | Poisson's ratio
Unit Mass | `GetValue('B', 'Material.UnitMass')` | UnitMass
Specific Heat | `GetValue('B', 'Material.SpecificHeat')` | Specific heat capacity
Thermal Expansion | `GetValue('B', 'Material.ThermalExpansion')` | Thermal Expansion
Thermal Conductivity | `GetValue('B', 'Material.ThermalConductivity')` | Thermal Conductivity
Is Default Materail | `GetValue('B', 'Material.IsDefaultMaterial')` | True if material is default material from the code

### Operation Reference Functions

> Currently operation reference functions only exist for internal use. Please let us know what model information you require to retrieve from operations and so we can add appropriate public functions. 
