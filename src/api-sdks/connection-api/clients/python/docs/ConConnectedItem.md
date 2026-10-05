# ConConnectedItem


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**plate_id** | **int** |  | [optional] 
**operation_id** | **int** |  | [optional] 
**member_id** | **int** |  | [optional] 
**plate_sub_index** | **int** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_connected_item import ConConnectedItem

# TODO update the JSON string below
json = "{}"
# create an instance of ConConnectedItem from a JSON string
con_connected_item_instance = ConConnectedItem.from_json(json)
# print the JSON string representation of the object
print(con_connected_item_instance.to_json())

# convert the object into a dict
con_connected_item_dict = con_connected_item_instance.to_dict()
# create an instance of ConConnectedItem from a dict
con_connected_item_from_dict = ConConnectedItem.from_dict(con_connected_item_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


