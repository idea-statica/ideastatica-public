# PatchDevice

Abstract class of patch support/load

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.patch_device import PatchDevice

# TODO update the JSON string below
json = "{}"
# create an instance of PatchDevice from a JSON string
patch_device_instance = PatchDevice.from_json(json)
# print the JSON string representation of the object
print(PatchDevice.to_json())

# convert the object into a dict
patch_device_dict = patch_device_instance.to_dict()
# create an instance of PatchDevice from a dict
patch_device_from_dict = PatchDevice.from_dict(patch_device_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


