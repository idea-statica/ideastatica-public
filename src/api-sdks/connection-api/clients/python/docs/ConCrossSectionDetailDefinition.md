# ConCrossSectionDetailDefinition

Polymorphic root. Every element on the wire is one of the concrete subtypes listed in the discriminator mapping and carries the $type discriminator; $type is deliberately declared on each subtype schema (with its exact wire value as default) rather than here.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**definition_type** | **str** |  | [optional] [readonly] 
**material_name** | **str** |  | [optional] 
**shape_type** | **str** |  | [optional] 
**parameters** | [**List[ConCrossSectionParameter]**](ConCrossSectionParameter.md) |  | [optional] 
**type** | **str** |  | [optional] [default to 'IdeaStatiCa.Api.Connection.Model.Material.ConCrossSectionCustomDefinition, IdeaStatiCa.Api']
**components** | [**List[ConCrossSectionCustomComponent]**](ConCrossSectionCustomComponent.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_cross_section_detail_definition import ConCrossSectionDetailDefinition

# TODO update the JSON string below
json = "{}"
# create an instance of ConCrossSectionDetailDefinition from a JSON string
con_cross_section_detail_definition_instance = ConCrossSectionDetailDefinition.from_json(json)
# print the JSON string representation of the object
print(con_cross_section_detail_definition_instance.to_json())

# convert the object into a dict
con_cross_section_detail_definition_dict = con_cross_section_detail_definition_instance.to_dict()
# create an instance of ConCrossSectionDetailDefinition from a dict
con_cross_section_detail_definition_from_dict = ConCrossSectionDetailDefinition.from_dict(con_cross_section_detail_definition_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


