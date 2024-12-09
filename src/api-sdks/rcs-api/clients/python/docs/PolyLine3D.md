# PolyLine3D

Represents a polyline in three-dimensional space.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.poly_line3_d import PolyLine3D

# TODO update the JSON string below
json = "{}"
# create an instance of PolyLine3D from a JSON string
poly_line3_d_instance = PolyLine3D.from_json(json)
# print the JSON string representation of the object
print(PolyLine3D.to_json())

# convert the object into a dict
poly_line3_d_dict = poly_line3_d_instance.to_dict()
# create an instance of PolyLine3D from a dict
poly_line3_d_from_dict = PolyLine3D.from_dict(poly_line3_d_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


