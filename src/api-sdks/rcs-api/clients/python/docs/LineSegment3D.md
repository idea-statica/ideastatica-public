# LineSegment3D

Represents a line segment in three-dimensional space.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.line_segment3_d import LineSegment3D

# TODO update the JSON string below
json = "{}"
# create an instance of LineSegment3D from a JSON string
line_segment3_d_instance = LineSegment3D.from_json(json)
# print the JSON string representation of the object
print(LineSegment3D.to_json())

# convert the object into a dict
line_segment3_d_dict = line_segment3_d_instance.to_dict()
# create an instance of LineSegment3D from a dict
line_segment3_d_from_dict = LineSegment3D.from_dict(line_segment3_d_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


