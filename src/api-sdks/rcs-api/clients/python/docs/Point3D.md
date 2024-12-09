# Point3D

Represents an x- , y- an z-coordinates in three-dimensional space.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**x** | **float** | Gets or sets the X-coordinate value | [optional] 
**y** | **float** | Gets or sets the Y-coordinate value | [optional] 
**z** | **float** | Gets or sets the Z-coordinate value | [optional] 
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.point3_d import Point3D

# TODO update the JSON string below
json = "{}"
# create an instance of Point3D from a JSON string
point3_d_instance = Point3D.from_json(json)
# print the JSON string representation of the object
print(Point3D.to_json())

# convert the object into a dict
point3_d_dict = point3_d_instance.to_dict()
# create an instance of Point3D from a dict
point3_d_from_dict = Point3D.from_dict(point3_d_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


