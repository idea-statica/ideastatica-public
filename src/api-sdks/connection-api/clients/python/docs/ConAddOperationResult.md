# ConAddOperationResult


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**operation** | [**ConOperation**](ConOperation.md) |  | [optional] 
**issues** | [**List[ConNonConformityIssue]**](ConNonConformityIssue.md) |  | [optional] 
**added_without_issues** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_add_operation_result import ConAddOperationResult

# TODO update the JSON string below
json = "{}"
# create an instance of ConAddOperationResult from a JSON string
con_add_operation_result_instance = ConAddOperationResult.from_json(json)
# print the JSON string representation of the object
print(con_add_operation_result_instance.to_json())

# convert the object into a dict
con_add_operation_result_dict = con_add_operation_result_instance.to_dict()
# create an instance of ConAddOperationResult from a dict
con_add_operation_result_from_dict = ConAddOperationResult.from_dict(con_add_operation_result_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


