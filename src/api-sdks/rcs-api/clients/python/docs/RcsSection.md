# RcsSection


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**description** | **str** |  | [optional] 
**check_member_id** | **int** |  | [optional] 
**rcs_id** | **int** |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_section import RcsSection

# TODO update the JSON string below
json = "{}"
# create an instance of RcsSection from a JSON string
rcs_section_instance = RcsSection.from_json(json)
# print the JSON string representation of the object
print(RcsSection.to_json())

# convert the object into a dict
rcs_section_dict = rcs_section_instance.to_dict()
# create an instance of RcsSection from a dict
rcs_section_from_dict = RcsSection.from_dict(rcs_section_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


