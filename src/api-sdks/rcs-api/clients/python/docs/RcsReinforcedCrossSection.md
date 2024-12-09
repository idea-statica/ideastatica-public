# RcsReinforcedCrossSection


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**cross_section_id** | **int** |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_reinforced_cross_section import RcsReinforcedCrossSection

# TODO update the JSON string below
json = "{}"
# create an instance of RcsReinforcedCrossSection from a JSON string
rcs_reinforced_cross_section_instance = RcsReinforcedCrossSection.from_json(json)
# print the JSON string representation of the object
print(RcsReinforcedCrossSection.to_json())

# convert the object into a dict
rcs_reinforced_cross_section_dict = rcs_reinforced_cross_section_instance.to_dict()
# create an instance of RcsReinforcedCrossSection from a dict
rcs_reinforced_cross_section_from_dict = RcsReinforcedCrossSection.from_dict(rcs_reinforced_cross_section_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


