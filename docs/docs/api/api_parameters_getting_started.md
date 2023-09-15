Below provides an introduction to the use of parameters in IDEA StatiCa Connection. 

Parameters can be accessed by [enabling the Developer Mode] in the connection application.

## Introduction

IDEA StatiCa Connection v23 provides an improved experience for creating and setting parameters in an IDEA StatiCa Connection Project.

Parameters can be used to facilitate many functions including:
* Parametric templates.
* Connection sensitivity studies.
* Connection part optimisation.
* Machine learning algorithms and studies.

Parameters are specific to one item in a connection project. This allows parameters to be exported inside of connection templates which are saved from a connection project item. 

> It is recommended, although not required, that when using parameters, only one connection item is present in the connection project.

### Accessing parameters

Parameters can be accessed through the [Developer Tab](https://github.com/idea-statica/ideastatica-public/wiki/Developer-Tools-in-IDEA-StatiCa-Connection).

### Parameters vs API

Parameters can be seen as a middle ground between API functionally and the classic UI of the program. 

All functionality (input/outputs) of the program should ideally be available through the API, however, __Parameters__ allow for certain specific inputs to be linked within a connection project.

Enabling parameters allows easy access through a high-level call in the API, making it a lot easier for a user to programmatically update specifically targeted inputs without needing to understand the finer details of the API.  

Parameters also allow the passing of these high-level links across Connection projects, which is beneficial for designing similar connection types with different loading and geometry configurations.

## Terminology

| Item | Description |
| :--- | :--- |
| **Parameter**  | A parameter is a global value that can be used as input to one or more model properties. Parameters are typeless and hold no specific type or unit information. Parameters are validated only once assigned to a model property. See below for more information.   |
| **Model property**  | Model properties are inputs or selections that are available in a project that can be driven by a parameter. Typically, inputs should mimic the input available in the UI of the project. Only one parameter can be linked to any model property. |
| **Model Property Type**   | Each model property is of a specific type. The model property type provides the requirements for validation. For number-type properties, it also provides the required output unit. Output units are derived from the current unit settings in the project. |
| **Property Item**   | A property item is a high-level item under which model properties are grouped in the model properties selection table. Property items can be either a project item (material, etc) or specific to a connection item (member, operation etc). Some property items include materials, cross-sections, members, and operations. |
| **Model Property Group**   | Model properties are grouped to match different sections of the Program to allow staged application of model properties. These groups include libraries (materials and cross-sections), members, and operations.

## Parameters

Parameters are type-less and unit-less. They are either a number or a string. Where a parameter needs to be applied to a model property with a number input property the parameter should be provided as an SI unit to the Model Property. A conversion will take place to convert it to the appropriate model units assigned to the current Project.

### Parameter input

All parameters are now evaluated as expressions. We use an expression module called [ncalc](https://github.com/ncalc/ncalc/wiki) that provides access to basic math functions and operations. Inputs generally need to be separately identified between strings, decimal numbers, and integers. 

The table below shows how different basic types should be provided in the expression editor.

| **Basic Input Type** | **Description** | **Example** |
| --- | --- | ---|
| **Strings** | strings should be provided with single quotation marks. | `'myString'` |
| **Decimal number<sup>1<sup>** | decimal numbers should have a 0 or Integer prefix and contain a decimal point. | `0.45` |
| **Integer** | Integers should be whole numbers without any decimators. Use `Round( )` where a decimal number is referenced. | `3` |
| **Reference** | reference a previous parameter in the list by using square brackets. | `[param1]` |

1. Currently only decimal point input is allowed when defining parameters. **Decimal commas, based on user PC settings will be supported in future**.

### Units

> Currently, all parameter inputs need to be provided in _basic_ SI Units. Please see the link [here](https://github.com/idea-statica/ideastatica-public/wiki/IOM-Model-Basics) for reference.

### Expression methods

Expression methods can be used to perform the transformation of basic type inputs to more complex inputs required by some Model properties. This primarily involves concatenating strings and providing referencing ability to previously defined parameters. 

The below table provides the basic expression functions typically required to enable the use of parameters: 

| **Expression** | **Description** | **Example Input** | **Example Ouput** |
| --- | -- | ---| --- |
| **ToString(..)** | Converts a number to a string |  ToString(2.1578) | `'2.1578'`
| **ToString(..)** | Concatenates multiple strings into one output |  ToString('M',20,' ','8.8') | `'M20 8.8'`
| **ToString(..)** | Each comma delineated input is also separately evaluated. `spacing` and `bolts` are previously defined parameters |  ToString([spacing],'*',[bolts]-1) | `'.075*3'`
| **RoundNumber(..)** | rounds a decimal number to an Integer |  Round(2.1578, 0) | `2` |
| **RoundNumber(..)** | rounds a decimal number to a precision three |  Round(2.1578, 3) | `2.158` |

For a further and full explanation of available expression methods refer to the [Parameter Reference Guide](https://github.com/idea-statica/ideastatica-public/wiki/Reference-Guide-Expression-Parameters)

### Advanced expression methods

There are further advanced Expression methods that can retrieve parameters currently available to be accessed from parts of an existing model using specific evaluation strings. This is currently an advanced feature and will be developed further in the future. Refer to the [Parameter Reference Guide](https://github.com/idea-statica/ideastatica-public/wiki/Reference-Guide-Expression-Parameters)

## Model properties

### Model property validation

The below table provides basic validation criteria for different model property types.

|  | **Model Property Type** | **Basic Type Validation<sup>1<sup>** | **Example** |
| --- | :--- | --- | :--- |
| **Simple** | String | string |  |
|  | Boolean | string | `'true'` or `'false'` |
|  | Enum | string | `'Rear'` or `'Both'` or `'End'` | 
|  | Integer | int | 3 | 
|  | Float | float, int | `3.0` or `0.12` | 
| **Complex** | Number groups | string (format varies, ref UI) | `'.0075*3'` or `'0.075; 0.05'` |
|  | Vector | string (format 'X Y Z') |  `'0.0 0.0 1.0'` |
| **Library<sup>2<sub>** | Material | string | `'S 355'` |
|  | Bolt Assembly | string | `'M20 8.8'` |
|  | Cross-section | string | `'IPE220'` |

1. Model properties are validated by priority, an attempt for validation will be undertaken on the primary input type. If unsuccessful a parsing/casting to the second validation type will be tried.  
2. Validation of library properties can change depending on the property item that it is assigned to. Refer below.

### Library model properties

Validation of library properties such as Material, Bolt Assembly and Cross-section changes depending on the property item that it is assigned under. 

A library property can be validated in two ways:
* **Method 1.** Against the items currently available in the Project database or; 
* **Method 2.** Against the available possibilities in a user MPRL database

For a Cross-section or Plate Operation project item, Method 1 is used and the _project database_ is searched and that material is referenced. For a Material project item, Method 2 is used, the user's _MPRL_ is searched by name and if found that material is changed to the found material. 

Therefore, parameters **cannot add** materials to the current Project database. These would need to be added manually or through the API. 

## Transferring parameters between projects

Parameters and Model Properties can be exported and used in another project using the developer mode template functionality and available API calls. 
When a template is saved manually from the developer tab, parameters and associated model properties are saved into the template itself. 
 
When the template is then applied to a clean/naked connection (one without operations), those operations are assigned and parameters and model properties should also be available for editing.

Note that the developer tab ‘Templates’ functionality is like that provided through the API for saving and applying templates.

## Further information and examples
* [Webinar on using parameters for optimization](https://www.youtube.com/watch?v=YJ748NLRYIA&t=1067s)
* [Parameter Expression Reference Guide](https://github.com/idea-statica/ideastatica-public/wiki/Reference-Guide-Expression-Parameters)




