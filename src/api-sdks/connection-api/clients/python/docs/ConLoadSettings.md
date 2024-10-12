# ConLoadSettings


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**loads_in_equilibrium** | **bool** |  | [optional] 
**loads_in_percentage** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_load_settings import ConLoadSettings

# TODO update the JSON string below
json = "{}"
# create an instance of ConLoadSettings from a JSON string
con_load_settings_instance = ConLoadSettings.from_json(json)
# print the JSON string representation of the object
print(ConLoadSettings.to_json())

# convert the object into a dict
con_load_settings_dict = con_load_settings_instance.to_dict()
# create an instance of ConLoadSettings from a dict
con_load_settings_from_dict = ConLoadSettings.from_dict(con_load_settings_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


