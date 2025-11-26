# ConNonConformityIssue


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**operation_id** | **int** |  | [optional] 
**description** | **str** |  | [optional] 
**details** | **str** |  | [optional] 
**severity** | [**ConNonConformityIssueSeverity**](ConNonConformityIssueSeverity.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_non_conformity_issue import ConNonConformityIssue

# TODO update the JSON string below
json = "{}"
# create an instance of ConNonConformityIssue from a JSON string
con_non_conformity_issue_instance = ConNonConformityIssue.from_json(json)
# print the JSON string representation of the object
print(con_non_conformity_issue_instance.to_json())

# convert the object into a dict
con_non_conformity_issue_dict = con_non_conformity_issue_instance.to_dict()
# create an instance of ConNonConformityIssue from a dict
con_non_conformity_issue_from_dict = ConNonConformityIssue.from_dict(con_non_conformity_issue_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


