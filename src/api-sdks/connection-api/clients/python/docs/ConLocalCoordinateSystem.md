# ConLocalCoordinateSystem


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**origin** | [**Point3D**](Point3D.md) |  | [optional] 
**x_axis** | [**Vector3D**](Vector3D.md) |  | [optional] 
**y_axis** | [**Vector3D**](Vector3D.md) |  | [optional] 
**z_axis** | [**Vector3D**](Vector3D.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_local_coordinate_system import ConLocalCoordinateSystem

# TODO update the JSON string below
json = "{}"
# create an instance of ConLocalCoordinateSystem from a JSON string
con_local_coordinate_system_instance = ConLocalCoordinateSystem.from_json(json)
# print the JSON string representation of the object
print(con_local_coordinate_system_instance.to_json())

# convert the object into a dict
con_local_coordinate_system_dict = con_local_coordinate_system_instance.to_dict()
# create an instance of ConLocalCoordinateSystem from a dict
con_local_coordinate_system_from_dict = ConLocalCoordinateSystem.from_dict(con_local_coordinate_system_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


