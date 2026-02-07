# NonConformityIssue


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**guid** | **str** |  | [optional] 
**description** | **str** |  | [optional] 
**severity** | [**NonConformitySeverity**](NonConformitySeverity.md) |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.non_conformity_issue import NonConformityIssue

# TODO update the JSON string below
json = "{}"
# create an instance of NonConformityIssue from a JSON string
non_conformity_issue_instance = NonConformityIssue.from_json(json)
# print the JSON string representation of the object
print(non_conformity_issue_instance.to_json())

# convert the object into a dict
non_conformity_issue_dict = non_conformity_issue_instance.to_dict()
# create an instance of NonConformityIssue from a dict
non_conformity_issue_from_dict = NonConformityIssue.from_dict(non_conformity_issue_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


