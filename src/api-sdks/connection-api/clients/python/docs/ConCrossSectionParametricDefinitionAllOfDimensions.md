# ConCrossSectionParametricDefinitionAllOfDimensions

Polymorphic root. Every element on the wire is one of the concrete subtypes listed in the discriminator mapping and carries the $type discriminator; $type is deliberately declared on each subtype schema (with its exact wire value as default) rather than here.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**value** | **str** |  | [optional] 
**type** | **str** |  | [optional] [default to 'IdeaStatiCa.Api.Connection.Model.Material.ConCssChoiceDimension, IdeaStatiCa.Api']
**options** | [**List[ConCssOption]**](ConCssOption.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_cross_section_parametric_definition_all_of_dimensions import ConCrossSectionParametricDefinitionAllOfDimensions

# TODO update the JSON string below
json = "{}"
# create an instance of ConCrossSectionParametricDefinitionAllOfDimensions from a JSON string
con_cross_section_parametric_definition_all_of_dimensions_instance = ConCrossSectionParametricDefinitionAllOfDimensions.from_json(json)
# print the JSON string representation of the object
print(con_cross_section_parametric_definition_all_of_dimensions_instance.to_json())

# convert the object into a dict
con_cross_section_parametric_definition_all_of_dimensions_dict = con_cross_section_parametric_definition_all_of_dimensions_instance.to_dict()
# create an instance of ConCrossSectionParametricDefinitionAllOfDimensions from a dict
con_cross_section_parametric_definition_all_of_dimensions_from_dict = ConCrossSectionParametricDefinitionAllOfDimensions.from_dict(con_cross_section_parametric_definition_all_of_dimensions_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


