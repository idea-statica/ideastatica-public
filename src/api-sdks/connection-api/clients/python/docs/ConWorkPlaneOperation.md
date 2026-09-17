# ConWorkPlaneOperation


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**method** | [**ConWorkPlaneMethod**](ConWorkPlaneMethod.md) |  | [optional] 
**related_to** | [**ConWorkPlaneRelatedTo**](ConWorkPlaneRelatedTo.md) |  | [optional] 
**origin** | [**Point3D**](Point3D.md) |  | [optional] 
**normal_vector** | [**Vector3D**](Vector3D.md) |  | [optional] 
**rotation_x** | **float** |  | [optional] 
**rotation_y** | **float** |  | [optional] 
**rotation_z** | **float** |  | [optional] 
**is_fatigue** | **bool** |  | [optional] 
**member_id** | **int** |  | [optional] 
**plate_id** | **int** |  | [optional] 
**plate_operation_id** | **int** |  | [optional] 
**related_member_id** | **int** |  | [optional] 
**offset_x** | **float** |  | [optional] 
**near_intersection** | **bool** |  | [optional] 
**is_imported** | **bool** |  | [optional] 
**operation_type** | **str** |  | [optional] 
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**active** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_work_plane_operation import ConWorkPlaneOperation

# TODO update the JSON string below
json = "{}"
# create an instance of ConWorkPlaneOperation from a JSON string
con_work_plane_operation_instance = ConWorkPlaneOperation.from_json(json)
# print the JSON string representation of the object
print(con_work_plane_operation_instance.to_json())

# convert the object into a dict
con_work_plane_operation_dict = con_work_plane_operation_instance.to_dict()
# create an instance of ConWorkPlaneOperation from a dict
con_work_plane_operation_from_dict = ConWorkPlaneOperation.from_dict(con_work_plane_operation_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


