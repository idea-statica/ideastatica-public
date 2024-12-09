# RcsReinforcedCrosssSectionImportSetting


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**reinforced_cross_section_id** | **int** |  | [optional] 
**parts_to_import** | **str** |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_reinforced_crosss_section_import_setting import RcsReinforcedCrosssSectionImportSetting

# TODO update the JSON string below
json = "{}"
# create an instance of RcsReinforcedCrosssSectionImportSetting from a JSON string
rcs_reinforced_crosss_section_import_setting_instance = RcsReinforcedCrosssSectionImportSetting.from_json(json)
# print the JSON string representation of the object
print(RcsReinforcedCrosssSectionImportSetting.to_json())

# convert the object into a dict
rcs_reinforced_crosss_section_import_setting_dict = rcs_reinforced_crosss_section_import_setting_instance.to_dict()
# create an instance of RcsReinforcedCrosssSectionImportSetting from a dict
rcs_reinforced_crosss_section_import_setting_from_dict = RcsReinforcedCrosssSectionImportSetting.from_dict(rcs_reinforced_crosss_section_import_setting_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


