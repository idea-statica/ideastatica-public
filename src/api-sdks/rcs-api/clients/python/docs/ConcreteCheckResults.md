# ConcreteCheckResults

Concrete Check results

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**check_results** | [**List[ConcreteCheckResult]**](ConcreteCheckResult.md) | All results | [optional] 
**overall** | [**ConcreteCheckResultOverall**](ConcreteCheckResultOverall.md) |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.concrete_check_results import ConcreteCheckResults

# TODO update the JSON string below
json = "{}"
# create an instance of ConcreteCheckResults from a JSON string
concrete_check_results_instance = ConcreteCheckResults.from_json(json)
# print the JSON string representation of the object
print(ConcreteCheckResults.to_json())

# convert the object into a dict
concrete_check_results_dict = concrete_check_results_instance.to_dict()
# create an instance of ConcreteCheckResults from a dict
concrete_check_results_from_dict = ConcreteCheckResults.from_dict(concrete_check_results_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


