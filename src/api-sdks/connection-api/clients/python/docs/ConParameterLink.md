# ConParameterLink


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**parameter** | **str** |  | [optional] 
**owner** | [**ConPropertyOwner**](ConPropertyOwner.md) |  | [optional] 
**property_id** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_parameter_link import ConParameterLink

# TODO update the JSON string below
json = "{}"
# create an instance of ConParameterLink from a JSON string
con_parameter_link_instance = ConParameterLink.from_json(json)
# print the JSON string representation of the object
print(con_parameter_link_instance.to_json())

# convert the object into a dict
con_parameter_link_dict = con_parameter_link_instance.to_dict()
# create an instance of ConParameterLink from a dict
con_parameter_link_from_dict = ConParameterLink.from_dict(con_parameter_link_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


