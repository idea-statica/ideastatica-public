# Region3D

Represents a region in three-dimensional space included outline (border) and openings.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.region3_d import Region3D

# TODO update the JSON string below
json = "{}"
# create an instance of Region3D from a JSON string
region3_d_instance = Region3D.from_json(json)
# print the JSON string representation of the object
print(Region3D.to_json())

# convert the object into a dict
region3_d_dict = region3_d_instance.to_dict()
# create an instance of Region3D from a dict
region3_d_from_dict = Region3D.from_dict(region3_d_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


