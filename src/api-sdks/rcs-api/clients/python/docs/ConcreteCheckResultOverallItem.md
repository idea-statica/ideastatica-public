# ConcreteCheckResultOverallItem

Overal check item

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**result_type** | [**CheckResultType**](CheckResultType.md) |  | [optional] 
**result** | [**CheckResult**](CheckResult.md) |  | [optional] 
**check_value** | **float** | calculated limited value, calculated as strain to limit strain | [optional] 

## Example

```python
from ideastatica_rcs_api.models.concrete_check_result_overall_item import ConcreteCheckResultOverallItem

# TODO update the JSON string below
json = "{}"
# create an instance of ConcreteCheckResultOverallItem from a JSON string
concrete_check_result_overall_item_instance = ConcreteCheckResultOverallItem.from_json(json)
# print the JSON string representation of the object
print(ConcreteCheckResultOverallItem.to_json())

# convert the object into a dict
concrete_check_result_overall_item_dict = concrete_check_result_overall_item_instance.to_dict()
# create an instance of ConcreteCheckResultOverallItem from a dict
concrete_check_result_overall_item_from_dict = ConcreteCheckResultOverallItem.from_dict(concrete_check_result_overall_item_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


