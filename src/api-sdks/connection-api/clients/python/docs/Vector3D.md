# Vector3D


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**x** | **float** |  | [optional] 
**y** | **float** |  | [optional] 
**z** | **float** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.vector3_d import Vector3D

# TODO update the JSON string below
json = "{}"
# create an instance of Vector3D from a JSON string
vector3_d_instance = Vector3D.from_json(json)
# print the JSON string representation of the object
print(vector3_d_instance.to_json())

# convert the object into a dict
vector3_d_dict = vector3_d_instance.to_dict()
# create an instance of Vector3D from a dict
vector3_d_from_dict = Vector3D.from_dict(vector3_d_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


