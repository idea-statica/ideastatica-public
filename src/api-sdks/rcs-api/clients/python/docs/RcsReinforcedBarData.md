# RcsReinforcedBarData


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**diameter** | **float** |  | [optional] 
**point** | [**Point2D**](Point2D.md) |  | [optional] 
**material** | [**RcsMaterial**](RcsMaterial.md) |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_reinforced_bar_data import RcsReinforcedBarData

# TODO update the JSON string below
json = "{}"
# create an instance of RcsReinforcedBarData from a JSON string
rcs_reinforced_bar_data_instance = RcsReinforcedBarData.from_json(json)
# print the JSON string representation of the object
print(rcs_reinforced_bar_data_instance.to_json())

# convert the object into a dict
rcs_reinforced_bar_data_dict = rcs_reinforced_bar_data_instance.to_dict()
# create an instance of RcsReinforcedBarData from a dict
rcs_reinforced_bar_data_from_dict = RcsReinforcedBarData.from_dict(rcs_reinforced_bar_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


