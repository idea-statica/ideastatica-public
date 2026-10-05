# ConAddedMemberPositioning


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**positioning_type** | [**ConAddedMemberPositioningEnum**](ConAddedMemberPositioningEnum.md) |  | [optional] 
**angle_alpha** | **float** |  | [optional] 
**angle_beta** | **float** |  | [optional] 
**rotation_rx** | **float** |  | [optional] 
**insert_point** | [**Point3D**](Point3D.md) |  | [optional] 
**eccentricity** | [**Vector3D**](Vector3D.md) |  | [optional] 
**placement_definition** | [**ConStiffeningMemberPlacement**](ConStiffeningMemberPlacement.md) |  | [optional] 
**axis_x** | [**Vector3D**](Vector3D.md) |  | [optional] 
**axis_y** | [**Vector3D**](Vector3D.md) |  | [optional] 
**axis_z** | [**Vector3D**](Vector3D.md) |  | [optional] 
**on_member_id** | **int** |  | [optional] 
**on_plate_index** | **int** |  | [optional] 
**on_plate_operation_id** | **int** |  | [optional] 
**on_member_plate_index** | **int** |  | [optional] 
**added_member_plate_index** | **int** |  | [optional] 
**mount_type** | [**ConPlateFunction**](ConPlateFunction.md) |  | [optional] 
**mount_location** | [**ConLocation**](ConLocation.md) |  | [optional] 
**plate_edge_index** | **int** |  | [optional] 
**position_x** | **float** |  | [optional] 
**position_y** | **float** |  | [optional] 
**rotation** | **float** |  | [optional] 
**pitch** | **float** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_added_member_positioning import ConAddedMemberPositioning

# TODO update the JSON string below
json = "{}"
# create an instance of ConAddedMemberPositioning from a JSON string
con_added_member_positioning_instance = ConAddedMemberPositioning.from_json(json)
# print the JSON string representation of the object
print(con_added_member_positioning_instance.to_json())

# convert the object into a dict
con_added_member_positioning_dict = con_added_member_positioning_instance.to_dict()
# create an instance of ConAddedMemberPositioning from a dict
con_added_member_positioning_from_dict = ConAddedMemberPositioning.from_dict(con_added_member_positioning_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


