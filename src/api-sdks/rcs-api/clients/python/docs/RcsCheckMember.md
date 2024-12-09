# RcsCheckMember


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_check_member import RcsCheckMember

# TODO update the JSON string below
json = "{}"
# create an instance of RcsCheckMember from a JSON string
rcs_check_member_instance = RcsCheckMember.from_json(json)
# print the JSON string representation of the object
print(RcsCheckMember.to_json())

# convert the object into a dict
rcs_check_member_dict = rcs_check_member_instance.to_dict()
# create an instance of RcsCheckMember from a dict
rcs_check_member_from_dict = RcsCheckMember.from_dict(rcs_check_member_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


