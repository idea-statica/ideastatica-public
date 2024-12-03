# RcsSectionLoading


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**section_id** | **int** |  | [optional] 
**loading_xml** | **str** |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_section_loading import RcsSectionLoading

# TODO update the JSON string below
json = "{}"
# create an instance of RcsSectionLoading from a JSON string
rcs_section_loading_instance = RcsSectionLoading.from_json(json)
# print the JSON string representation of the object
print(RcsSectionLoading.to_json())

# convert the object into a dict
rcs_section_loading_dict = rcs_section_loading_instance.to_dict()
# create an instance of RcsSectionLoading from a dict
rcs_section_loading_from_dict = RcsSectionLoading.from_dict(rcs_section_loading_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


