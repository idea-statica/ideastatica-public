# Stirrup

Stirrup

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**geometry** | [**PolyLine2D**](PolyLine2D.md) |  | [optional] 
**diameter** | **float** | Diameter | [optional] 
**material** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**anchorage_lenght** | **float** | Anchorage Lenght | [optional] 
**diameter_of_mandrel** | **float** | Radius of stirrup mandrel - refering to stirrup axis | [optional] 
**is_closed** | **bool** | Open / Closed stirrup | [optional] 
**distance** | **float** | Longitudinal distance between stirrups | [optional] 
**shear_check** | **bool** | Status of shear check, not possible for detailing stirrup | [optional] 
**torsion_check** | **bool** | Status of torsion check, not possible for detailing stirrup | [optional] 

## Example

```python
from ideastatica_rcs_api.models.stirrup import Stirrup

# TODO update the JSON string below
json = "{}"
# create an instance of Stirrup from a JSON string
stirrup_instance = Stirrup.from_json(json)
# print the JSON string representation of the object
print(Stirrup.to_json())

# convert the object into a dict
stirrup_dict = stirrup_instance.to_dict()
# create an instance of Stirrup from a dict
stirrup_from_dict = Stirrup.from_dict(stirrup_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


