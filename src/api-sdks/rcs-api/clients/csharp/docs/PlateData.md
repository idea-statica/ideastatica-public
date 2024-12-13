# IdeaStatiCa.RcsApi.Model.PlateData
Provides data of the single plate

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Name** | **string** | Name of the plate | [optional] 
**Thickness** | **double** | Thickness of the plate | [optional] 
**Material** | **string** | Name of the material | [optional] 
**OutlinePoints** | [**List&lt;Point2D&gt;**](Point2D.md) | Outline points | [optional] 
**Origin** | [**Point3D**](Point3D.md) |  | [optional] 
**AxisX** | [**Vector3D**](Vector3D.md) |  | [optional] 
**AxisY** | [**Vector3D**](Vector3D.md) |  | [optional] 
**AxisZ** | [**Vector3D**](Vector3D.md) |  | [optional] 
**Region** | **string** | Geometry of the plate in svg format. In next version will be mark as OBSOLETE! New use property Geometry | [optional] 
**Geometry** | [**Region2D**](Region2D.md) |  | [optional] 
**OriginalModelId** | **string** | Get or set the identification in the original model  In the case of the imported connection from another application | [optional] 
**IsNegativeObject** | **bool** | Is negative object | [optional] 
**Id** | **int** | Element Id | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

