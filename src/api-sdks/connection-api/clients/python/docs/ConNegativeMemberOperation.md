# ConNegativeMemberOperation


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**cross_section_id** | **int** |  | [optional] 
**length1** | **float** |  | [optional] 
**length2** | **float** |  | [optional] 
**mirror_y** | **bool** |  | [optional] 
**mirror_z** | **bool** |  | [optional] 
**positioning** | [**ConAddedMemberPositioning**](ConAddedMemberPositioning.md) |  | [optional] 
**is_imported** | **bool** |  | [optional] 
**operation_type** | **str** |  | [optional] 
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**active** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_negative_member_operation import ConNegativeMemberOperation

# TODO update the JSON string below
json = "{}"
# create an instance of ConNegativeMemberOperation from a JSON string
con_negative_member_operation_instance = ConNegativeMemberOperation.from_json(json)
# print the JSON string representation of the object
print(con_negative_member_operation_instance.to_json())

# convert the object into a dict
con_negative_member_operation_dict = con_negative_member_operation_instance.to_dict()
# create an instance of ConNegativeMemberOperation from a dict
con_negative_member_operation_from_dict = ConNegativeMemberOperation.from_dict(con_negative_member_operation_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


