# TendonDuct


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**point** | [**Point2D**](Point2D.md) |  | [optional] 
**material_duct** | [**MaterialDuct**](MaterialDuct.md) |  | [optional] 
**is_debonding_tube** | **bool** |  | [optional] 
**diameter** | **float** |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.tendon_duct import TendonDuct

# TODO update the JSON string below
json = "{}"
# create an instance of TendonDuct from a JSON string
tendon_duct_instance = TendonDuct.from_json(json)
# print the JSON string representation of the object
print(tendon_duct_instance.to_json())

# convert the object into a dict
tendon_duct_dict = tendon_duct_instance.to_dict()
# create an instance of TendonDuct from a dict
tendon_duct_from_dict = TendonDuct.from_dict(tendon_duct_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


