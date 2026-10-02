# ConCssChoiceDimension


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**value** | **str** |  | [optional] 
**options** | [**List[ConCssOption]**](ConCssOption.md) |  | [optional] 
**type** | **str** |  | [default to 'IdeaStatiCa.Api.Connection.Model.Material.ConCssChoiceDimension, IdeaStatiCa.Api']

## Example

```python
from ideastatica_connection_api.models.con_css_choice_dimension import ConCssChoiceDimension

# TODO update the JSON string below
json = "{}"
# create an instance of ConCssChoiceDimension from a JSON string
con_css_choice_dimension_instance = ConCssChoiceDimension.from_json(json)
# print the JSON string representation of the object
print(con_css_choice_dimension_instance.to_json())

# convert the object into a dict
con_css_choice_dimension_dict = con_css_choice_dimension_instance.to_dict()
# create an instance of ConCssChoiceDimension from a dict
con_css_choice_dimension_from_dict = ConCssChoiceDimension.from_dict(con_css_choice_dimension_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


