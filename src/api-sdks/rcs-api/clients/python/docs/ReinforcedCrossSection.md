# ReinforcedCrossSection

Reinforced cross-section

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** | Name of cross-section | [optional] 
**cross_section** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**bars** | [**List[ReinforcedBar]**](ReinforcedBar.md) | Reinforced bars | [optional] 
**stirrups** | [**List[Stirrup]**](Stirrup.md) | Stirrups | [optional] 
**tendon_bars** | [**List[TendonBar]**](TendonBar.md) | Tendon bars | [optional] 
**tendon_ducts** | [**List[TendonDuct]**](TendonDuct.md) | Tendon ducts | [optional] 
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.reinforced_cross_section import ReinforcedCrossSection

# TODO update the JSON string below
json = "{}"
# create an instance of ReinforcedCrossSection from a JSON string
reinforced_cross_section_instance = ReinforcedCrossSection.from_json(json)
# print the JSON string representation of the object
print(ReinforcedCrossSection.to_json())

# convert the object into a dict
reinforced_cross_section_dict = reinforced_cross_section_instance.to_dict()
# create an instance of ReinforcedCrossSection from a dict
reinforced_cross_section_from_dict = ReinforcedCrossSection.from_dict(reinforced_cross_section_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


