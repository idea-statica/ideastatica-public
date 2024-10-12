# ConLoadEffectMemberLoad


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**member_id** | **int** |  | [optional] 
**position** | [**ConLoadEffectPositionEnum**](ConLoadEffectPositionEnum.md) |  | [optional] 
**section_load** | [**ConLoadEffectSectionLoad**](ConLoadEffectSectionLoad.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_load_effect_member_load import ConLoadEffectMemberLoad

# TODO update the JSON string below
json = "{}"
# create an instance of ConLoadEffectMemberLoad from a JSON string
con_load_effect_member_load_instance = ConLoadEffectMemberLoad.from_json(json)
# print the JSON string representation of the object
print(ConLoadEffectMemberLoad.to_json())

# convert the object into a dict
con_load_effect_member_load_dict = con_load_effect_member_load_instance.to_dict()
# create an instance of ConLoadEffectMemberLoad from a dict
con_load_effect_member_load_from_dict = ConLoadEffectMemberLoad.from_dict(con_load_effect_member_load_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


