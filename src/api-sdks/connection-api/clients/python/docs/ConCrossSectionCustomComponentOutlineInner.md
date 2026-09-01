# ConCrossSectionCustomComponentOutlineInner

Polymorphic root. Every element on the wire is one of the concrete subtypes listed in the discriminator mapping and carries the $type discriminator; $type is deliberately declared on each subtype schema (with its exact wire value as default) rather than here.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**segment_type** | **str** |  | [optional] [readonly] 
**start** | [**ConCssPoint2D**](ConCssPoint2D.md) |  | [optional] 
**end** | [**ConCssPoint2D**](ConCssPoint2D.md) |  | [optional] 
**mid** | [**ConCssPoint2D**](ConCssPoint2D.md) |  | [optional] 
**type** | **str** |  | [optional] [default to 'IdeaStatiCa.Api.Connection.Model.Material.ConCssArcSegment, IdeaStatiCa.Api']

## Example

```python
from ideastatica_connection_api.models.con_cross_section_custom_component_outline_inner import ConCrossSectionCustomComponentOutlineInner

# TODO update the JSON string below
json = "{}"
# create an instance of ConCrossSectionCustomComponentOutlineInner from a JSON string
con_cross_section_custom_component_outline_inner_instance = ConCrossSectionCustomComponentOutlineInner.from_json(json)
# print the JSON string representation of the object
print(con_cross_section_custom_component_outline_inner_instance.to_json())

# convert the object into a dict
con_cross_section_custom_component_outline_inner_dict = con_cross_section_custom_component_outline_inner_instance.to_dict()
# create an instance of ConCrossSectionCustomComponentOutlineInner from a dict
con_cross_section_custom_component_outline_inner_from_dict = ConCrossSectionCustomComponentOutlineInner.from_dict(con_cross_section_custom_component_outline_inner_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


