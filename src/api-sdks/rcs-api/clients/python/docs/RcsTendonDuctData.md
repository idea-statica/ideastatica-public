# RcsTendonDuctData


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**point** | [**Point2D**](Point2D.md) |  | [optional] 
**diameter** | **float** |  | [optional] 
**is_debonding_tube** | **bool** |  | [optional] 
**material_duct** | [**MaterialDuct**](MaterialDuct.md) |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_tendon_duct_data import RcsTendonDuctData

# TODO update the JSON string below
json = "{}"
# create an instance of RcsTendonDuctData from a JSON string
rcs_tendon_duct_data_instance = RcsTendonDuctData.from_json(json)
# print the JSON string representation of the object
print(rcs_tendon_duct_data_instance.to_json())

# convert the object into a dict
rcs_tendon_duct_data_dict = rcs_tendon_duct_data_instance.to_dict()
# create an instance of RcsTendonDuctData from a dict
rcs_tendon_duct_data_from_dict = RcsTendonDuctData.from_dict(rcs_tendon_duct_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


