# IdeaStatiCa.ConnectionApi.Model.CutBeamByBeamData
Provides data of the cut objec by object

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Name** | **string** | Name of the cut | [optional] 
**ModifiedObject** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**CuttingObject** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**IsWeld** | **bool** | is cut welded | [optional] 
**WebWeld** | [**WeldDefinition**](WeldDefinition.md) |  | [optional] 
**FlangeWeld** | [**WeldDefinition**](WeldDefinition.md) |  | [optional] 
**Offset** | **double** | Offset | [optional] 
**Method** | **CutMethod** |  | [optional] 
**Orientation** | **CutOrientation** |  | [optional] 
**PlaneOnCuttingObject** | **DistanceComparison** |  | [optional] 
**CutPart** | **CutPart** |  | [optional] 
**ExtendBeforeCut** | **bool** | Extend before cut - for cuts where user can decide if modified beam will be extended or not | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

