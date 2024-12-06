# Region2D

Represents a region in two-dimensional space included outline (border) and openings.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**outline** | [**PolyLine2D**](PolyLine2D.md) |  | [optional] 
**openings** | [**List[PolyLine2D]**](PolyLine2D.md) | Gets or sets the list of openings in the Region2D. | [optional] 

## Example

```python
from ideastatica_rcs_api.models.region2_d import Region2D

# TODO update the JSON string below
json = "{}"
# create an instance of Region2D from a JSON string
region2_d_instance = Region2D.from_json(json)
# print the JSON string representation of the object
print(Region2D.to_json())

# convert the object into a dict
region2_d_dict = region2_d_instance.to_dict()
# create an instance of Region2D from a dict
region2_d_from_dict = Region2D.from_dict(region2_d_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


