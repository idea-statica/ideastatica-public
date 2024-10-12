# LoadEffectData


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**description** | **str** |  | [optional] 
**is_active** | **bool** |  | [optional] 
**internal_forces** | **List[int]** |  | [optional] 
**purpose** | [**EPurpose**](EPurpose.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.load_effect_data import LoadEffectData

# TODO update the JSON string below
json = "{}"
# create an instance of LoadEffectData from a JSON string
load_effect_data_instance = LoadEffectData.from_json(json)
# print the JSON string representation of the object
print(LoadEffectData.to_json())

# convert the object into a dict
load_effect_data_dict = load_effect_data_instance.to_dict()
# create an instance of LoadEffectData from a dict
load_effect_data_from_dict = LoadEffectData.from_dict(load_effect_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


