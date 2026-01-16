# RcsMaterial


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**material_type** | [**RcsMaterialType**](RcsMaterialType.md) |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_material import RcsMaterial

# TODO update the JSON string below
json = "{}"
# create an instance of RcsMaterial from a JSON string
rcs_material_instance = RcsMaterial.from_json(json)
# print the JSON string representation of the object
print(rcs_material_instance.to_json())

# convert the object into a dict
rcs_material_dict = rcs_material_instance.to_dict()
# create an instance of RcsMaterial from a dict
rcs_material_from_dict = RcsMaterial.from_dict(rcs_material_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


