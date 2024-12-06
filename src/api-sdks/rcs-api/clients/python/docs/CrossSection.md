# CrossSection

Cross-section

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** | Name of cross-section | [optional] 
**cross_section_rotation** | **float** | Rotation of Cross - Section | [optional] 
**is_in_principal** | **bool** | Specifies that the cross-section is in its principal axis. | [optional] 
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.cross_section import CrossSection

# TODO update the JSON string below
json = "{}"
# create an instance of CrossSection from a JSON string
cross_section_instance = CrossSection.from_json(json)
# print the JSON string representation of the object
print(CrossSection.to_json())

# convert the object into a dict
cross_section_dict = cross_section_instance.to_dict()
# create an instance of CrossSection from a dict
cross_section_from_dict = CrossSection.from_dict(cross_section_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


