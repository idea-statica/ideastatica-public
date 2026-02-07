# RcsCssComponentData


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**material_name** | **str** |  | [optional] 
**phase** | **int** |  | [optional] 
**geometry** | [**Region2D**](Region2D.md) |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_css_component_data import RcsCssComponentData

# TODO update the JSON string below
json = "{}"
# create an instance of RcsCssComponentData from a JSON string
rcs_css_component_data_instance = RcsCssComponentData.from_json(json)
# print the JSON string representation of the object
print(rcs_css_component_data_instance.to_json())

# convert the object into a dict
rcs_css_component_data_dict = rcs_css_component_data_instance.to_dict()
# create an instance of RcsCssComponentData from a dict
rcs_css_component_data_from_dict = RcsCssComponentData.from_dict(rcs_css_component_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


