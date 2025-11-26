# ConTemplateApplyResult


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**applied_without_issues** | **bool** |  | [optional] 
**template_model** | [**ConConnectionTemplateModel**](ConConnectionTemplateModel.md) |  | [optional] 
**issues** | [**List[ConNonConformityIssue]**](ConNonConformityIssue.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_template_apply_result import ConTemplateApplyResult

# TODO update the JSON string below
json = "{}"
# create an instance of ConTemplateApplyResult from a JSON string
con_template_apply_result_instance = ConTemplateApplyResult.from_json(json)
# print the JSON string representation of the object
print(con_template_apply_result_instance.to_json())

# convert the object into a dict
con_template_apply_result_dict = con_template_apply_result_instance.to_dict()
# create an instance of ConTemplateApplyResult from a dict
con_template_apply_result_from_dict = ConTemplateApplyResult.from_dict(con_template_apply_result_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


