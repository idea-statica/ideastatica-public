# ArcSegment3D

Represents an arc segment in three-dimensional space.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.arc_segment3_d import ArcSegment3D

# TODO update the JSON string below
json = "{}"
# create an instance of ArcSegment3D from a JSON string
arc_segment3_d_instance = ArcSegment3D.from_json(json)
# print the JSON string representation of the object
print(ArcSegment3D.to_json())

# convert the object into a dict
arc_segment3_d_dict = arc_segment3_d_instance.to_dict()
# create an instance of ArcSegment3D from a dict
arc_segment3_d_from_dict = ArcSegment3D.from_dict(arc_segment3_d_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


