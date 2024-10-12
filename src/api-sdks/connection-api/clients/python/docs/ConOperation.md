# ConOperation


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**is_active** | **bool** |  | [optional] 
**is_imported** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_operation import ConOperation

# TODO update the JSON string below
json = "{}"
# create an instance of ConOperation from a JSON string
con_operation_instance = ConOperation.from_json(json)
# print the JSON string representation of the object
print(ConOperation.to_json())

# convert the object into a dict
con_operation_dict = con_operation_instance.to_dict()
# create an instance of ConOperation from a dict
con_operation_from_dict = ConOperation.from_dict(con_operation_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


