# ConCssSegment

Polymorphic root. Every element on the wire is one of the concrete subtypes listed in the discriminator mapping and carries the $type discriminator; $type is deliberately declared on each subtype schema (with its exact wire value as default) rather than here.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**segment_type** | **str** |  | [optional] [readonly] 
**start** | [**ConCssPoint2D**](ConCssPoint2D.md) |  | [optional] 
**end** | [**ConCssPoint2D**](ConCssPoint2D.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_css_segment import ConCssSegment

# TODO update the JSON string below
json = "{}"
# create an instance of ConCssSegment from a JSON string
con_css_segment_instance = ConCssSegment.from_json(json)
# print the JSON string representation of the object
print(con_css_segment_instance.to_json())

# convert the object into a dict
con_css_segment_dict = con_css_segment_instance.to_dict()
# create an instance of ConCssSegment from a dict
con_css_segment_from_dict = ConCssSegment.from_dict(con_css_segment_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


