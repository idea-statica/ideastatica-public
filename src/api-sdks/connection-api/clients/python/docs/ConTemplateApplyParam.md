# ConTemplateApplyParam


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**connection_template** | **str** |  | [optional] 
**mapping** | [**TemplateConversions**](TemplateConversions.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_template_apply_param import ConTemplateApplyParam

# TODO update the JSON string below
json = "{}"
# create an instance of ConTemplateApplyParam from a JSON string
con_template_apply_param_instance = ConTemplateApplyParam.from_json(json)
# print the JSON string representation of the object
print(ConTemplateApplyParam.to_json())

# convert the object into a dict
con_template_apply_param_dict = con_template_apply_param_instance.to_dict()
# create an instance of ConTemplateApplyParam from a dict
con_template_apply_param_from_dict = ConTemplateApplyParam.from_dict(con_template_apply_param_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


