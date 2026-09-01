# ConParameterLinkCreate


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**parameter** | **str** |  | [optional] 
**kind** | [**ConPropertyOwnerKind**](ConPropertyOwnerKind.md) |  | [optional] 
**operation_id** | **int** |  | [optional] 
**member_id** | **int** |  | [optional] 
**property_id** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_parameter_link_create import ConParameterLinkCreate

# TODO update the JSON string below
json = "{}"
# create an instance of ConParameterLinkCreate from a JSON string
con_parameter_link_create_instance = ConParameterLinkCreate.from_json(json)
# print the JSON string representation of the object
print(con_parameter_link_create_instance.to_json())

# convert the object into a dict
con_parameter_link_create_dict = con_parameter_link_create_instance.to_dict()
# create an instance of ConParameterLinkCreate from a dict
con_parameter_link_create_from_dict = ConParameterLinkCreate.from_dict(con_parameter_link_create_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


