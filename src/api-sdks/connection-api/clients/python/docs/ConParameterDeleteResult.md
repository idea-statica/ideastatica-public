# ConParameterDeleteResult


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**key** | **str** |  | [optional] 
**deleted_links** | **int** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_parameter_delete_result import ConParameterDeleteResult

# TODO update the JSON string below
json = "{}"
# create an instance of ConParameterDeleteResult from a JSON string
con_parameter_delete_result_instance = ConParameterDeleteResult.from_json(json)
# print the JSON string representation of the object
print(con_parameter_delete_result_instance.to_json())

# convert the object into a dict
con_parameter_delete_result_dict = con_parameter_delete_result_instance.to_dict()
# create an instance of ConParameterDeleteResult from a dict
con_parameter_delete_result_from_dict = ConParameterDeleteResult.from_dict(con_parameter_delete_result_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


