# BaseTemplateConversion


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**original_value** | **str** |  | [optional] 
**original_template_id** | **str** |  | [optional] 
**new_value** | **str** |  | [optional] 
**description** | **str** |  | [optional] 
**new_template_id** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.base_template_conversion import BaseTemplateConversion

# TODO update the JSON string below
json = "{}"
# create an instance of BaseTemplateConversion from a JSON string
base_template_conversion_instance = BaseTemplateConversion.from_json(json)
# print the JSON string representation of the object
print(BaseTemplateConversion.to_json())

# convert the object into a dict
base_template_conversion_dict = base_template_conversion_instance.to_dict()
# create an instance of BaseTemplateConversion from a dict
base_template_conversion_from_dict = BaseTemplateConversion.from_dict(base_template_conversion_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


