# IdeaStatiCa.ConnectionApi.Model.ConcreteBlockData
Provides data of the single concrete block

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Id** | **int** | Plate unique ID | [optional] 
**Name** | **string** | Name of the concrete block | [optional] 
**Depth** | **double** | Depth of the concrete block | [optional] 
**Material** | **string** | Name of the material | [optional] 
**Center** | [**Point3D**](Point3D.md) |  | [optional] 
**OutlinePoints** | [**List&lt;Point2D&gt;**](Point2D.md) | Outline points | [optional] 
**Origin** | [**Point3D**](Point3D.md) |  | [optional] 
**AxisX** | [**Vector3D**](Vector3D.md) |  | [optional] 
**AxisY** | [**Vector3D**](Vector3D.md) |  | [optional] 
**AxisZ** | [**Vector3D**](Vector3D.md) |  | [optional] 
**Region** | **string** | Geometry of the concrete block in svg format | [optional] 
**OriginalModelId** | **string** | Get or set the identification in the original model  In the case of the imported connection from another application | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

