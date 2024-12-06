# ConcreteCheckResultOverall

Concrete check result

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**checks** | [**List[ConcreteCheckResultOverallItem]**](ConcreteCheckResultOverallItem.md) | All check by the type | [optional] 

## Example

```python
from ideastatica_rcs_api.models.concrete_check_result_overall import ConcreteCheckResultOverall

# TODO update the JSON string below
json = "{}"
# create an instance of ConcreteCheckResultOverall from a JSON string
concrete_check_result_overall_instance = ConcreteCheckResultOverall.from_json(json)
# print the JSON string representation of the object
print(ConcreteCheckResultOverall.to_json())

# convert the object into a dict
concrete_check_result_overall_dict = concrete_check_result_overall_instance.to_dict()
# create an instance of ConcreteCheckResultOverall from a dict
concrete_check_result_overall_from_dict = ConcreteCheckResultOverall.from_dict(concrete_check_result_overall_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


