# ConConversionSettings


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**target_design_code** | [**CountryCode**](CountryCode.md) |  | [optional] 
**concrete** | [**List[ConversionMapping]**](ConversionMapping.md) |  | [optional] 
**cross_sections** | [**List[ConversionMapping]**](ConversionMapping.md) |  | [optional] 
**fasteners** | [**List[ConversionMapping]**](ConversionMapping.md) |  | [optional] 
**steel** | [**List[ConversionMapping]**](ConversionMapping.md) |  | [optional] 
**welds** | [**List[ConversionMapping]**](ConversionMapping.md) |  | [optional] 
**bolt_grade** | [**List[ConversionMapping]**](ConversionMapping.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_conversion_settings import ConConversionSettings

# TODO update the JSON string below
json = "{}"
# create an instance of ConConversionSettings from a JSON string
con_conversion_settings_instance = ConConversionSettings.from_json(json)
# print the JSON string representation of the object
print(ConConversionSettings.to_json())

# convert the object into a dict
con_conversion_settings_dict = con_conversion_settings_instance.to_dict()
# create an instance of ConConversionSettings from a dict
con_conversion_settings_from_dict = ConConversionSettings.from_dict(con_conversion_settings_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


