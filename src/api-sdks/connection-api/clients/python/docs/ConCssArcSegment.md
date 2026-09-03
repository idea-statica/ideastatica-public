# ConCssArcSegment


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**segment_type** | **str** |  | [optional] [readonly] 
**mid** | [**ConCssPoint2D**](ConCssPoint2D.md) |  | [optional] 
**type** | **str** |  | [optional] [default to 'IdeaStatiCa.Api.Connection.Model.Material.ConCssArcSegment, IdeaStatiCa.Api']

## Example

```python
from ideastatica_connection_api.models.con_css_arc_segment import ConCssArcSegment

# TODO update the JSON string below
json = "{}"
# create an instance of ConCssArcSegment from a JSON string
con_css_arc_segment_instance = ConCssArcSegment.from_json(json)
# print the JSON string representation of the object
print(con_css_arc_segment_instance.to_json())

# convert the object into a dict
con_css_arc_segment_dict = con_css_arc_segment_instance.to_dict()
# create an instance of ConCssArcSegment from a dict
con_css_arc_segment_from_dict = ConCssArcSegment.from_dict(con_css_arc_segment_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


