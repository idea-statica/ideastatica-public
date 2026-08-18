# ConCrossSectionParametricDefinition


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**definition_type** | **str** |  | [optional] [readonly] 
**shape_type** | **str** |  | [optional] 
**parameters** | [**List[ConCrossSectionParameter]**](ConCrossSectionParameter.md) |  | [optional] 
**type** | **str** |  | [optional] [default to 'IdeaStatiCa.Api.Connection.Model.Material.ConCrossSectionParametricDefinition, IdeaStatiCa.Api']

## Example

```python
from ideastatica_connection_api.models.con_cross_section_parametric_definition import ConCrossSectionParametricDefinition

# TODO update the JSON string below
json = "{}"
# create an instance of ConCrossSectionParametricDefinition from a JSON string
con_cross_section_parametric_definition_instance = ConCrossSectionParametricDefinition.from_json(json)
# print the JSON string representation of the object
print(con_cross_section_parametric_definition_instance.to_json())

# convert the object into a dict
con_cross_section_parametric_definition_dict = con_cross_section_parametric_definition_instance.to_dict()
# create an instance of ConCrossSectionParametricDefinition from a dict
con_cross_section_parametric_definition_from_dict = ConCrossSectionParametricDefinition.from_dict(con_cross_section_parametric_definition_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


