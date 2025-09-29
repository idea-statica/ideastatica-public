# ConTemplatePublishParam


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** |  | [optional] 
**author** | **str** |  | [optional] 
**company_name** | **str** |  | [optional] 
**design_set_type** | [**ConDesignSetType**](ConDesignSetType.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_template_publish_param import ConTemplatePublishParam

# TODO update the JSON string below
json = "{}"
# create an instance of ConTemplatePublishParam from a JSON string
con_template_publish_param_instance = ConTemplatePublishParam.from_json(json)
# print the JSON string representation of the object
print(ConTemplatePublishParam.to_json())

# convert the object into a dict
con_template_publish_param_dict = con_template_publish_param_instance.to_dict()
# create an instance of ConTemplatePublishParam from a dict
con_template_publish_param_from_dict = ConTemplatePublishParam.from_dict(con_template_publish_param_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


