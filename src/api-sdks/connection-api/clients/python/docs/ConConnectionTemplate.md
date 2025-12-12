# ConConnectionTemplate


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**library_template_id** | **str** |  | [optional] 
**template_id** | **int** |  | [optional] 
**members** | [**List[ConItem]**](ConItem.md) |  | [optional] 
**operations** | [**List[ConItem]**](ConItem.md) |  | [optional] 
**parameter_keys** | **List[str]** |  | [optional] 
**common_properties** | [**ConOperationCommonProperties**](ConOperationCommonProperties.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_connection_template import ConConnectionTemplate

# TODO update the JSON string below
json = "{}"
# create an instance of ConConnectionTemplate from a JSON string
con_connection_template_instance = ConConnectionTemplate.from_json(json)
# print the JSON string representation of the object
print(con_connection_template_instance.to_json())

# convert the object into a dict
con_connection_template_dict = con_connection_template_instance.to_dict()
# create an instance of ConConnectionTemplate from a dict
con_connection_template_from_dict = ConConnectionTemplate.from_dict(con_connection_template_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


