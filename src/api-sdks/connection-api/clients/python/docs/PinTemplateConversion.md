# PinTemplateConversion


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**type** | **str** |  | [optional] [default to 'IdeaStatiCa.Api.Connection.Model.PinTemplateConversion, IdeaStatiCa.Api']

## Example

```python
from ideastatica_connection_api.models.pin_template_conversion import PinTemplateConversion

# TODO update the JSON string below
json = "{}"
# create an instance of PinTemplateConversion from a JSON string
pin_template_conversion_instance = PinTemplateConversion.from_json(json)
# print the JSON string representation of the object
print(pin_template_conversion_instance.to_json())

# convert the object into a dict
pin_template_conversion_dict = pin_template_conversion_instance.to_dict()
# create an instance of PinTemplateConversion from a dict
pin_template_conversion_from_dict = PinTemplateConversion.from_dict(pin_template_conversion_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


