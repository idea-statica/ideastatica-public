# ReinforcedCrossSectionData


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** |  | [optional] 
**cross_section** | [**RcsCrossSectionData**](RcsCrossSectionData.md) |  | [optional] 
**bars** | [**List[RcsReinforcedBarData]**](RcsReinforcedBarData.md) |  | [optional] 
**stirrups** | [**List[RcsStirrupsData]**](RcsStirrupsData.md) |  | [optional] 
**tendon_bars** | [**List[RcsTendonBarData]**](RcsTendonBarData.md) |  | [optional] 
**tendon_ducts** | [**List[RcsTendonDuctData]**](RcsTendonDuctData.md) |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.reinforced_cross_section_data import ReinforcedCrossSectionData

# TODO update the JSON string below
json = "{}"
# create an instance of ReinforcedCrossSectionData from a JSON string
reinforced_cross_section_data_instance = ReinforcedCrossSectionData.from_json(json)
# print the JSON string representation of the object
print(reinforced_cross_section_data_instance.to_json())

# convert the object into a dict
reinforced_cross_section_data_dict = reinforced_cross_section_data_instance.to_dict()
# create an instance of ReinforcedCrossSectionData from a dict
reinforced_cross_section_data_from_dict = ReinforcedCrossSectionData.from_dict(reinforced_cross_section_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


