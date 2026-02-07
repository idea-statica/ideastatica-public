# Stirrup


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**geometry** | [**PolyLine2D**](PolyLine2D.md) |  | [optional] 
**diameter** | **float** |  | [optional] 
**material** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**anchorage_lenght** | **float** |  | [optional] 
**diameter_of_mandrel** | **float** |  | [optional] 
**is_closed** | **bool** |  | [optional] 
**distance** | **float** |  | [optional] 
**shear_check** | **bool** |  | [optional] 
**torsion_check** | **bool** |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.stirrup import Stirrup

# TODO update the JSON string below
json = "{}"
# create an instance of Stirrup from a JSON string
stirrup_instance = Stirrup.from_json(json)
# print the JSON string representation of the object
print(stirrup_instance.to_json())

# convert the object into a dict
stirrup_dict = stirrup_instance.to_dict()
# create an instance of Stirrup from a dict
stirrup_from_dict = Stirrup.from_dict(stirrup_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


