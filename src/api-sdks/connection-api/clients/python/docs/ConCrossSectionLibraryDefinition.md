# ConCrossSectionLibraryDefinition


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**definition_type** | **str** |  | [optional] [readonly] 
**mprl_name** | **str** |  | [optional] 
**mirror_y** | **bool** |  | [optional] 
**mirror_z** | **bool** |  | [optional] 
**type** | **str** |  | [optional] [default to 'IdeaStatiCa.Api.Connection.Model.Material.ConCrossSectionLibraryDefinition, IdeaStatiCa.Api']

## Example

```python
from ideastatica_connection_api.models.con_cross_section_library_definition import ConCrossSectionLibraryDefinition

# TODO update the JSON string below
json = "{}"
# create an instance of ConCrossSectionLibraryDefinition from a JSON string
con_cross_section_library_definition_instance = ConCrossSectionLibraryDefinition.from_json(json)
# print the JSON string representation of the object
print(con_cross_section_library_definition_instance.to_json())

# convert the object into a dict
con_cross_section_library_definition_dict = con_cross_section_library_definition_instance.to_dict()
# create an instance of ConCrossSectionLibraryDefinition from a dict
con_cross_section_library_definition_from_dict = ConCrossSectionLibraryDefinition.from_dict(con_cross_section_library_definition_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


