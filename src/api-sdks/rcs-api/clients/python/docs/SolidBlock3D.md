# SolidBlock3D

Representation of Solid Block in IDEA StatiCa Detail

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.solid_block3_d import SolidBlock3D

# TODO update the JSON string below
json = "{}"
# create an instance of SolidBlock3D from a JSON string
solid_block3_d_instance = SolidBlock3D.from_json(json)
# print the JSON string representation of the object
print(SolidBlock3D.to_json())

# convert the object into a dict
solid_block3_d_dict = solid_block3_d_instance.to_dict()
# create an instance of SolidBlock3D from a dict
solid_block3_d_from_dict = SolidBlock3D.from_dict(solid_block3_d_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


