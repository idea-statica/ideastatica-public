# ConCrossSectionCustomComponent


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**outline** | [**List[ConCssPoint2D]**](ConCssPoint2D.md) |  | [optional] 
**openings** | **List[List[ConCssPoint2D]]** |  | [optional] 
**material_name** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_cross_section_custom_component import ConCrossSectionCustomComponent

# TODO update the JSON string below
json = "{}"
# create an instance of ConCrossSectionCustomComponent from a JSON string
con_cross_section_custom_component_instance = ConCrossSectionCustomComponent.from_json(json)
# print the JSON string representation of the object
print(con_cross_section_custom_component_instance.to_json())

# convert the object into a dict
con_cross_section_custom_component_dict = con_cross_section_custom_component_instance.to_dict()
# create an instance of ConCrossSectionCustomComponent from a dict
con_cross_section_custom_component_from_dict = ConCrossSectionCustomComponent.from_dict(con_cross_section_custom_component_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


