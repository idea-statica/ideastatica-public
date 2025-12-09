# ConConnectionTemplateModel


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**plate_material_id** | **int** |  | [optional] 
**weld_material_id** | **int** |  | [optional] 
**bolt_assembly_id** | **int** |  | [optional] 
**tempalte_id** | **str** |  | [optional] 
**instance_id** | **int** |  | [optional] 
**member_ids** | **List[int]** |  | [optional] 
**operation_ids** | **List[int]** |  | [optional] 
**parameter_ids** | **List[int]** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_connection_template_model import ConConnectionTemplateModel

# TODO update the JSON string below
json = "{}"
# create an instance of ConConnectionTemplateModel from a JSON string
con_connection_template_model_instance = ConConnectionTemplateModel.from_json(json)
# print the JSON string representation of the object
print(con_connection_template_model_instance.to_json())

# convert the object into a dict
con_connection_template_model_dict = con_connection_template_model_instance.to_dict()
# create an instance of ConConnectionTemplateModel from a dict
con_connection_template_model_from_dict = ConConnectionTemplateModel.from_dict(con_connection_template_model_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


