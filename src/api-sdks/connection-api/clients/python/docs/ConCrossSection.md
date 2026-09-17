# ConCrossSection


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**definition** | [**ConCrossSectionDefinition**](ConCrossSectionDefinition.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_cross_section import ConCrossSection

# TODO update the JSON string below
json = "{}"
# create an instance of ConCrossSection from a JSON string
con_cross_section_instance = ConCrossSection.from_json(json)
# print the JSON string representation of the object
print(con_cross_section_instance.to_json())

# convert the object into a dict
con_cross_section_dict = con_cross_section_instance.to_dict()
# create an instance of ConCrossSection from a dict
con_cross_section_from_dict = ConCrossSection.from_dict(con_cross_section_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


