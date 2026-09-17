# ConContactOperation


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**placement** | [**ConWeldPlacement**](ConWeldPlacement.md) |  | [optional] 
**first_member_or_plate** | [**ConConnectedItem**](ConConnectedItem.md) |  | [optional] 
**first_plate_edge_index** | **List[int]** |  | [optional] 
**second_member_or_plate** | [**ConConnectedItem**](ConConnectedItem.md) |  | [optional] 
**second_plate_edge_index** | **int** |  | [optional] 
**is_imported** | **bool** |  | [optional] 
**operation_type** | **str** |  | [optional] 
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**active** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_contact_operation import ConContactOperation

# TODO update the JSON string below
json = "{}"
# create an instance of ConContactOperation from a JSON string
con_contact_operation_instance = ConContactOperation.from_json(json)
# print the JSON string representation of the object
print(con_contact_operation_instance.to_json())

# convert the object into a dict
con_contact_operation_dict = con_contact_operation_instance.to_dict()
# create an instance of ConContactOperation from a dict
con_contact_operation_from_dict = ConContactOperation.from_dict(con_contact_operation_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


