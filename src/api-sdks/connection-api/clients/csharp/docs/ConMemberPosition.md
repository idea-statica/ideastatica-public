# IdeaStatiCa.ConnectionApi.Model.ConMemberPosition
The member's placement. definedBy is rotationsOfX (by betaDirection, gamaPitch and alphaRotation), directionVector (by axisX and alphaRotation) or lcs (by axisX; the stored y and z axes are fitted to it). In a request, definedBy none is refused with 422, and so is a position without definedBy, which reads as none; a response can carry none for a member stored with it. axisX, axisY, axisZ and origin are the resolved local coordinate system in a response; in a request only axisX is read, and it must be a non-zero vector.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**DefinedBy** | **ConMemberPlacementDefinitionTypeEnum** |  | [optional] 
**Origin** | [**Vector3D**](Vector3D.md) |  | [optional] 
**AxisX** | [**Vector3D**](Vector3D.md) |  | [optional] 
**AxisY** | [**Vector3D**](Vector3D.md) |  | [optional] 
**AxisZ** | [**Vector3D**](Vector3D.md) |  | [optional] 
**BetaDirection** | **double?** |  | [optional] 
**GamaPitch** | **double?** |  | [optional] 
**AlphaRotation** | **double** |  | [optional] 
**OffsetEx** | **double** |  | [optional] 
**OffsetEy** | **double** |  | [optional] 
**OffsetEz** | **double** |  | [optional] 
**Align** | **ConMemberAlignmentTypeEnum** |  | [optional] 
**AlignedPlate** | [**ConAlignedPlate**](ConAlignedPlate.md) |  | [optional] 
**RelatedPlate** | [**ConAlignedPlate**](ConAlignedPlate.md) |  | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

