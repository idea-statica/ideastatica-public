# Segment2D

Represents a segment in two-dimensional space.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**end_point** | [**Point2D**](Point2D.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.segment2_d import Segment2D

# TODO update the JSON string below
json = "{}"
# create an instance of Segment2D from a JSON string
segment2_d_instance = Segment2D.from_json(json)
# print the JSON string representation of the object
print(Segment2D.to_json())

# convert the object into a dict
segment2_d_dict = segment2_d_instance.to_dict()
# create an instance of Segment2D from a dict
segment2_d_from_dict = Segment2D.from_dict(segment2_d_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


