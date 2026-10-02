# ConCutByTarget


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**kind** | [**ConCutByKind**](ConCutByKind.md) |  | [optional] 
**member_id** | **int** |  | [optional] 
**plate_id** | **int** |  | [optional] 
**operation_id** | **int** |  | [optional] 
**plate_sub_index** | **int** |  | [optional] 
**remaining_part** | [**ConRemainingPart**](ConRemainingPart.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_cut_by_target import ConCutByTarget

# TODO update the JSON string below
json = "{}"
# create an instance of ConCutByTarget from a JSON string
con_cut_by_target_instance = ConCutByTarget.from_json(json)
# print the JSON string representation of the object
print(con_cut_by_target_instance.to_json())

# convert the object into a dict
con_cut_by_target_dict = con_cut_by_target_instance.to_dict()
# create an instance of ConCutByTarget from a dict
con_cut_by_target_from_dict = ConCutByTarget.from_dict(con_cut_by_target_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


