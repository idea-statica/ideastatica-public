# RebarGeneral

Represents a single main rebar in 3D space.  Holds data from generated rebar or rebar imported from Tekla.  the rebarShape is in global coordinates and it is not possible to prject it along Member1D, Polyline, ... (referenceLine)

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rebar_general import RebarGeneral

# TODO update the JSON string below
json = "{}"
# create an instance of RebarGeneral from a JSON string
rebar_general_instance = RebarGeneral.from_json(json)
# print the JSON string representation of the object
print(RebarGeneral.to_json())

# convert the object into a dict
rebar_general_dict = rebar_general_instance.to_dict()
# create an instance of RebarGeneral from a dict
rebar_general_from_dict = RebarGeneral.from_dict(rebar_general_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


