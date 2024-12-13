# Polygon2D

Represents a polygon in two-dimensional space.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**points** | [**List[Point2D]**](Point2D.md) | List of polygon points | [optional] 

## Example

```python
from ideastatica_rcs_api.models.polygon2_d import Polygon2D

# TODO update the JSON string below
json = "{}"
# create an instance of Polygon2D from a JSON string
polygon2_d_instance = Polygon2D.from_json(json)
# print the JSON string representation of the object
print(Polygon2D.to_json())

# convert the object into a dict
polygon2_d_dict = polygon2_d_instance.to_dict()
# create an instance of Polygon2D from a dict
polygon2_d_from_dict = Polygon2D.from_dict(polygon2_d_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


