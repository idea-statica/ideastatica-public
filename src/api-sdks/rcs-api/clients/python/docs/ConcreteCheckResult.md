# ConcreteCheckResult

Concrete Check results

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**result_type** | [**CheckResultType**](CheckResultType.md) |  | [optional] 
**check_results** | [**List[ConcreteCheckResultBase]**](ConcreteCheckResultBase.md) | All results - first is extreme | [optional] 

## Example

```python
from ideastatica_rcs_api.models.concrete_check_result import ConcreteCheckResult

# TODO update the JSON string below
json = "{}"
# create an instance of ConcreteCheckResult from a JSON string
concrete_check_result_instance = ConcreteCheckResult.from_json(json)
# print the JSON string representation of the object
print(ConcreteCheckResult.to_json())

# convert the object into a dict
concrete_check_result_dict = concrete_check_result_instance.to_dict()
# create an instance of ConcreteCheckResult from a dict
concrete_check_result_from_dict = ConcreteCheckResult.from_dict(concrete_check_result_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


