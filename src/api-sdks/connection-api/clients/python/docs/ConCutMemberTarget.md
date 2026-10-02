# ConCutMemberTarget


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**kind** | [**ConCutMemberKind**](ConCutMemberKind.md) |  | [optional] 
**member_id** | **int** |  | [optional] 
**operation_id** | **int** |  | [optional] 
**remaining_part** | [**ConRemainingPart**](ConRemainingPart.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_cut_member_target import ConCutMemberTarget

# TODO update the JSON string below
json = "{}"
# create an instance of ConCutMemberTarget from a JSON string
con_cut_member_target_instance = ConCutMemberTarget.from_json(json)
# print the JSON string representation of the object
print(con_cut_member_target_instance.to_json())

# convert the object into a dict
con_cut_member_target_dict = con_cut_member_target_instance.to_dict()
# create an instance of ConCutMemberTarget from a dict
con_cut_member_target_from_dict = ConCutMemberTarget.from_dict(con_cut_member_target_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


