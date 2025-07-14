# ConMember


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**is_continuous** | **bool** |  | [optional] 
**cross_section_id** | **int** |  | [optional] 
**mirror_y** | **bool** |  | [optional] 
**mirror_z** | **bool** |  | [optional] 
**is_bearing** | **bool** |  | [optional] 
**position** | [**ConMemberPosition**](ConMemberPosition.md) |  | [optional] 
**model** | [**ConMemberModel**](ConMemberModel.md) |  | [optional] 
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**active** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_member import ConMember

# TODO update the JSON string below
json = "{}"
# create an instance of ConMember from a JSON string
con_member_instance = ConMember.from_json(json)
# print the JSON string representation of the object
print(ConMember.to_json())

# convert the object into a dict
con_member_dict = con_member_instance.to_dict()
# create an instance of ConMember from a dict
con_member_from_dict = ConMember.from_dict(con_member_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


