# LoadCase

Load case in the model

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.load_case import LoadCase

# TODO update the JSON string below
json = "{}"
# create an instance of LoadCase from a JSON string
load_case_instance = LoadCase.from_json(json)
# print the JSON string representation of the object
print(LoadCase.to_json())

# convert the object into a dict
load_case_dict = load_case_instance.to_dict()
# create an instance of LoadCase from a dict
load_case_from_dict = LoadCase.from_dict(load_case_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


