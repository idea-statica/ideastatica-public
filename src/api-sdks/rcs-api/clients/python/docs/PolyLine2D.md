# PolyLine2D

Represents a polyline in two-dimensional space.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**start_point** | [**Point2D**](Point2D.md) |  | [optional] 
**segments** | [**List[Segment2D]**](Segment2D.md) | Gets segments of &#x60;PolyLine2D&#x60;. | [optional] 

## Example

```python
from ideastatica_rcs_api.models.poly_line2_d import PolyLine2D

# TODO update the JSON string below
json = "{}"
# create an instance of PolyLine2D from a JSON string
poly_line2_d_instance = PolyLine2D.from_json(json)
# print the JSON string representation of the object
print(PolyLine2D.to_json())

# convert the object into a dict
poly_line2_d_dict = poly_line2_d_instance.to_dict()
# create an instance of PolyLine2D from a dict
poly_line2_d_from_dict = PolyLine2D.from_dict(poly_line2_d_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


