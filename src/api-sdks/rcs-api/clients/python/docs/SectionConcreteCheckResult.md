# SectionConcreteCheckResult

ection concrete result

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**section_id** | **int** | Id Of section | [optional] 
**extreme_results** | [**List[ConcreteCheckResults]**](ConcreteCheckResults.md) | Extreme results | [optional] 

## Example

```python
from ideastatica_rcs_api.models.section_concrete_check_result import SectionConcreteCheckResult

# TODO update the JSON string below
json = "{}"
# create an instance of SectionConcreteCheckResult from a JSON string
section_concrete_check_result_instance = SectionConcreteCheckResult.from_json(json)
# print the JSON string representation of the object
print(SectionConcreteCheckResult.to_json())

# convert the object into a dict
section_concrete_check_result_dict = section_concrete_check_result_instance.to_dict()
# create an instance of SectionConcreteCheckResult from a dict
section_concrete_check_result_from_dict = SectionConcreteCheckResult.from_dict(section_concrete_check_result_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


