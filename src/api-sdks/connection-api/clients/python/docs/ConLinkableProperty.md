# ConLinkableProperty


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**property_id** | **str** |  | [optional] 
**name** | **str** |  | [optional] 
**group** | **str** |  | [optional] 
**description** | **str** |  | [optional] 
**value_type** | **str** |  | [optional] 
**unit** | **str** |  | [optional] 
**current_value** | **object** |  | [optional] 
**linked_parameter** | **str** |  | [optional] 
**owner** | [**ConPropertyOwner**](ConPropertyOwner.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_linkable_property import ConLinkableProperty

# TODO update the JSON string below
json = "{}"
# create an instance of ConLinkableProperty from a JSON string
con_linkable_property_instance = ConLinkableProperty.from_json(json)
# print the JSON string representation of the object
print(con_linkable_property_instance.to_json())

# convert the object into a dict
con_linkable_property_dict = con_linkable_property_instance.to_dict()
# create an instance of ConLinkableProperty from a dict
con_linkable_property_from_dict = ConLinkableProperty.from_dict(con_linkable_property_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


