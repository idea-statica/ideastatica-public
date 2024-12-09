# RebarSingle

Represents a single main rebar in 3D space.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rebar_single import RebarSingle

# TODO update the JSON string below
json = "{}"
# create an instance of RebarSingle from a JSON string
rebar_single_instance = RebarSingle.from_json(json)
# print the JSON string representation of the object
print(RebarSingle.to_json())

# convert the object into a dict
rebar_single_dict = rebar_single_instance.to_dict()
# create an instance of RebarSingle from a dict
rebar_single_from_dict = RebarSingle.from_dict(rebar_single_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


