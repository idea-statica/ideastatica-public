# ConParameterCreate


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**key** | **str** |  | [optional] 
**parameter_type** | **str** |  | [optional] 
**expression** | **str** |  | [optional] 
**description** | **str** |  | [optional] 
**lower_bound** | **str** |  | [optional] 
**upper_bound** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_parameter_create import ConParameterCreate

# TODO update the JSON string below
json = "{}"
# create an instance of ConParameterCreate from a JSON string
con_parameter_create_instance = ConParameterCreate.from_json(json)
# print the JSON string representation of the object
print(con_parameter_create_instance.to_json())

# convert the object into a dict
con_parameter_create_dict = con_parameter_create_instance.to_dict()
# create an instance of ConParameterCreate from a dict
con_parameter_create_from_dict = ConParameterCreate.from_dict(con_parameter_create_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


