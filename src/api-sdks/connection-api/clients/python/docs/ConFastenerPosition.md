# ConFastenerPosition


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**x** | **float** |  | [optional] 
**y** | **float** |  | [optional] 
**grid_index** | **int** |  | [optional] 
**slotted_holes** | [**List[ConSlottedHole]**](ConSlottedHole.md) |  | [optional] 
**hook_rotation** | **float** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_fastener_position import ConFastenerPosition

# TODO update the JSON string below
json = "{}"
# create an instance of ConFastenerPosition from a JSON string
con_fastener_position_instance = ConFastenerPosition.from_json(json)
# print the JSON string representation of the object
print(con_fastener_position_instance.to_json())

# convert the object into a dict
con_fastener_position_dict = con_fastener_position_instance.to_dict()
# create an instance of ConFastenerPosition from a dict
con_fastener_position_from_dict = ConFastenerPosition.from_dict(con_fastener_position_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


