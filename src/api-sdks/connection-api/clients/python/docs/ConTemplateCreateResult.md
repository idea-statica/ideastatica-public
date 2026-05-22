# ConTemplateCreateResult


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**template_id** | **str** |  | [optional] 
**name** | **str** |  | [optional] 
**design_code** | **str** |  | [optional] 
**version** | **int** |  | [optional] 
**manufacturing_type** | **str** |  | [optional] 
**typology_code** | **str** |  | [optional] 
**typology_code_v2** | **str** |  | [optional] 
**operation_count** | **int** |  | [optional] 
**parameter_count** | **int** |  | [optional] 
**param_model_link_count** | **int** |  | [optional] 
**template** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_template_create_result import ConTemplateCreateResult

# TODO update the JSON string below
json = "{}"
# create an instance of ConTemplateCreateResult from a JSON string
con_template_create_result_instance = ConTemplateCreateResult.from_json(json)
# print the JSON string representation of the object
print(con_template_create_result_instance.to_json())

# convert the object into a dict
con_template_create_result_dict = con_template_create_result_instance.to_dict()
# create an instance of ConTemplateCreateResult from a dict
con_template_create_result_from_dict = ConTemplateCreateResult.from_dict(con_template_create_result_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


