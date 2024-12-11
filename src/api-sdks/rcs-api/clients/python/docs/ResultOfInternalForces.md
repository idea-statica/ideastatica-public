# ResultOfInternalForces

Result of internal forces in the one position

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**n** | **float** | Normal force | [optional] 
**qy** | **float** | Shear force in y dirrection | [optional] 
**qz** | **float** | Shear force in z dirrection | [optional] 
**mx** | **float** | Bending moment around x-axis | [optional] 
**my** | **float** | Bending moment around y-axis | [optional] 
**mz** | **float** | Bending moment around z-axis | [optional] 
**loading** | [**ResultOfLoading**](ResultOfLoading.md) |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.result_of_internal_forces import ResultOfInternalForces

# TODO update the JSON string below
json = "{}"
# create an instance of ResultOfInternalForces from a JSON string
result_of_internal_forces_instance = ResultOfInternalForces.from_json(json)
# print the JSON string representation of the object
print(ResultOfInternalForces.to_json())

# convert the object into a dict
result_of_internal_forces_dict = result_of_internal_forces_instance.to_dict()
# create an instance of ResultOfInternalForces from a dict
result_of_internal_forces_from_dict = ResultOfInternalForces.from_dict(result_of_internal_forces_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


