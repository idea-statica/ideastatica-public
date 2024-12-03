# TendonBar

Tendon bar

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Tendon Id | [optional] 
**tendon_type** | [**TendonBarType**](TendonBarType.md) |  | [optional] 
**point** | [**Point2D**](Point2D.md) |  | [optional] 
**material** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**prestressing_order** | **int** | order of tendon prestessing | [optional] 
**num_strands_in_tendon** | **int** | number of ropes in tendon | [optional] 
**prestress_reinforcement_type** | [**FatigueTypeOfPrestressingSteel**](FatigueTypeOfPrestressingSteel.md) |  | [optional] 
**phase** | **int** | Phase | [optional] 
**tendon_duct** | [**TendonDuct**](TendonDuct.md) |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.tendon_bar import TendonBar

# TODO update the JSON string below
json = "{}"
# create an instance of TendonBar from a JSON string
tendon_bar_instance = TendonBar.from_json(json)
# print the JSON string representation of the object
print(TendonBar.to_json())

# convert the object into a dict
tendon_bar_dict = tendon_bar_instance.to_dict()
# create an instance of TendonBar from a dict
tendon_bar_from_dict = TendonBar.from_dict(tendon_bar_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


