# RcsSetting


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**type** | **str** |  | [optional] 
**value** | **object** |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_setting import RcsSetting

# TODO update the JSON string below
json = "{}"
# create an instance of RcsSetting from a JSON string
rcs_setting_instance = RcsSetting.from_json(json)
# print the JSON string representation of the object
print(RcsSetting.to_json())

# convert the object into a dict
rcs_setting_dict = rcs_setting_instance.to_dict()
# create an instance of RcsSetting from a dict
rcs_setting_from_dict = RcsSetting.from_dict(rcs_setting_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


