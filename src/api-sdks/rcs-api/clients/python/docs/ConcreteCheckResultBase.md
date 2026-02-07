# ConcreteCheckResultBase


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**internal_fores** | [**ResultOfInternalForces**](ResultOfInternalForces.md) |  | [optional] 
**non_conformities** | [**List[NonConformity]**](NonConformity.md) |  | [optional] 
**result** | [**CheckResult**](CheckResult.md) |  | [optional] 
**check_value** | **float** |  | [optional] 
**limit_check_value** | **float** |  | [optional] 
**check** | [**CalculationType**](CalculationType.md) |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.concrete_check_result_base import ConcreteCheckResultBase

# TODO update the JSON string below
json = "{}"
# create an instance of ConcreteCheckResultBase from a JSON string
concrete_check_result_base_instance = ConcreteCheckResultBase.from_json(json)
# print the JSON string representation of the object
print(concrete_check_result_base_instance.to_json())

# convert the object into a dict
concrete_check_result_base_dict = concrete_check_result_base_instance.to_dict()
# create an instance of ConcreteCheckResultBase from a dict
concrete_check_result_base_from_dict = ConcreteCheckResultBase.from_dict(concrete_check_result_base_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


