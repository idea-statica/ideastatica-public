# Expression Parameter Reference

The below provides further information on using parameter expressions in IDEA StatiCa developer tab. More general information on using parameters can be found [here](api_parameters_getting_started.md).

## Introduction

This page provides a comprehensive list of the operations and functions avaliable within the developer tab parameter functionality.

On top of the standard functions that you would expect when using formulas or expressions, several special functions are provied to extend functionally specific to IDEA StatiCa applications. These include providing functions that can reference existing member geometric properties, project settings values, forces in a given connection as well as inputing and converting units.

All together they provide for a very powerful way of driving designs in a parametric fashion.

## Operators

### Standard operators

| **Expression** | **Description** | **Example Input** | **Example Ouput** |
| --- | -- | ---| --- |
| **+** | addition | `2.3 + 2` | 4.3d
| **-** | subtraction | `2.3 - 2` | 0.3d
| **\*** | multiplication | `2.5*3.1` | 7.75d
| **\\** | division | `5/4` | 1.25d
| **%** | remainder division | `9/4` | 1

### Logical

Below is a list of the standard logical operators avaliable.

| **Expression** | **Description** | **Example Input** | **Example Ouput** |
| --- | -- | ---| --- |
| **or, \|\|** | Condition testing based on or | `true || true`, `true or false` | true, true 
| **and, &&** | Condition testing based on and | `true && true`, `true and false` | true, false

### Relational

Below is a list of the standard relational operators avaliable.

| **Expression** | **Description** | **Example Input** | **Example Ouput** |
| --- | -- | ---| --- |
| **n = x, n == x** | n is equal to x | `5 == 5` | True
| **n != x** | n does not equal to x | `3 != 5` | True
| **n < x < j** | x is within bounds of n and j | `3 < 4 < 5` | True
| **n < x** | x is less than n | `3 < 4`| True
| **n <= x** | x is less or equal to n | `3 <= 4`, `3<=3` | True
| **x > n** | x is greater than n | `4 > 3` | True
| **x >= n** | x is greater than or equal to n | `4 > 3` | True



### Unary

| **Expression** | **Description** | **Example Input** | **Example Ouput** |
| --- | -- | ---| --- |
| **!, not** | reverse the meaning or logic of a given condition or object | `!true`, `not true` | False
| **-** | logical negation symbol | (given x=5) `-x` | -5
| **~** | Bitwise not |  | 


<!-- ### Bitwise

| **Expression** | **Description** | **Example Input** | **Example Ouput** |
| --- | -- | ---| --- |
| **&** | bitwise and |  | 
| **\|** | bitwise or |  | 
| **^** | Bitwise xor |  | 
| **<<** | left shift |  | 
| **>>** | right shift |  | -->

## Standard functions

### Math functions

Below is a list of the standard math functions avaliable.

| **Expression** | **Description** | **Example Input** | **Result |
|----|----|----|----|
| **Abs()** | Returns the absolute value of a specified number. | `Abs(-1)` | 1M |
| **Acos()** | Returns the angle whose cosine is the specified number. | `Acos(1)` | 0d |
| **Asin()** | Returns the angle whose sine is the specified number. | `Asin(0)` | 0d |
| **Atan()** | Returns the angle whose tangent is the specified number. | `Atan(0)` | 0d      |
| **Ceiling()** | Returns the smallest integer greater than or equal to the specified number. | `Ceiling(1.5)` | 2 |
| **Cos()** | Returns the cosine of the specified angle. | `Cos(0)`| 1d |
| **Exp()** | Returns e raised to the specified power. | `Exp(0)` | 1d |
| **Floor()** | Returns the largest integer less than or equal to the specified number. | `Floor(1.5)` | 1 |
| **IEEERemainder()** | Returns the remainder resulting from the division of a specified number by another specified number. | `IEEERemainder(3, 2)`| -1 |
| **Log()** | Returns the logarithm of a specified number. | `Log(1, 10)`| 0 |
| **Log10()** | Returns the base 10 logarithm of a specified number.  | `Log10(1)` | 0 |
| **Max()** | Returns the larger of two specified numbers.  | `Max(1, 2)` | 2  |
| **Min()** | Returns the smaller of two numbers.  | `Min(1, 2)` | 1 |
| **Pow()** | Returns a specified number raised to the specified power.  | `Pow(3, 2)` | 9 |
| **Round()** | Rounds a value to the nearest integer (0) or specified number of decimal places (>0). | `Round(3.222, 2)` | 3.22   |
| **Sign()**  | Returns a value indicating the sign of a number.  | `Sign(-10)` | -1 |
| **Sin()** | Returns the sine of the specified angle. | `Sin(0)` | 0 |
| **Sqrt()** | Returns the square root of a specified number. | `Sqrt(4)` | 2 |
| **Tan()** | Returns the tangent of the specified angle. | `Tan(0)` | 0 |
| **Truncate()** | Calculates the integral part of a number. | `Truncate(1.7)` | 1 |

### Logical functions

Logical functions and operators can be used for providing outputs based on a given condition.

| **Expression** | **Description** | **Example Input** | **Example Ouput** |
| --- | -- | ---| --- |
| **in(check,p1,p2,p3,pn)** | Returns whether an element (check) is in a set of values [p1,p2,p3,pn]. |  `in(1 + 1, 1, 2, 3)` | True
| **if(condition, value if true, value if false)** | If function, returns a value based on a true/false condition. |  `if(3 % 2 = 1, 45, 55)` | 45
| **?** | Simplified condition syntax. condtionat ? value if true : value if false. |  `3 % 2 =1 ? 45 : 55` | 45

### String functions

Below are functions which can be used to create strings. These functions are commonly useful for creating more complex input types such as [bolt spacing inputs, points and vectors](/api_parameters_getting_started.md#model-properties).  

| **Expression** | **Description** | **Example Input** | **Example Ouput** |
| --- | -- | ---| --- |
| **ToString(p1)** | Converts an input to its string equivilant. |  `ToString(2.1578)` | '2.1578'
| **Concat(p1, p2, pn)** | Concatenates any number strings into one string output, also evaluating each seperated input to a string. |  `Concat('M',20,' ','8.8')` | 'M20 8.8'
| **Join(Deliminater, p1, p2, Pn)** | Concatenates multiple strings into one output, with a given deliminater |  `Join(';',-0.020,0.020,0.040)` | '-0.020; 0.020; 0.040'
| **Format(input string, P0, P1, Pn)** | Allows input of specific values given a constant string format |  `Format('{0}*{1}',0.75,0.2)` | '0.75*2'


### Unit import and conversion functions

Remember that in the background all unitized values are stored in Basic SI units. We provide a list of functions where the user can input a particular decimal unitized value in there selected unit. This will then be converted to the basic SI equivilant in the background.

| **Expression** | **Description** | **Example Input** | **Example Ouput** |
| --- | -- | ---| --- |
| **Length(length, lengthunit)** | Allows input on a length to the specified unit type. ('m','dm','cm','mm','in','ft') |  `Length(4/5,'in')` | 0.020 m
| **Area(area, lengthunit)** | Allows input on a area to the specified unit type. ('m','dm','cm','mm','in','ft') |  `Area(200000,'mm')` | 0.2 m2
| **Force(force, forceunit)** | Allows a force input to a specified force unit type. ('N','kN','MN','daN','kp','lbf','kip') |  `Force(13.5,'kp')` | 132.39 N
| **Stress(stress, stressunit)** | Allows a stress input to a specified stress unit type. ('Pa','kPa','MPa','N/mm2','N/m2','N/m2','MN/m2','psi','ksi') |  `Stress(25,'MPa')` | 25000000 Pa
| **Moment(force, momentunit)** | Allows a moment input to a specified moment unit. ('Nm','kNm', 'MNm', 'daNm', 'lbf.ft','kip.ft','kip.in') |  `Moment(34,'lbf.ft')` | 46.097 Nm
| **Temp(temperature, temperatureunit)** | Allows a temperature input to a specified temperature unit. ('K','°C','°F') |  `Temp(10,'°F')` | 260.93 K
| **Angle(angle, angleunit)** | Allows an angle input from angle unit. ('rad','mrad','°','grad')|  `Angle(15,'°')` | 0.262 rad
| **Time(time, timeunit)** | Allows time input based on time unit. ('s','min.','h','d') |  `Time(5,'d')` | 432000 s

## Project and model property functions

Project and model property functions allow for the retreival of values from the given model or project. These include retriving information about the connection model (members and loading) aswell as other user settings which can be used to drive further calculations.


### Load effect functions

We provide a function which allows the user to retrieve the evelope of forces on a given member for all **Active** load effects in the project. This function can be very useful for making design decisions based on the enveloped load on a given member of the connection.

| **Expression** | **Description** | **Example Input** | **Example Ouput** |
| --- | -- | ---| --- |
| **GetLoadEffectEnvelope(Member, Action, Envelope, End)** | Allows retrieval of given member force envelope for a given direction ('N', 'Vy', 'Vz', 'Mx', 'My', 'Mz') with an envelope option of ('Max', 'Min', 'Abs Max', 'Abs Min') at either ('Begin', 'End') of the member. If no parameter for End is provided 'End' will be used by default |  `GetLoadEffectEnvelope('B','Vz','Abs Max','End')` | 17500 N
| | Example where the default End is used. |  `GetLoadEffectEnvelope('B','Vz','Abs Max')` | 17500 N
| | Example of getting minimum moment My. |  `GetLoadEffectEnvelope('B','My','Min')` | 120000 Nm

> [!NOTE]
> When load in percentage is set to true the resulting force value is still retrieved. Force values are always updated when the percentage is changed.
> [!WARNING]
> Be careful when transferring templates with the Min and Max envelope setting. If the local co-ordinate of a member changes, the positive and negative values my represent a different load direction on the connected member which should be carefully considered.

### Operation reference Functions

| **Expression** | **Description** | **Example Input** | **Example Ouput** |
| --- | -- | ---| --- |
| **GetBoltDiameter('operation_name', index)** | Allows retrieval of an operations bolt type diameter. For operations which have more than one bolt size, the index parameter (0=first, 1=second, etc) can be used. By default it is set at 0. |  `GetBoltDiameter('FP1',0)` | 0.020 m
| | Retrieves the first bolt type diameter of a given operation.  |  `GetBoltDiameter('FP1')` | 0.020 m
| | Retrieves the  second defined bolt type diameter of a given operation.  |  `GetBoltDiameter('FP1',1)` | 0.024 m

### Project setting value functions

We provide a general **GetSettingValue(setting)** function that allows a selected output value of a project setting to be used within parameters. Below is the currently avaliable general setting values that can be retrieved.

| **Description** | **Example Input** | **Example Ouput** |
| --- | ---| --- |
| **Rounding settings** |   | 
| Setting for rounding of plate input dimensions |  `GetSettingValue('Units.EntityRounding.PlateDimensions')` | 0.005 m
| Setting for rounding of plate thickness input |  `GetSettingValue('Units.EntityRounding.PlateThickness')` | 0.002 m
| Setting for rounding of bolt spacing input |  `GetSettingValue('Units.EntityRounding.BoltSpacing')` | 0.005 m
| Setting for rounding of weld size input |  `GetSettingValue('Units.EntityRounding.WeldSize')` | 0.001 m
| **Bolt detailing settings** |   | 
| Setting for end distance bolt diameter multiple |  `GetSettingValue('Design.Bolts.Detailing.End')` | 2.0
| Setting for edge distance bolt diameter multiple |  `GetSettingValue('Design.Bolts.Detailing.Edge')` | 2.0
| Setting for spacing bolt diameter multiple |  `GetSettingValue('Design.Bolts.Detailing.Spacing')` | 3.0
| Setting for wall distance bolt diameter multiple |  `GetSettingValue('Design.Bolts.Detailing.Wall')` | 2.3
| **Component design settings** |   | 
| Target utilization of bolt design |  `GetSettingValue('Design.AutoDesign.Bolts.TargetUtilization')` | 0.9
| Target utilization of bolt design |  `GetSettingValue('Design.AutoDesign.Welds.TargetUtilization')` | 0.9
| Overstrength factor for weld design |  `GetSettingValue('Design.AutoDesign.Welds.OverstrengthFactor')` | 1.4
| **General design settings** |   | 
| Beam resistance percentage |  `GetSettingValue('Design.General.ResistancePercentage')` | 0.4

### Member property value functions

We provide a general **GetValue(member, property)** function that can be used to extract model properties from the connection model. This enables the extraction of member information such as cross-section information, member geometry, etc. Here the `member` is the associated member in the model and `property` is the path of the desired property which will be retrieved.

> In all examples below 'B' represents the Name of the referenced member in the connection.

**Description** | **Example Input** | **Example Output**
----|----|----
**Member Properties** | General relating properties of the member | 
Material Name | `GetValue('B','MaterialName')` | 
Is the bearing member | `GetValue('B','IsBearingMember')` | 
Length | `GetValue('B', 'Length')` | 
Support Code | `GetValue('B', 'SupportCode')` | 
**Member Cross-Section Bounds** | Get the bounds of the given members cross-section | 
Height | `GetValue('B', 'CrossSection.Bounds.Height')` | 
Width | `GetValue('B', 'CrossSection.Bounds.Width')` | 
Bottom | `GetValue('B', 'CrossSection.Bounds.Bottom')` | 	 
Left | `GetValue('B', 'CrossSection.Bounds.Left')` | 	  		
Right | `GetValue('B', 'CrossSection.Bounds.Right')` | 	  		 		
Top | `GetValue('B', 'CrossSection.Bounds.Top')` |  		
**Member Geometry** | Properties of the given member | 
Angle Alpha | `GetValue('B', 'AngleAlpha')` | 
Angle Beta | `GetValue('B','AngleBeta')` | 
Rotation Rx | `GetValue('B','RotationRx')` | 
DirVect /X | `GetValue('B', 'DirVectX')` | 
DirVect /Y | `GetValue('B', 'DirVectY')` | 
DirVect /Z | `GetValue('B', 'DirVectZ')` | 
LCS Y Axis Vect /X | `GetValue('B', 'LcsyVectX')` | 
LCS Y Axis Vect /Y | `GetValue('B', 'LcsyVectY')` | 
LCS Y Axis Vect /Z | `GetValue('B', 'LcsyVectZ')` | 
LCS ZAxis Vect /X | `GetValue('B', 'LcszVectX')` | 
LCS Z Axis Vect /Y | `GetValue('B', 'LcszVectY')` | 
LCS Z Axis Vect /Z | `GetValue('B', 'LcszVectZ')` | 
Begin Offset X | `GetValue('B', 'EccentricityBeginX')` | 
Begin Offset Y | `GetValue('B', 'EccentricityBeginY')` | 
Begin Offset Z | `GetValue('B', 'EccentricityBeginZ')` | 
End Offset X | `GetValue('B', 'EccentricityBeginX')` | 
End Offset Y | `GetValue('B', 'EccentricityBeginY')` | 
End Offset Z | `GetValue('B', 'EccentricityBeginZ')` | 
InsertPointOnRefLine | `GetValue('B', 'InsertPointOnRefLine.X')` | 
Theoretical Length Y | `GetValue('B', 'TheoreticalLengthY')` | 
Theoretical Length Z | `GetValue('B', 'TheoreticalLengthZ')` | 
**Member CP position** | Gets the point of connection of this segment to the joint | 
Connection Point - X | `GetValue('B', 'ConnectionPoint.X')` | 
Connection Point - Y | `GetValue('B', 'ConnectionPoint.Y')` | 
Connection Point - Z | `GetValue('B', 'ConnectionPoint.Z')` | 
Position On Ref Line - Gets the relative position of the ConnectionPoint on the ReferenceLine | `GetValue('B', 'PositionOnRefLine')` | 
Calculated Pin Position - Gets the position of the calculated pinned connection. The position is measured on the local X-axis of the beam. The calculation is done according to the sizes and positions of bolts on this member | `GetValue('B', 'CalculatedPinPosition')` | 
**Member Bounding Box Geometry** | Gets bounding box in the local coordinate system of the beam | 
Bounding Box in LCS - Height  | `GetValue('B', 'BoundingBoxInLcs.Height')` | 
Bounding Box in LCS - Width  | `GetValue('B', 'BoundingBoxInLcs.Width)` | 
Bounding Box in LCS - Length  | `GetValue('B', 'BoundingBoxInLcs.Length)` | 

### Member position functions

GetQuadrant{*Axis1*}{*Axis*}(*Member1*, *Member2*)

The GetQuadrant function determines the quadrant position of a target structural member (Member2) relative to a source structural member (Member1) in a specified 2D plane within a 3D coordinate system.

#### Parameters
* Axis1 (char) – The first coordinate axis (either X, Y, or Z).
* Axis2 (char) – The second coordinate axis (either X, Y, or Z).
* Member1 (string) – The name of the reference structural member.
* Member2 (string) – The name of the structural member whose position is evaluated relative to Member1.

**Description** | **Example Input** | **Example Output**
----|----|----
Quadrant position of members in axis X & Z | `GetQuadrantXZ('M1', 'M2')` | 4
Quadrant position of members in axis X & Y | `GetQuadrantXY('M1', 'M2')` | 2

### Member relating cross-section property functions

Most cross-section properties of a related member can be retrieved with the general **GetValue(member, property)** function. For retrieving plate thickness another function is required, **GetBeamPlateThickness(member, plateitem)**.

**Description** | **Example Input** | **Example Output**
----|----|----
Top Flange Thickness | `GetBeamPlateThickness('B', 'TopFlange')` | 0.010 m
Bottom Flange Thickness | `GetBeamPlateThickness('B', 'BottomFlange')` | 0.012 m
Web Thickness |  `GetBeamPlateThickness('B', 'Web')` | 0.005 m

**Description** | **Example Input** | **Example Output**
----|----|----
Type of cross-section | `GetValue('B','CrossSection.CrossSectionType')` | RolledI
**A** - Area of Cross Section | `GetValue('B', 'CrossSection.CssCharact.A')` | 
**Av1** - Shear area Av1 (major principal axis) |  `GetValue('B','CrossSection.CssCharact.Av1')` | 
**Av2** Shear area Av2 (minor principal axis) | `GetValue('B','CrossSection.CssCharact.Av2')` | 
**Sx** - The first moment of area related to the X axis | `GetValue('B', 'CrossSection.CssCharact.Sx')` | 
**Sy** - The first moment of area related to the Y axis | `GetValue('B', 'CrossSection.CssCharact.Sy')` | 
**Ix** - The second moment of area related to the X axis | `GetValue('B', 'CrossSection.CssCharact.Ix')` | 
**Iy** - The second moment of area related to the Y axis | `GetValue('B', 'CrossSection.CssCharact.Iy')` |  		 		
**I1** - The principal second moment of area related to the first principal axis | `GetValue('B', 'CrossSection.CssCharact.I1')` | 
**I2** - The principal second moment of area related to the second principal axis | `GetValue('B', 'CrossSection.CssCharact.I2')` | 
**Alpha**- Gets the angle, in radians, between system and principal axes | `GetValue('B', 'CrossSection.CssCharact.Alpha')` | 		
**It**- Sant Vennant torsional constant | `GetValue('B', 'CrossSection.CssCharact.It')` |    
**Ixy** - The product moment of area | `GetValue('B', 'CrossSection.CssCharact.Ixy')` | 
**Cgx** - Gets the centre of gravity related to the X axis | `GetValue('B', 'CrossSection.CssCharact.Cgx')` | 
**Cgy** - Gets the centre of gravity related to the Y axis | `GetValue('B', 'CrossSection.CssCharact.Cgy')` | 
**Wpl1** - Plastic modulus related to major principal axis | `GetValue('B', 'CrossSection.CssCharact.Wpl1')` | 
**Wpl2** - Plastic modulus related to minor principal axis | `GetValue('B', 'CrossSection.CssCharact.Wpl2')` | 
**Wel1** - Section modulus related to major principal axis | `GetValue('B', 'CrossSection.CssCharact.Wel1')` | 
**Wel2** - Section modulus related to minor principal axis| `GetValue('B', 'CrossSection.CssCharact.Wel2')` | 
**Iw** - Warping Constant | `GetValue('B', 'CrossSection.CssCharact.Iw')` | 
**C** - Torsion Constant| `GetValue('B', 'CrossSection.CssCharact.C')` | 
**Rgx** -Gets the radius of gyration related to the X axis | `GetValue('B', 'CrossSection.CssCharact.Rgx')` | 
**Rgy** - Gets the radius of gyration related to the Y axis | `GetValue('B', 'CrossSection.CssCharact.Rgy')` | 
**Rg1** -  Gets the radius of gyration related to the first principal axis | `GetValue('B', 'CrossSection.CssCharact.Rg1')` |
**Rg2** - Gets the radius of gyration related to the second principal axis | `GetValue('B', 'CrossSection.CssCharact.Rg2')` | 
**x0** -  Shear centre distance from centroidal point in X direction | `GetValue('B', 'CrossSection.CssCharact.x0')` |
**y0** - Shear centre distance from centroidal point in Y direction| `GetValue('B', 'CrossSection.CssCharact.y0')` | 
Painting surface as surface of 1m long part of beam with that cross-section | `GetValue('B', 'CrossSection.CssCharact.PaintingSurface')` | 

### Member relating section material property functions

Material properties of a related member can be retrieved with the general **GetValue(member, property)** function.

**Description** | **Example Input** | **Example Output**
----|----|----
**E** - Young's modulus | `GetValue('B', 'Material.E')` | 
**G** - Shear modulus | `GetValue('B', 'Material.G')` | 
Poisson's ratio | `GetValue('B', 'Material.Poissons')` | 
Unit Mass | `GetValue('B', 'Material.UnitMass')` | 
Specific Heat capacity | `GetValue('B', 'Material.SpecificHeat')` | 
Thermal Expansion | `GetValue('B', 'Material.ThermalExpansion')` |
Thermal Conductivity | `GetValue('B', 'Material.ThermalConductivity')` | 
Is Default Materail | `GetValue('B', 'Material.IsDefaultMaterial')` |

## Further information

Further information of using expressions can be found on the [NCalc](https://github.com/ncalc/ncalc/wiki) wiki page.

Looking for something more? Post a question on our [discussion forum](https://github.com/idea-statica/ideastatica-public/discussions).
