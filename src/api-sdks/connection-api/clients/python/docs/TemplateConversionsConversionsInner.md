# TemplateConversionsConversionsInner

Polymorphic conversion root. Every element on the wire is one of the concrete subtypes listed in the discriminator mapping and carries the $type discriminator; $type is deliberately declared on each subtype schema (with its exact wire value as default) rather than here.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**original_value** | **str** |  | [optional] 
**original_template_id** | **str** |  | [optional] 
**new_value** | **str** |  | [optional] 
**description** | **str** |  | [optional] 
**new_template_id** | **str** |  | [optional] 
**type** | **str** |  | [optional] [default to 'IdeaStatiCa.Api.Connection.Model.PinTemplateConversion, IdeaStatiCa.Api']
**is_bearing** | **bool** |  | [optional] 
**original_member_name** | **str** |  | [optional] 
**new_member_name** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.template_conversions_conversions_inner import TemplateConversionsConversionsInner

# TODO update the JSON string below
json = "{}"
# create an instance of TemplateConversionsConversionsInner from a JSON string
template_conversions_conversions_inner_instance = TemplateConversionsConversionsInner.from_json(json)
# print the JSON string representation of the object
print(template_conversions_conversions_inner_instance.to_json())

# convert the object into a dict
template_conversions_conversions_inner_dict = template_conversions_conversions_inner_instance.to_dict()
# create an instance of TemplateConversionsConversionsInner from a dict
template_conversions_conversions_inner_from_dict = TemplateConversionsConversionsInner.from_dict(template_conversions_conversions_inner_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


