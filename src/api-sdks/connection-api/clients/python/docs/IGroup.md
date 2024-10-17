# IGroup


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**faces** | **List[int]** |  | [optional] 
**selected** | [**Selected**](Selected.md) |  | [optional] 
**lines** | [**List[Line]**](Line.md) |  | [optional] 
**priority** | **int** |  | [optional] 
**text** | [**List[Text]**](Text.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.i_group import IGroup

# TODO update the JSON string below
json = "{}"
# create an instance of IGroup from a JSON string
i_group_instance = IGroup.from_json(json)
# print the JSON string representation of the object
print(IGroup.to_json())

# convert the object into a dict
i_group_dict = i_group_instance.to_dict()
# create an instance of IGroup from a dict
i_group_from_dict = IGroup.from_dict(i_group_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


