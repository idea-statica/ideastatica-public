# ConStiffeningMemberOperation


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**cross_section_id** | **int** |  | [optional] 
**material_id** | **int** |  | [optional] 
**length1** | **float** |  | [optional] 
**length2** | **float** |  | [optional] 
**mirror_y** | **bool** |  | [optional] 
**mirror_z** | **bool** |  | [optional] 
**positioning** | [**ConAddedMemberPositioning**](ConAddedMemberPositioning.md) |  | [optional] 
**weld** | [**ConWeldData**](ConWeldData.md) |  | [optional] 
**is_imported** | **bool** |  | [optional] 
**operation_type** | **str** |  | [optional] 
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**active** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_stiffening_member_operation import ConStiffeningMemberOperation

# TODO update the JSON string below
json = "{}"
# create an instance of ConStiffeningMemberOperation from a JSON string
con_stiffening_member_operation_instance = ConStiffeningMemberOperation.from_json(json)
# print the JSON string representation of the object
print(con_stiffening_member_operation_instance.to_json())

# convert the object into a dict
con_stiffening_member_operation_dict = con_stiffening_member_operation_instance.to_dict()
# create an instance of ConStiffeningMemberOperation from a dict
con_stiffening_member_operation_from_dict = ConStiffeningMemberOperation.from_dict(con_stiffening_member_operation_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


