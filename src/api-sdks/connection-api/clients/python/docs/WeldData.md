# WeldData

Provides data of the single weld

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Id of the weld | [optional] 
**name** | **str** | Name of the weld | [optional] 
**thickness** | **float** | Thickness of the weld | [optional] 
**material** | **str** | Name of the material | [optional] 
**weld_material** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**weld_type** | [**WeldType**](WeldType.md) |  | [optional] 
**connected_part_ids** | **List[str]** | Id of the weld | [optional] 
**start** | [**Point3D**](Point3D.md) |  | [optional] 
**end** | [**Point3D**](Point3D.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.weld_data import WeldData

# TODO update the JSON string below
json = "{}"
# create an instance of WeldData from a JSON string
weld_data_instance = WeldData.from_json(json)
# print the JSON string representation of the object
print(WeldData.to_json())

# convert the object into a dict
weld_data_dict = weld_data_instance.to_dict()
# create an instance of WeldData from a dict
weld_data_from_dict = WeldData.from_dict(weld_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


