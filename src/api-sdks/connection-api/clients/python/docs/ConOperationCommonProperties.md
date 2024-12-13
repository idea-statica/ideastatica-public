# ConOperationCommonProperties


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**weld_material_id** | **int** |  | [optional] 
**plate_material_id** | **int** |  | [optional] 
**bolt_assembly_id** | **int** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_operation_common_properties import ConOperationCommonProperties

# TODO update the JSON string below
json = "{}"
# create an instance of ConOperationCommonProperties from a JSON string
con_operation_common_properties_instance = ConOperationCommonProperties.from_json(json)
# print the JSON string representation of the object
print(ConOperationCommonProperties.to_json())

# convert the object into a dict
con_operation_common_properties_dict = con_operation_common_properties_instance.to_dict()
# create an instance of ConOperationCommonProperties from a dict
con_operation_common_properties_from_dict = ConOperationCommonProperties.from_dict(con_operation_common_properties_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


