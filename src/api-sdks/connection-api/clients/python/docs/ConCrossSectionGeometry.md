# ConCrossSectionGeometry


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**components** | [**List[ConCrossSectionGeometryComponent]**](ConCrossSectionGeometryComponent.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_cross_section_geometry import ConCrossSectionGeometry

# TODO update the JSON string below
json = "{}"
# create an instance of ConCrossSectionGeometry from a JSON string
con_cross_section_geometry_instance = ConCrossSectionGeometry.from_json(json)
# print the JSON string representation of the object
print(con_cross_section_geometry_instance.to_json())

# convert the object into a dict
con_cross_section_geometry_dict = con_cross_section_geometry_instance.to_dict()
# create an instance of ConCrossSectionGeometry from a dict
con_cross_section_geometry_from_dict = ConCrossSectionGeometry.from_dict(con_cross_section_geometry_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


