# ConMemberPosition

The member's placement. definedBy is rotationsOfX (by betaDirection, gamaPitch and alphaRotation), directionVector (by axisX and alphaRotation) or lcs (by axisX; the stored y and z axes are fitted to it). In a request, definedBy none is refused with 422, and so is a position without definedBy, which reads as none; a response can carry none for a member stored with it. axisX, axisY, axisZ and origin are the resolved local coordinate system in a response; in a request only axisX is read, and it must be a non-zero vector.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**defined_by** | [**ConMemberPlacementDefinitionTypeEnum**](ConMemberPlacementDefinitionTypeEnum.md) |  | [optional] 
**origin** | [**Vector3D**](Vector3D.md) |  | [optional] 
**axis_x** | [**Vector3D**](Vector3D.md) |  | [optional] 
**axis_y** | [**Vector3D**](Vector3D.md) |  | [optional] 
**axis_z** | [**Vector3D**](Vector3D.md) |  | [optional] 
**beta_direction** | **float** |  | [optional] 
**gama_pitch** | **float** |  | [optional] 
**alpha_rotation** | **float** |  | [optional] 
**offset_ex** | **float** |  | [optional] 
**offset_ey** | **float** |  | [optional] 
**offset_ez** | **float** |  | [optional] 
**align** | [**ConMemberAlignmentTypeEnum**](ConMemberAlignmentTypeEnum.md) |  | [optional] 
**aligned_plate** | [**ConAlignedPlate**](ConAlignedPlate.md) |  | [optional] 
**related_plate** | [**ConAlignedPlate**](ConAlignedPlate.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_member_position import ConMemberPosition

# TODO update the JSON string below
json = "{}"
# create an instance of ConMemberPosition from a JSON string
con_member_position_instance = ConMemberPosition.from_json(json)
# print the JSON string representation of the object
print(con_member_position_instance.to_json())

# convert the object into a dict
con_member_position_dict = con_member_position_instance.to_dict()
# create an instance of ConMemberPosition from a dict
con_member_position_from_dict = ConMemberPosition.from_dict(con_member_position_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


