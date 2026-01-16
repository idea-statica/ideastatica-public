# RcsTendonBarData


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**point** | [**Point2D**](Point2D.md) |  | [optional] 
**material** | [**RcsMaterial**](RcsMaterial.md) |  | [optional] 
**num_strands_in_tendon** | **int** |  | [optional] 
**prestressing_order** | **int** |  | [optional] 
**tendon_duct** | [**RcsTendonDuctData**](RcsTendonDuctData.md) |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_tendon_bar_data import RcsTendonBarData

# TODO update the JSON string below
json = "{}"
# create an instance of RcsTendonBarData from a JSON string
rcs_tendon_bar_data_instance = RcsTendonBarData.from_json(json)
# print the JSON string representation of the object
print(rcs_tendon_bar_data_instance.to_json())

# convert the object into a dict
rcs_tendon_bar_data_dict = rcs_tendon_bar_data_instance.to_dict()
# create an instance of RcsTendonBarData from a dict
rcs_tendon_bar_data_from_dict = RcsTendonBarData.from_dict(rcs_tendon_bar_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


