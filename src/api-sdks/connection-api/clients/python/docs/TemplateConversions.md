# TemplateConversions


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**conversions** | [**List[BaseTemplateConversion]**](BaseTemplateConversion.md) |  | [optional] 
**country_code** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.template_conversions import TemplateConversions

# TODO update the JSON string below
json = "{}"
# create an instance of TemplateConversions from a JSON string
template_conversions_instance = TemplateConversions.from_json(json)
# print the JSON string representation of the object
print(TemplateConversions.to_json())

# convert the object into a dict
template_conversions_dict = template_conversions_instance.to_dict()
# create an instance of TemplateConversions from a dict
template_conversions_from_dict = TemplateConversions.from_dict(template_conversions_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


