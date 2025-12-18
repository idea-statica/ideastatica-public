# RcsStirrupsData


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**diameter** | **float** |  | [optional] 
**material** | [**RcsMaterial**](RcsMaterial.md) |  | [optional] 
**diameter_of_mandrel** | **float** |  | [optional] 
**is_closed** | **bool** |  | [optional] 
**shear_check** | **bool** |  | [optional] 
**torsion_check** | **bool** |  | [optional] 
**distance** | **float** |  | [optional] 
**geometry** | [**PolyLine2D**](PolyLine2D.md) |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_stirrups_data import RcsStirrupsData

# TODO update the JSON string below
json = "{}"
# create an instance of RcsStirrupsData from a JSON string
rcs_stirrups_data_instance = RcsStirrupsData.from_json(json)
# print the JSON string representation of the object
print(rcs_stirrups_data_instance.to_json())

# convert the object into a dict
rcs_stirrups_data_dict = rcs_stirrups_data_instance.to_dict()
# create an instance of RcsStirrupsData from a dict
rcs_stirrups_data_from_dict = RcsStirrupsData.from_dict(rcs_stirrups_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


