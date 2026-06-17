# ResultOnMesh


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**nodes** | **List[List[float]]** |  | [optional] 
**plates_element** | [**List[PlateElements]**](PlateElements.md) | List of elements for each plate UID | [optional] 
**displacement** | **List[List[float]]** |  | [optional] 
**value** | **List[float]** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.result_on_mesh import ResultOnMesh

# TODO update the JSON string below
json = "{}"
# create an instance of ResultOnMesh from a JSON string
result_on_mesh_instance = ResultOnMesh.from_json(json)
# print the JSON string representation of the object
print(result_on_mesh_instance.to_json())

# convert the object into a dict
result_on_mesh_dict = result_on_mesh_instance.to_dict()
# create an instance of ResultOnMesh from a dict
result_on_mesh_from_dict = ResultOnMesh.from_dict(result_on_mesh_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


