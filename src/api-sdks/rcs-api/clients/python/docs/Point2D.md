# Point2D

Represents an x- and y-coordinate pair in two-dimensional space.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**x** | **float** | Gets or sets the X-coordinate value | [optional] 
**y** | **float** | Gets or sets the Y-coordinate value | [optional] 

## Example

```python
from ideastatica_rcs_api.models.point2_d import Point2D

# TODO update the JSON string below
json = "{}"
# create an instance of Point2D from a JSON string
point2_d_instance = Point2D.from_json(json)
# print the JSON string representation of the object
print(Point2D.to_json())

# convert the object into a dict
point2_d_dict = point2_d_instance.to_dict()
# create an instance of Point2D from a dict
point2_d_from_dict = Point2D.from_dict(point2_d_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


