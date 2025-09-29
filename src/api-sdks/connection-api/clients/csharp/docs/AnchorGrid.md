# IdeaStatiCa.ConnectionApi.Model.AnchorGrid
Data of the anchor grid

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**ShearInThread** | **bool** | Indicates, whether a shear plane is in the thread of a bolt. | [optional] 
**ConcreteBlock** | [**ConcreteBlock**](ConcreteBlock.md) |  | [optional] 
**AnchorType** | **AnchorType** |  | [optional] 
**WasherSize** | **double** | Washer Size used if AnchorType is washer | [optional] 
**AnchoringLength** | **double** | Anchoring Length | [optional] 
**HookLength** | **double** | Length of anchor hook    (distance from the inner surface of the anchor shaft to the outer tip of the hook specified as an anchor diameter multiplier) | [optional] 
**BoltAssembly** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**Origin** | [**Point3D**](Point3D.md) |  | [optional] 
**AxisX** | [**Vector3D**](Vector3D.md) |  | [optional] 
**AxisY** | [**Vector3D**](Vector3D.md) |  | [optional] 
**AxisZ** | [**Vector3D**](Vector3D.md) |  | [optional] 
**Positions** | [**List&lt;Point3D&gt;**](Point3D.md) | Positions of holes in the local coordinate system of the grid | [optional] 
**ConnectedParts** | [**List&lt;ReferenceElement&gt;**](ReferenceElement.md) | List of the connected parts | [optional] 
**Name** | **string** | Name | [optional] 
**Length** | **double** | Length | [optional] 
**Id** | **int** | Element Id | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

