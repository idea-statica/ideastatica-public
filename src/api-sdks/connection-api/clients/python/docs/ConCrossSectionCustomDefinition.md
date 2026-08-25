# ConCrossSectionCustomDefinition


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**definition_type** | **str** |  | [optional] [readonly] 
**components** | [**List[ConCrossSectionCustomComponent]**](ConCrossSectionCustomComponent.md) |  | [optional] 
**type** | **str** |  | [optional] [default to 'IdeaStatiCa.Api.Connection.Model.Material.ConCrossSectionCustomDefinition, IdeaStatiCa.Api']

## Example

```python
from ideastatica_connection_api.models.con_cross_section_custom_definition import ConCrossSectionCustomDefinition

# TODO update the JSON string below
json = "{}"
# create an instance of ConCrossSectionCustomDefinition from a JSON string
con_cross_section_custom_definition_instance = ConCrossSectionCustomDefinition.from_json(json)
# print the JSON string representation of the object
print(con_cross_section_custom_definition_instance.to_json())

# convert the object into a dict
con_cross_section_custom_definition_dict = con_cross_section_custom_definition_instance.to_dict()
# create an instance of ConCrossSectionCustomDefinition from a dict
con_cross_section_custom_definition_from_dict = ConCrossSectionCustomDefinition.from_dict(con_cross_section_custom_definition_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


