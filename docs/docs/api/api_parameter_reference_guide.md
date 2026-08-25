# Expression Parameter Reference

The below provides further information on using parameter expressions in the IDEA StatiCa developer tab. More general information on using parameters can be found [here](api_parameters_getting_started.md).

## Introduction

This page provides a comprehensive list of the operations and functions available within the developer tab parameter functionality.

On top of the standard functions that you would expect when using formulas or expressions, several special functions are provided to extend functionality specific to IDEA StatiCa applications. These include functions that can reference existing member geometric properties, project setting values, and forces in a given connection, as well as inputting and converting units.

All together they provide for a very powerful way of driving designs in a parametric fashion.

> [!NOTE]
> Write function names exactly as documented on this page. While some functions tolerate different casing, several (for example `CheckFormCode`, `RoundWithOffset` and the string functions) are case-sensitive. String arguments which represent enumeration values (for example `'Node'` in `CheckForcesIn` or `'I'` in `CheckFormCode`) are also case-sensitive.

## Operators

### Standard operators

| **Expression** | **Description** | **Example Input** | **Example Output** |
| --- | -- | ---| --- |
| **+** | addition | `2.3 + 2` | 4.3
| **-** | subtraction | `2.3 - 2` | 0.3
| **\*** | multiplication | `2.5*3.1` | 7.75
| **/** | division | `5/4` | 1.25
| **%** | remainder (modulo) division | `9 % 4` | 1

### Logical

Below is a list of the standard logical operators available.

| **Expression** | **Description** | **Example Input** | **Example Output** |
| --- | -- | ---| --- |
| **or, \|\|** | Condition testing based on or | `true || true`, `true or false` | true, true 
| **and, &&** | Condition testing based on and | `true && true`, `true and false` | true, false

### Relational

Below is a list of the standard relational operators available.

| **Expression** | **Description** | **Example Input** | **Example Output** |
| --- | -- | ---| --- |
| **n = x, n == x** | n is equal to x | `5 == 5` | True
| **n != x** | n does not equal x | `3 != 5` | True
| **n < x < j** | x is within bounds of n and j | `3 < 4 < 5` | True
| **n < x** | n is less than x | `3 < 4`| True
| **n <= x** | n is less than or equal to x | `3 <= 4`, `3 <= 3` | True
| **x > n** | x is greater than n | `4 > 3` | True
| **x >= n** | x is greater than or equal to n | `4 >= 3`, `3 >= 3` | True

### Unary

| **Expression** | **Description** | **Example Input** | **Example Output** |
| --- | -- | ---| --- |
| **!, not** | reverse the meaning or logic of a given condition or object | `!true`, `not true` | False
| **-** | numeric negation symbol | (given x=5) `-x` | -5
| **~** | Bitwise not |  | 

## Standard functions

### Math functions

Below is a list of the standard math functions available.

| **Expression** | **Description** | **Example Input** | **Result** |
|----|----|----|----|
| **Abs()** | Returns the absolute value of a specified number. | `Abs(-1)` | 1 |
| **Acos()** | Returns the angle (in radians) whose cosine is the specified number. | `Acos(1)` | 0 |
| **Asin()** | Returns the angle (in radians) whose sine is the specified number. | `Asin(0)` | 0 |
| **Atan()** | Returns the angle (in radians) whose tangent is the specified number. | `Atan(0)` | 0 |
| **Ceiling()** | Returns the smallest integer greater than or equal to the specified number. | `Ceiling(1.5)` | 2 |
| **Cos()** | Returns the cosine of the specified angle (in radians). | `Cos(0)`| 1 |
| **Exp()** | Returns e raised to the specified power. | `Exp(0)` | 1 |
| **Floor()** | Returns the largest integer less than or equal to the specified number. | `Floor(1.5)` | 1 |
| **IEEERemainder()** | Returns the remainder resulting from the division of a specified number by another specified number. | `IEEERemainder(3, 2)`| -1 |
| **Log()** | Returns the logarithm of a specified number in a specified base. `Log(value, base)` | `Log(100, 10)`| 2 |
| **Log10()** | Returns the base 10 logarithm of a specified number.  | `Log10(100)` | 2 |
| **Max()** | Returns the larger of two specified numbers.  | `Max(1, 2)` | 2  |
| **Min()** | Returns the smaller of two numbers.  | `Min(1, 2)` | 1 |
| **Pow()** | Returns a specified number raised to the specified power.  | `Pow(3, 2)` | 9 |
| **Round()** | Rounds a value to the nearest integer (0) or specified number of decimal places (>0). | `Round(3.222, 2)` | 3.22 |
| **Sign()**  | Returns a value indicating the sign of a number.  | `Sign(-10)` | -1 |
| **Sin()** | Returns the sine of the specified angle (in radians). | `Sin(0)` | 0 |
| **Sqrt()** | Returns the square root of a specified number. | `Sqrt(4)` | 2 |
| **Tan()** | Returns the tangent of the specified angle (in radians). | `Tan(0)` | 0 |
| **Truncate()** | Calculates the integral part of a number. | `Truncate(1.7)` | 1 |

### Rounding functions

In addition to the standard `Round()`, `Ceiling()`, `Floor()` and `Truncate()` functions, a special rounding function is provided for rounding to a given step size. This is particularly useful for rounding calculated dimensions (plate sizes, bolt spacings) to practical fabrication values.

| **Expression** | **Description** | **Example Input** | **Example Output** |
| --- | -- | ---| --- |
| **RoundWithOffset(value, step)** | Rounds a value to the nearest multiple of the given step. The step must be positive. The result is rounded to the same number of decimal places as the step. | `RoundWithOffset(0.0762, 0.005)` | 0.075
| | Rounds up when the value is closer to the upper multiple. | `RoundWithOffset(0.0782, 0.005)` | 0.08
| | | `RoundWithOffset(0.0682, 0.005)` | 0.07
| | Values which are already a multiple of the step are unchanged. | `RoundWithOffset(0.075, 0.005)` | 0.075

> [!TIP]
> `RoundWithOffset` combines well with the [project setting value functions](#project-setting-value-functions). For example, `RoundWithOffset([calculated_width], GetSettingValue('Units.EntityRounding.PlateDimensions'))` rounds a calculated plate width to the project's plate dimension rounding setting.

### Logical functions

Logical functions and operators can be used for providing outputs based on a given condition.

| **Expression** | **Description** | **Example Input** | **Example Output** |
| --- | -- | ---| --- |
| **in(check, p1, p2, p3, pn)** | Returns whether an element (check) is in a set of values [p1,p2,p3,pn]. |  `in(1 + 1, 1, 2, 3)` | True
| **if(condition, value if true, value if false)** | If function, returns a value based on a true/false condition. |  `if(3 % 2 = 1, 45, 55)` | 45
| **ifs(cond1, value1, cond2, value2, ..., default)** | Evaluates a number of condition/value pairs in order and returns the value of the first true condition, or the default if none are true. |  `ifs([t] > 0.02, 0.008, [t] > 0.01, 0.006, 0.004)` | 0.006 (for t = 0.012)
| **?** | Simplified condition syntax. condition ? value if true : value if false. |  `3 % 2 = 1 ? 45 : 55` | 45

### String functions

Below are functions which can be used to create strings. These functions are commonly useful for creating more complex input types such as [bolt spacing inputs, points and vectors](api_parameters_getting_started.md#model-properties).

| **Expression** | **Description** | **Example Input** | **Example Output** |
| --- | -- | ---| --- |
| **ToString(p1, p2, pn)** | Converts an input to its string equivalent. When multiple inputs are provided, each is converted and the results are concatenated. |  `ToString(2.1578)` | '2.1578'
| **Concat(p1, p2, pn)** | Concatenates any number of inputs into one string output, evaluating each separated input to a string. |  `Concat('M', 20, ' ', '8.8')` | 'M20 8.8'
| **Join(delimiter, p1, p2, pn)** | Concatenates multiple inputs (at least two) into one string, separated by the given delimiter. |  `Join(';', -0.02, 0.02, 0.04)` | '-0.02;0.02;0.04'
| **Format(format string, p0, p1, pn)** | Inserts the given values into a constant string format (uses .NET composite formatting). |  `Format('{0}*{1}', 0.75, 0.2)` | '0.75*0.2'

> [!NOTE]
> Numbers are always converted to strings using a decimal point (invariant culture), regardless of the regional settings of the PC.

### Unit input and conversion functions

Remember that in the background all unitized values are stored in basic SI units. We provide a list of functions where the user can input a particular decimal unitized value in their selected unit. This will then be converted to the basic SI equivalent in the background.

| **Expression** | **Description** | **Example Input** | **Example Output** |
| --- | -- | ---| --- |
| **Length(length, lengthunit)** | Allows input of a length in the specified unit. ('m', 'dm', 'cm', 'mm', 'in', 'ft') |  `Length(4/5, 'in')` | 0.02032 m
| **Area(area, areaunit)** | Allows input of an area in the specified unit. ('m2', 'dm2', 'cm2', 'mm2', 'in2', 'ft2') |  `Area(200000, 'mm2')` | 0.2 m2
| **Force(force, forceunit)** | Allows a force input in the specified force unit. ('N', 'kN', 'MN', 'daN', 'kp', 'lbf', 'kip') |  `Force(13.5, 'kp')` | 132.39 N
| **Stress(stress, stressunit)** | Allows a stress input in the specified stress unit. ('Pa', 'kPa', 'MPa', 'N/mm2', 'N/m2', 'MN/m2', 'psi', 'ksi') |  `Stress(25, 'MPa')` | 25000000 Pa
| **Moment(moment, momentunit)** | Allows a moment input in the specified moment unit. ('Nm', 'kNm', 'MNm', 'daNm', 'lbf.ft', 'kip.ft', 'kip.in') |  `Moment(34, 'lbf.ft')` | 46.098 Nm
| **Temp(temperature, temperatureunit)** | Allows a temperature input in the specified temperature unit. ('K', '°C', '°F'). The aliases 'degC' and 'degF' are also accepted. |  `Temp(10, '°F')` | 260.93 K
| **Angle(angle, angleunit)** | Allows an angle input in the specified angle unit. ('rad', 'mrad', '°', 'grad')|  `Angle(15, '°')` | 0.262 rad
| **Time(time, timeunit)** | Allows a time input in the specified time unit. ('s', 'min.', 'h', 'd'). Note the trailing dot in 'min.'. |  `Time(5, 'd')` | 432000 s

> [!NOTE]
> Unit signs are case-insensitive, e.g. `Length(2, 'Ft')` is also valid. For areas, the trailing '2' is optional — `Area(200000, 'mm')` is interpreted as mm2.

## Project and model property functions

Project and model property functions allow for the retrieval of values from the given model or project. These include retrieving information about the connection model (members and loading) as well as other user settings which can be used to drive further calculations.

### Load effect functions

We provide a function which allows the user to retrieve the envelope of forces on a given member for all **Active** load effects in the project. This function can be very useful for making design decisions based on the enveloped load on a given member of the connection.

| **Expression** | **Description** | **Example Input** | **Example Output** |
| --- | -- | ---| --- |
| **GetLoadEffectEnvelope(Member, Action, Envelope, Position)** | Allows retrieval of a given member force envelope for a given direction ('N', 'Vy', 'Vz', 'Mx', 'My', 'Mz') with an envelope option of ('Max', 'Min', 'Abs Max', 'Abs Min') at either ('Begin', 'End') of the member. If no Position parameter is provided, 'End' is used by default. |  `GetLoadEffectEnvelope('B', 'Vz', 'Abs Max', 'End')` | 17500 N
| | Example where the default position 'End' is used. |  `GetLoadEffectEnvelope('B', 'Vz', 'Abs Max')` | 17500 N
| | Example of getting minimum moment My. |  `GetLoadEffectEnvelope('B', 'My', 'Min')` | 120000 Nm
| | Example of getting the maximum moment at the begin position of a continuous member. |  `GetLoadEffectEnvelope('C', 'My', 'Max', 'Begin')` | 35500 Nm

The Action, Envelope and Position arguments are case-insensitive (e.g. `'vz'`, `'min'` are also accepted). The function throws an error when the member name is not found or when an invalid Action/Envelope/Position string is provided.

> [!NOTE]
> When load in percentage is set to true the resulting force value is still retrieved. Force values are always updated when the percentage is changed.

> [!WARNING]
> Be careful when transferring templates with the Min and Max envelope setting. If the local coordinate system of a member changes, the positive and negative values may represent a different load direction on the connected member, which should be carefully considered.

### Load position check functions

The **CheckForcesIn(position, [member1, member2, ...])** function checks the 'Forces in' setting of members. It returns `true` only when **all** of the checked members have their forces applied in the given position.

**Possible position values** (case-sensitive):

* `'Node'` — forces applied at the connection point (also accepts members set to 'Position' with a position value of 0, which is equivalent)
* `'Position'` — forces applied at a user-defined position
* `'Bolts'` — forces applied at the center of bolts
* `'SelectedMemberFace'` — forces applied at the closest face of a user-selected member

The member list is optional. When omitted, the function checks the members mapped in the applied parametric template; if no template has been applied, it checks all members of the connection.

**Description** | **Example Input** | **Example Output**
----|----|----
Check a single member | `CheckForcesIn('Node', 'C')` | `true`
Check all (template) members | `CheckForcesIn('Position')` | `false`
Check multiple specific members | `CheckForcesIn('SelectedMemberFace', 'M3', 'M4')` | `true`

> [!TIP]
> This function is useful for guarding template logic that is only valid for a certain position of internal forces, e.g. `if(CheckForcesIn('Node'), [valueA], [valueB])`.

### Operation reference functions

| **Expression** | **Description** | **Example Input** | **Example Output** |
| --- | -- | ---| --- |
| **GetBoltDiameter('operation_name', index)** | Allows retrieval of an operation's bolt type diameter. For operations which have more than one bolt input (e.g. cleats), the index parameter (0 = first, 1 = second) can be used. By default it is set at 0. The operation name is case-insensitive. |  `GetBoltDiameter('FP1', 0)` | 0.020 m
| | Retrieves the first bolt type diameter of a given operation.  |  `GetBoltDiameter('FP1')` | 0.020 m
| | Retrieves the second defined bolt type diameter of a given operation.  |  `GetBoltDiameter('CLEAT1', 1)` | 0.018 m
| | Without arguments, retrieves the bolt diameter of the last operation containing bolts (kept for backward compatibility — prefer naming the operation explicitly). |  `GetBoltDiameter()` | 0.016 m

Only indexes 0 and 1 are supported. The function throws an error when the operation is not found, the operation has no bolts, or the index is out of range.

### Project setting value functions

We provide a general **GetSettingValue(setting)** function that allows a selected output value of a project setting to be used within parameters. The setting path is case-insensitive. Design-related settings are read for the design code of the current project. Below are the currently available setting values that can be retrieved.

| **Description** | **Example Input** | **Example Output** |
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
| Target utilization of weld design |  `GetSettingValue('Design.AutoDesign.Welds.TargetUtilization')` | 0.9
| Overstrength factor for weld design |  `GetSettingValue('Design.AutoDesign.Welds.OverstrengthFactor')` | 1.4
| **General design settings** |   | 
| Beam resistance percentage |  `GetSettingValue('Design.General.ResistancePercentage')` | 0.4
| Weld size specification method |  `GetSettingValue('Design.General.WeldSpecification')` | 

The function throws an error when the path is not recognized or is not fully specified (e.g. `GetSettingValue('Design.AutoDesign.Bolts')` fails because it does not point to a single value).

### Member property value functions

We provide a general **GetValue(member, property, default)** function that can be used to extract model properties from the connection model. This enables the extraction of member information such as cross-section information, member geometry, etc. Here `member` is the name of the associated member in the model and `property` is the path of the desired property which will be retrieved. The property path is case-insensitive.

The third parameter `default` is optional. When the property is not found:

* with a default provided, the default value is returned, e.g. `GetValue('B', 'Css.R2', 50)` returns `50`;
* without a default, the string `Property '<property>' not found` is returned.

Properties whose value is an enumeration are returned as strings (e.g. `GetValue('B', 'ForcesIn')` returns `'Node'`).

> In all examples below 'B' represents the Name of the referenced member in the connection.

**Description** | **Example Input** | **Example Output**
----|----|----
**Member Properties** | General relating properties of the member | 
Material Name | `GetValue('B', 'MaterialName')` | 
Is the bearing member | `GetValue('B', 'IsBearingMember')` | 
Length | `GetValue('B', 'Length')` | 
Support Code | `GetValue('B', 'SupportCode')` | 
Forces | `GetValue('B', 'ForcesIn')` | (Position, Node, Bolts, SelectedMemberFace) |
Static behavior | `GetValue('B', 'StaticBehavior')` | (AllDirActive = N-Vy-Vz-Mx-My-Mz,  DoNotActDirYRotZ = N-Vz-My, DoNotActDirZRotY = N-Vy-Mz, NoBending = N-Vy-Vz) |
**Member Cross-Section Bounds** | Get the bounds of the given member's cross-section | 
Height | `GetValue('B', 'CrossSection.Bounds.Height')` | 
Width | `GetValue('B', 'CrossSection.Bounds.Width')` | 
Bottom | `GetValue('B', 'CrossSection.Bounds.Bottom')` | 	 
Left | `GetValue('B', 'CrossSection.Bounds.Left')` | 	  		
Right | `GetValue('B', 'CrossSection.Bounds.Right')` | 	  		 		
Top | `GetValue('B', 'CrossSection.Bounds.Top')` |  		
**Member Geometry** | Properties of the given member | 
Angle Alpha | `GetValue('B', 'AngleAlpha')` | 
Angle Beta | `GetValue('B', 'AngleBeta')` | 
Rotation Rx | `GetValue('B', 'RotationRx')` | 
DirVect /X | `GetValue('B', 'DirVectX')` | 
DirVect /Y | `GetValue('B', 'DirVectY')` | 
DirVect /Z | `GetValue('B', 'DirVectZ')` | 
LCS Y Axis Vect /X | `GetValue('B', 'LcsyVectX')` | 
LCS Y Axis Vect /Y | `GetValue('B', 'LcsyVectY')` | 
LCS Y Axis Vect /Z | `GetValue('B', 'LcsyVectZ')` | 
LCS Z Axis Vect /X | `GetValue('B', 'LcszVectX')` | 
LCS Z Axis Vect /Y | `GetValue('B', 'LcszVectY')` | 
LCS Z Axis Vect /Z | `GetValue('B', 'LcszVectZ')` | 
Begin Offset X | `GetValue('B', 'EccentricityBeginX')` | 
Begin Offset Y | `GetValue('B', 'EccentricityBeginY')` | 
Begin Offset Z | `GetValue('B', 'EccentricityBeginZ')` | 
End Offset X | `GetValue('B', 'EccentricityEndX')` | 
End Offset Y | `GetValue('B', 'EccentricityEndY')` | 
End Offset Z | `GetValue('B', 'EccentricityEndZ')` | 
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
Bounding Box in LCS - Width  | `GetValue('B', 'BoundingBoxInLcs.Width')` | 
Bounding Box in LCS - Length  | `GetValue('B', 'BoundingBoxInLcs.Length')` | 

> [!NOTE]
> `GetValue` is not limited to members. The object name is also searched among plates, operations and member properties of the connection, so properties of a named operation can be retrieved in the same way, e.g. `GetValue('FP1', 'Thickness')`.

### Member position functions

#### GetQuadrant{*Axis1*}{*Axis2*}(*Member1*, *Member2*)

The GetQuadrant function determines the quadrant position of a target structural member (Member2) relative to a source structural member (Member1) in a specified 2D plane of the source member's local coordinate system. The function returns the quadrant number 1–4.

#### Parameters
* Axis1 (char) – The first coordinate axis (either X, Y, or Z; must be uppercase).
* Axis2 (char) – The second coordinate axis (either X, Y, or Z; must be uppercase).
* Member1 (string) – The name of the reference structural member.
* Member2 (string) – The name of the structural member whose position is evaluated relative to Member1.

**Description** | **Example Input** | **Example Output**
----|----|----
Quadrant position of members in plane X & Z | `GetQuadrantXZ('C', 'B')` | 4
Quadrant position of members in plane X & Y | `GetQuadrantXY('C', 'B')` | 1
Reversing the member order changes the reference system | `GetQuadrantXY('B', 'C')` | 2

Boundary cases (the target member lying exactly on an axis) are assigned as follows: positive first axis → quadrant 1, negative first axis → quadrant 2, positive second axis → quadrant 1, negative second axis → quadrant 4.

#### GetMembersAngle(*Member1*, *Member2*)

Calculates the angle between the given members in radians. The result is in the range 0 to π (0° to 180°).

**Description** | **Example Input** | **Example Output**
----|----|----
Angle between perpendicular members B and C | `GetMembersAngle('C', 'B')` | 1.5708 (= 90°)
Angle between opposite members | `GetMembersAngle('M3', 'M5')` | 3.1416 (= 180°)

If you want to see degree values, you can assign the `Angle` value type to the parameter.

### Member cross-section property functions

Most cross-section properties of a related member can be retrieved with the general **GetValue(member, property)** function. For retrieving plate thickness another function is required, **GetBeamPlateThickness(member, plateitem)**.

**Description** | **Example Input** | **Example Output**
----|----|----
Top Flange Thickness | `GetBeamPlateThickness('B', 'TopFlange')` | 0.010 m
Bottom Flange Thickness | `GetBeamPlateThickness('B', 'BottomFlange')` | 0.012 m
Web Thickness |  `GetBeamPlateThickness('B', 'Web')` | 0.005 m

> [!NOTE]
> If the given plate item name is not found, the thickness of the first plate of the member is returned. The plate item name is case-insensitive.

#### Form code functions

There are two functions for working with the form code (profile type) of members in the project:

* **GetCrossSectionFormCode(*Member*)** returns the form code of a member's cross-section as a string.
* **CheckFormCode(*FormCode*, *[Member1, Member2, ...]*)** returns `true` when **all** of the checked members have the given form code. The member list is optional — when omitted, the function checks the members mapped in the applied parametric template, or all members of the connection if no template has been applied.

**Possible Form Codes** (case-sensitive):
<table><tr><td>

- `I`
- `U`
- `L`

</td><td>

- `Z`
- `Rectangle`
- `Circle`

</td><td>

- `Tee`
- `Flat`
- `Rod`

</td><td>

- `Omega`
- `Compound`
- `General`

</td></tr></table>

**Description** | **Example Input** | **Example Output**
----|----|----
Get the form code of a member | `GetCrossSectionFormCode('B')` | `'I'`
Check all members in project for given form code | `CheckFormCode('U')` | `false`
Check only specific members | `CheckFormCode('I', 'B', 'C')` | `true`
Works with logical connectors |  `CheckFormCode('U', 'C') && CheckFormCode('I', 'M')`| `true`
Combine with conditions | `if(GetCrossSectionFormCode('B') == 'I', [valueA], [valueB])` | 

#### Cross-section characteristics

**Description** | **Example Input** | **Example Output**
----|----|----
Type of cross-section | `GetValue('B', 'CrossSection.CrossSectionType')` | RolledI
**A** - Area of Cross Section | `GetValue('B', 'CrossSection.CssCharact.A')` | 
**Av1** - Shear area Av1 (major principal axis) |  `GetValue('B', 'CrossSection.CssCharact.Av1')` | 
**Av2** - Shear area Av2 (minor principal axis) | `GetValue('B', 'CrossSection.CssCharact.Av2')` | 
**Sx** - The first moment of area related to the X axis | `GetValue('B', 'CrossSection.CssCharact.Sx')` | 
**Sy** - The first moment of area related to the Y axis | `GetValue('B', 'CrossSection.CssCharact.Sy')` | 
**Ix** - The second moment of area related to the X axis | `GetValue('B', 'CrossSection.CssCharact.Ix')` | 
**Iy** - The second moment of area related to the Y axis | `GetValue('B', 'CrossSection.CssCharact.Iy')` |  		 		
**I1** - The principal second moment of area related to the first principal axis | `GetValue('B', 'CrossSection.CssCharact.I1')` | 
**I2** - The principal second moment of area related to the second principal axis | `GetValue('B', 'CrossSection.CssCharact.I2')` | 
**Alpha** - Gets the angle, in radians, between system and principal axes | `GetValue('B', 'CrossSection.CssCharact.Alpha')` | 		
**It** - Saint-Venant torsional constant | `GetValue('B', 'CrossSection.CssCharact.It')` |    
**Ixy** - The product moment of area | `GetValue('B', 'CrossSection.CssCharact.Ixy')` | 
**Cgx** - Gets the centre of gravity related to the X axis | `GetValue('B', 'CrossSection.CssCharact.Cgx')` | 
**Cgy** - Gets the centre of gravity related to the Y axis | `GetValue('B', 'CrossSection.CssCharact.Cgy')` | 
**Wpl1** - Plastic modulus related to major principal axis | `GetValue('B', 'CrossSection.CssCharact.Wpl1')` | 
**Wpl2** - Plastic modulus related to minor principal axis | `GetValue('B', 'CrossSection.CssCharact.Wpl2')` | 
**Wel1** - Section modulus related to major principal axis | `GetValue('B', 'CrossSection.CssCharact.Wel1')` | 
**Wel2** - Section modulus related to minor principal axis| `GetValue('B', 'CrossSection.CssCharact.Wel2')` | 
**Iw** - Warping Constant | `GetValue('B', 'CrossSection.CssCharact.Iw')` | 
**C** - Torsion Constant| `GetValue('B', 'CrossSection.CssCharact.C')` | 
**Rgx** - Gets the radius of gyration related to the X axis | `GetValue('B', 'CrossSection.CssCharact.Rgx')` | 
**Rgy** - Gets the radius of gyration related to the Y axis | `GetValue('B', 'CrossSection.CssCharact.Rgy')` | 
**Rg1** -  Gets the radius of gyration related to the first principal axis | `GetValue('B', 'CrossSection.CssCharact.Rg1')` |
**Rg2** - Gets the radius of gyration related to the second principal axis | `GetValue('B', 'CrossSection.CssCharact.Rg2')` | 
**x0** -  Shear centre distance from centroidal point in X direction | `GetValue('B', 'CrossSection.CssCharact.x0')` |
**y0** - Shear centre distance from centroidal point in Y direction| `GetValue('B', 'CrossSection.CssCharact.y0')` | 
Painting surface as surface of 1m long part of beam with that cross-section | `GetValue('B', 'CrossSection.CssCharact.PaintingSurface')` | 

#### Cross-section geometry components

Geometric properties of the cross-section shape components (e.g. fillet radii of rolled sections) can be accessed via the `Css.` prefix. The property is searched in the geometry of the cross-section components of the member. The optional default value of `GetValue` is useful here because the available properties differ per section shape.

**Description** | **Example Input** | **Example Output**
----|----|----
Fillet radius of a rolled section | `GetValue('B', 'Css.R1')` | 
Fillet radius with a fallback when the shape has no R2 | `GetValue('B', 'Css.R2', 0)` | 

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
Is Default Material | `GetValue('B', 'Material.IsDefaultMaterial')` |

## Advanced functions

The functions in this section produce complex model property values. They are primarily intended for use in parametric templates and should be assigned directly to the matching model property type.

### ValueCount function

The **ValueCount(value, count)** function creates a value/count pair representing a repeated distance, equivalent to entering `0.06*3` in a spacing input in the UI. The count must be a positive number.

| **Expression** | **Description** | **Example Input** |
| --- | -- | ---|
| **ValueCount(value, count)** | Creates a repeated distance entry, e.g. three spacings of 60 mm. |  `ValueCount(0.06, 3)`

`ValueCount` entries can be passed as row/column inputs into the bolt and anchor layout functions below.

### Bolt and anchor layout functions

These functions build a complete bolt or anchor layout value which can be assigned to the bolt/anchor grid model property of an operation (e.g. a base plate). Position values are entered in basic SI units (m) and lists are separated by the `'#'` marker string.

#### Bolts(*BoltAssembly*, *PositionType*, rows..., '#', cols..., ['#', negativeRows..., '#', negativeCols...])

Creates a rectangular bolt layout.

* **BoltAssembly** (string) – the name of the bolt assembly, e.g. `'M16 8.8'`.
* **PositionType** (string) – how positions are measured. One of:
  * `'Rectangle'` – distances related to the grid axis, asymmetrical
  * `'RectangleSymmetrical'` – distances related to the grid axis, symmetrical
  * `'ToProfile'` – distances related to the profile, asymmetrical
  * `'ToProfileSymmetrical'` – distances related to the profile, symmetrical
  * `'TopOfSteel'` – row distances related to the top of steel, column distances related to the axis
* **rows / cols** (numbers or `ValueCount` entries) – the row positions, then a `'#'` separator, then the column positions. For asymmetrical position types, two further optional `'#'`-separated groups define negative-direction rows and columns.

The row and column entries correspond one-to-one to what would be typed in the rows/columns fields of the bolt grid in the UI.

**Example Input** | **Description**
----|----
`Bolts('M16 8.8', 'RectangleSymmetrical', 0.05, '#', 0.04, 0.08)` | Symmetric layout with one row entry (50 mm) and two column entries (40 and 80 mm)
`Bolts('M16 8.8', 'Rectangle', ValueCount(0.05, 2), '#', 0.04)` | Layout using a repeated 50 mm row entry (equivalent to typing `50*2`)

#### AnchorOrthogonal(*Anchor*, *IsSymmetrical*, rows..., '#', cols...)

Creates a rectangular (Cartesian) anchor layout related to the grid axis.

* **Anchor** (string) – the name of the anchor/bolt assembly, e.g. `'M16 8.8'`.
* **IsSymmetrical** (bool) – `true` for a symmetrical layout, `false` for asymmetrical.
* **rows / cols** (numbers or `ValueCount` entries) – the row positions, then a `'#'` separator, then the column positions.

**Example Input** | **Description**
----|----
`AnchorOrthogonal('M16 8.8', true, 0.05, 0.1, '#', 0.05, 0.1)` | Symmetric anchor layout with row entries 50 and 100 mm and column entries 50 and 100 mm

#### AnchorPolar(*Anchor*, *Length*, *Count*, *AnchorType*, radii..., '#', angles...)

Creates a polar (circular) anchor layout.

* **Anchor** (string) – the name of the anchor/bolt assembly.
* **Length** (number) – the anchor length in m.
* **Count** (int) – the number of anchors.
* **AnchorType** (string) – one of `'Straight'`, `'WasherPlateCircular'`, `'WasherPlateRectangular'`.
* **radii / angles** (numbers) – the radii of the anchor circles, then a `'#'` separator, then the angles.

**Example Input** | **Description**
----|----
`AnchorPolar('M20 8.8', 0.35, 6, 'Straight', 0.2, '#', 0)` | Six straight anchors of length 350 mm on a circle of radius 200 mm

### Parametric cross-section functions

The following functions redefine the cross-section they are assigned to from basic dimensional parameters. They only take effect when the expression is assigned to a **cross-section** property item — evaluating them in a standalone parameter has no effect. All dimensions are entered in basic SI units (m).

| **Expression** | **Description** |
| --- | -- |
| **IProfile(width, height, flangeThickness, webThickness)** | Creates a welded I-section. |
| **UProfile(width, height, flangeThickness, webThickness, webRadius, flangeRadius)** | Creates a channel (U) section. |
| **LProfile(width, height, legThickness, legThickness2, webRadius, flangeRadius)** | Creates an angle (L) section. |
| **PProfile(width, height, thickness, radius)** | Creates a rectangular hollow section (RHS). |
| **OProfile(diameter, thickness)** | Creates a circular hollow section. |
| **PDProfile(diameter, thickness)** | Creates a circular hollow (pipe) section. |
| **CCProfile(width, height, thickness, lip)** | Creates a cold-formed C section. |
| **PLProfile(width, height)** | Creates a rectangular plate section. |

**Example Input** | **Description**
----|----
`IProfile(0.2, 0.4, 0.012, 0.008)` | Welded I-section 200 mm wide, 400 mm deep, with 12 mm flanges and an 8 mm web
`PLProfile([width], [thickness])` | Plate section driven by previously defined parameters

## Further information

Further information on using expressions can be found on the [NCalc](https://github.com/ncalc/ncalc/wiki) wiki page.

Looking for something more? Post a question on our [discussion forum](https://github.com/idea-statica/ideastatica-public/discussions).
