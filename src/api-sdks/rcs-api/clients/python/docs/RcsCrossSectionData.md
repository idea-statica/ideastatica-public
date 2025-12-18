# RcsCrossSectionData


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** |  | [optional] 
**cross_section_rotation** | **float** |  | [optional] 
**components** | [**List[RcsCssComponentData]**](RcsCssComponentData.md) |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_cross_section_data import RcsCrossSectionData

# TODO update the JSON string below
json = "{}"
# create an instance of RcsCrossSectionData from a JSON string
rcs_cross_section_data_instance = RcsCrossSectionData.from_json(json)
# print the JSON string representation of the object
print(rcs_cross_section_data_instance.to_json())

# convert the object into a dict
rcs_cross_section_data_dict = rcs_cross_section_data_instance.to_dict()
# create an instance of RcsCrossSectionData from a dict
rcs_cross_section_data_from_dict = RcsCrossSectionData.from_dict(rcs_cross_section_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


