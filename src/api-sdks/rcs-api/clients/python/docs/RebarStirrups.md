# RebarStirrups

Represents a rebar grouping in 3D space.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rebar_stirrups import RebarStirrups

# TODO update the JSON string below
json = "{}"
# create an instance of RebarStirrups from a JSON string
rebar_stirrups_instance = RebarStirrups.from_json(json)
# print the JSON string representation of the object
print(RebarStirrups.to_json())

# convert the object into a dict
rebar_stirrups_dict = rebar_stirrups_instance.to_dict()
# create an instance of RebarStirrups from a dict
rebar_stirrups_from_dict = RebarStirrups.from_dict(rebar_stirrups_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


