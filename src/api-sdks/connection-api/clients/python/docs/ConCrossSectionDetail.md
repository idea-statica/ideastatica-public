# ConCrossSectionDetail


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**definition** | [**ConCrossSectionDetailDefinition**](ConCrossSectionDetailDefinition.md) |  | [optional] 
**geometry** | [**ConCrossSectionGeometry**](ConCrossSectionGeometry.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_cross_section_detail import ConCrossSectionDetail

# TODO update the JSON string below
json = "{}"
# create an instance of ConCrossSectionDetail from a JSON string
con_cross_section_detail_instance = ConCrossSectionDetail.from_json(json)
# print the JSON string representation of the object
print(con_cross_section_detail_instance.to_json())

# convert the object into a dict
con_cross_section_detail_dict = con_cross_section_detail_instance.to_dict()
# create an instance of ConCrossSectionDetail from a dict
con_cross_section_detail_from_dict = ConCrossSectionDetail.from_dict(con_cross_section_detail_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


