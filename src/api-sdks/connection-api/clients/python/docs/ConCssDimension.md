# ConCssDimension

Polymorphic root. Every element on the wire is one of the concrete subtypes listed in the discriminator mapping and carries the $type discriminator; $type is deliberately declared on each subtype schema (with its exact wire value as default) rather than here.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_css_dimension import ConCssDimension

# TODO update the JSON string below
json = "{}"
# create an instance of ConCssDimension from a JSON string
con_css_dimension_instance = ConCssDimension.from_json(json)
# print the JSON string representation of the object
print(con_css_dimension_instance.to_json())

# convert the object into a dict
con_css_dimension_dict = con_css_dimension_instance.to_dict()
# create an instance of ConCssDimension from a dict
con_css_dimension_from_dict = ConCssDimension.from_dict(con_css_dimension_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


