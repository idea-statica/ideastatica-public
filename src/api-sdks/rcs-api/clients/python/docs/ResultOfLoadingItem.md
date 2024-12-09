# ResultOfLoadingItem

Item of Result of Loading

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**loading** | [**Loading**](Loading.md) |  | [optional] 
**coefficient** | **float** | Coefficient of loading | [optional] 

## Example

```python
from ideastatica_rcs_api.models.result_of_loading_item import ResultOfLoadingItem

# TODO update the JSON string below
json = "{}"
# create an instance of ResultOfLoadingItem from a JSON string
result_of_loading_item_instance = ResultOfLoadingItem.from_json(json)
# print the JSON string representation of the object
print(ResultOfLoadingItem.to_json())

# convert the object into a dict
result_of_loading_item_dict = result_of_loading_item_instance.to_dict()
# create an instance of ResultOfLoadingItem from a dict
result_of_loading_item_from_dict = ResultOfLoadingItem.from_dict(result_of_loading_item_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


