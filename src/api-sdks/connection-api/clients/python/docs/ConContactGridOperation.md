# ConContactGridOperation


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**connected_items** | [**List[ConConnectedItem]**](ConConnectedItem.md) |  | [optional] 
**foundation_block** | [**ConFoundationBlockDto**](ConFoundationBlockDto.md) |  | [optional] 
**block_type** | [**ConBlockType**](ConBlockType.md) |  | [optional] 
**existing_block_operation_id** | **int** |  | [optional] 
**plate_side** | [**ConPlateSide**](ConPlateSide.md) |  | [optional] 
**is_imported** | **bool** |  | [optional] 
**operation_type** | **str** |  | [optional] 
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**active** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_contact_grid_operation import ConContactGridOperation

# TODO update the JSON string below
json = "{}"
# create an instance of ConContactGridOperation from a JSON string
con_contact_grid_operation_instance = ConContactGridOperation.from_json(json)
# print the JSON string representation of the object
print(con_contact_grid_operation_instance.to_json())

# convert the object into a dict
con_contact_grid_operation_dict = con_contact_grid_operation_instance.to_dict()
# create an instance of ConContactGridOperation from a dict
con_contact_grid_operation_from_dict = ConContactGridOperation.from_dict(con_contact_grid_operation_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


