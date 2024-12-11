# ResultOfLoading

Result Of Loading

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**items** | [**List[ResultOfLoadingItem]**](ResultOfLoadingItem.md) | Items od loading | [optional] 
**loading_type** | [**LoadingType**](LoadingType.md) |  | [optional] 
**id** | **int** | Id of loading | [optional] 

## Example

```python
from ideastatica_rcs_api.models.result_of_loading import ResultOfLoading

# TODO update the JSON string below
json = "{}"
# create an instance of ResultOfLoading from a JSON string
result_of_loading_instance = ResultOfLoading.from_json(json)
# print the JSON string representation of the object
print(ResultOfLoading.to_json())

# convert the object into a dict
result_of_loading_dict = result_of_loading_instance.to_dict()
# create an instance of ResultOfLoading from a dict
result_of_loading_from_dict = ResultOfLoading.from_dict(result_of_loading_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


