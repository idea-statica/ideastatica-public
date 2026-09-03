# ConPropertyOwner


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**kind** | [**ConPropertyOwnerKind**](ConPropertyOwnerKind.md) |  | [optional] 
**operation_id** | **int** |  | [optional] 
**member_id** | **int** |  | [optional] 
**cross_section_id** | **int** |  | [optional] 
**material_id** | **int** |  | [optional] 
**bolt_assembly_id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_property_owner import ConPropertyOwner

# TODO update the JSON string below
json = "{}"
# create an instance of ConPropertyOwner from a JSON string
con_property_owner_instance = ConPropertyOwner.from_json(json)
# print the JSON string representation of the object
print(con_property_owner_instance.to_json())

# convert the object into a dict
con_property_owner_dict = con_property_owner_instance.to_dict()
# create an instance of ConPropertyOwner from a dict
con_property_owner_from_dict = ConPropertyOwner.from_dict(con_property_owner_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


