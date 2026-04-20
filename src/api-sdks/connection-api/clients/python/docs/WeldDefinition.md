# WeldDefinition


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**thickness** | **float** |  | [optional] 
**weld_type** | [**WeldType**](WeldType.md) |  | [optional] 
**weld_material** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.weld_definition import WeldDefinition

# TODO update the JSON string below
json = "{}"
# create an instance of WeldDefinition from a JSON string
weld_definition_instance = WeldDefinition.from_json(json)
# print the JSON string representation of the object
print(weld_definition_instance.to_json())

# convert the object into a dict
weld_definition_dict = weld_definition_instance.to_dict()
# create an instance of WeldDefinition from a dict
weld_definition_from_dict = WeldDefinition.from_dict(weld_definition_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


