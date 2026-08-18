# ConCrossSectionParameter


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**value** | **float** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_cross_section_parameter import ConCrossSectionParameter

# TODO update the JSON string below
json = "{}"
# create an instance of ConCrossSectionParameter from a JSON string
con_cross_section_parameter_instance = ConCrossSectionParameter.from_json(json)
# print the JSON string representation of the object
print(con_cross_section_parameter_instance.to_json())

# convert the object into a dict
con_cross_section_parameter_dict = con_cross_section_parameter_instance.to_dict()
# create an instance of ConCrossSectionParameter from a dict
con_cross_section_parameter_from_dict = ConCrossSectionParameter.from_dict(con_cross_section_parameter_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


