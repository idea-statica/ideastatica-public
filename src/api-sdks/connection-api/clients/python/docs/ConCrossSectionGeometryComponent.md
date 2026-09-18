# ConCrossSectionGeometryComponent


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**outline** | [**List[ConCrossSectionCustomComponentOutlineInner]**](ConCrossSectionCustomComponentOutlineInner.md) |  | [optional] 
**openings** | **List[List[ConCrossSectionCustomComponentOutlineInner]]** |  | [optional] 
**material_name** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_cross_section_geometry_component import ConCrossSectionGeometryComponent

# TODO update the JSON string below
json = "{}"
# create an instance of ConCrossSectionGeometryComponent from a JSON string
con_cross_section_geometry_component_instance = ConCrossSectionGeometryComponent.from_json(json)
# print the JSON string representation of the object
print(con_cross_section_geometry_component_instance.to_json())

# convert the object into a dict
con_cross_section_geometry_component_dict = con_cross_section_geometry_component_instance.to_dict()
# create an instance of ConCrossSectionGeometryComponent from a dict
con_cross_section_geometry_component_from_dict = ConCrossSectionGeometryComponent.from_dict(con_cross_section_geometry_component_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


