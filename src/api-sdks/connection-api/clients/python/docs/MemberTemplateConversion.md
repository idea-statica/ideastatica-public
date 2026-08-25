# MemberTemplateConversion


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**is_bearing** | **bool** |  | [optional] 
**original_member_name** | **str** |  | [optional] 
**new_member_name** | **str** |  | [optional] 
**type** | **str** |  | [optional] [default to 'IdeaStatiCa.Api.Connection.Model.MemberTemplateConversion, IdeaStatiCa.Api']

## Example

```python
from ideastatica_connection_api.models.member_template_conversion import MemberTemplateConversion

# TODO update the JSON string below
json = "{}"
# create an instance of MemberTemplateConversion from a JSON string
member_template_conversion_instance = MemberTemplateConversion.from_json(json)
# print the JSON string representation of the object
print(member_template_conversion_instance.to_json())

# convert the object into a dict
member_template_conversion_dict = member_template_conversion_instance.to_dict()
# create an instance of MemberTemplateConversion from a dict
member_template_conversion_from_dict = MemberTemplateConversion.from_dict(member_template_conversion_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


