# BendData

Provides data of bend

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**plate1_id** | **int** | First plate | [optional] 
**plate2_id** | **int** | Second plate | [optional] 
**radius** | **float** | Radius of bend | [optional] 
**point1_of_side_boundary1** | [**Point3D**](Point3D.md) |  | [optional] 
**point2_of_side_boundary1** | [**Point3D**](Point3D.md) |  | [optional] 
**end_face_normal1** | [**Vector3D**](Vector3D.md) |  | [optional] 
**point1_of_side_boundary2** | [**Point3D**](Point3D.md) |  | [optional] 
**point2_of_side_boundary2** | [**Point3D**](Point3D.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.bend_data import BendData

# TODO update the JSON string below
json = "{}"
# create an instance of BendData from a JSON string
bend_data_instance = BendData.from_json(json)
# print the JSON string representation of the object
print(BendData.to_json())

# convert the object into a dict
bend_data_dict = bend_data_instance.to_dict()
# create an instance of BendData from a dict
bend_data_from_dict = BendData.from_dict(bend_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


