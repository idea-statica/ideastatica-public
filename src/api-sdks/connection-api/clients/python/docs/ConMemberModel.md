# ConMemberModel


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**model_type** | **str** |  | [optional] 
**forces_in** | [**ConMemberForcesInEnum**](ConMemberForcesInEnum.md) |  | [optional] 
**x** | **float** |  | [optional] 
**connected_member_id** | **int** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_member_model import ConMemberModel

# TODO update the JSON string below
json = "{}"
# create an instance of ConMemberModel from a JSON string
con_member_model_instance = ConMemberModel.from_json(json)
# print the JSON string representation of the object
print(ConMemberModel.to_json())

# convert the object into a dict
con_member_model_dict = con_member_model_instance.to_dict()
# create an instance of ConMemberModel from a dict
con_member_model_from_dict = ConMemberModel.from_dict(con_member_model_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


