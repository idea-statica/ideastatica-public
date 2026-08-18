# ConCrossSectionDefinition

Polymorphic root. Every element on the wire is one of the concrete subtypes listed in the discriminator mapping and carries the $type discriminator; $type is deliberately declared on each subtype schema (with its exact wire value as default) rather than here.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**definition_type** | **str** |  | [optional] [readonly] 
**material_name** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_cross_section_definition import ConCrossSectionDefinition

# TODO update the JSON string below
json = "{}"
# create an instance of ConCrossSectionDefinition from a JSON string
con_cross_section_definition_instance = ConCrossSectionDefinition.from_json(json)
# print the JSON string representation of the object
print(con_cross_section_definition_instance.to_json())

# convert the object into a dict
con_cross_section_definition_dict = con_cross_section_definition_instance.to_dict()
# create an instance of ConCrossSectionDefinition from a dict
con_cross_section_definition_from_dict = ConCrossSectionDefinition.from_dict(con_cross_section_definition_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


