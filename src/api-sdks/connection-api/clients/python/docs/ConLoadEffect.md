# ConLoadEffect


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**is_percentage** | **bool** |  | [optional] 
**member_loadings** | [**List[ConLoadEffectMemberLoad]**](ConLoadEffectMemberLoad.md) |  | [optional] 
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**active** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_load_effect import ConLoadEffect

# TODO update the JSON string below
json = "{}"
# create an instance of ConLoadEffect from a JSON string
con_load_effect_instance = ConLoadEffect.from_json(json)
# print the JSON string representation of the object
print(ConLoadEffect.to_json())

# convert the object into a dict
con_load_effect_dict = con_load_effect_instance.to_dict()
# create an instance of ConLoadEffect from a dict
con_load_effect_from_dict = ConLoadEffect.from_dict(con_load_effect_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


