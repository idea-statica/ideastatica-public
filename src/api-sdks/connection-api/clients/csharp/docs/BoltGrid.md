# IdeaStatiCa.ConnectionApi.Model.BoltGrid
Data of the bolt grid

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**BoltAssemblyRef** | **string** |  | [optional] 
**Id** | **int** | Unique Id of the bolt grid | [optional] 
**IsAnchor** | **bool** | Is Anchor | [optional] 
**AnchorLen** | **double** | Anchor lenght | [optional] 
**HoleDiameter** | **double** | The diameter of the hole | [optional] 
**Diameter** | **double** | The diameter of bolt | [optional] 
**HeadDiameter** | **double** | The head diameter of bolt | [optional] 
**DiagonalHeadDiameter** | **double** | The Diagonal Head Diameter of bolt | [optional] 
**HeadHeight** | **double** | The Head Height of bolt | [optional] 
**BoreHole** | **double** | The BoreHole of bolt | [optional] 
**TensileStressArea** | **double** | The Tensile Stress Area of bolt | [optional] 
**NutThickness** | **double** | The Nut Thickness of bolt | [optional] 
**BoltAssemblyName** | **string** | The description of the bolt assembly | [optional] 
**Standard** | **string** | The standard of the bolt assembly | [optional] 
**Material** | **string** | The material of the bolt assembly | [optional] 
**Origin** | [**Point3D**](Point3D.md) |  | [optional] 
**AxisX** | [**Vector3D**](Vector3D.md) |  | [optional] 
**AxisY** | [**Vector3D**](Vector3D.md) |  | [optional] 
**AxisZ** | [**Vector3D**](Vector3D.md) |  | [optional] 
**Positions** | [**List&lt;Point3D&gt;**](Point3D.md) | Positions of holes in the local coodinate system of the bolt grid | [optional] 
**ConnectedPlates** | **List&lt;int&gt;** | Identifiers of the connected plates | [optional] 
**ConnectedPartIds** | **List&lt;string&gt;** | Id of the weld | [optional] 
**ShearInThread** | **bool** | Indicates, whether a shear plane is in the thread of a bolt. | [optional] 
**BoltInteraction** | **BoltShearType** |  | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

