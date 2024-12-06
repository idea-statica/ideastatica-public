# RebarShape

Represents a geometrical shape for Rebar in 3D space.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rebar_shape import RebarShape

# TODO update the JSON string below
json = "{}"
# create an instance of RebarShape from a JSON string
rebar_shape_instance = RebarShape.from_json(json)
# print the JSON string representation of the object
print(RebarShape.to_json())

# convert the object into a dict
rebar_shape_dict = rebar_shape_instance.to_dict()
# create an instance of RebarShape from a dict
rebar_shape_from_dict = RebarShape.from_dict(rebar_shape_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


