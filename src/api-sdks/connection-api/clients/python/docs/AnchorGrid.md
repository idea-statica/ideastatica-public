# AnchorGrid


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**shear_in_thread** | **bool** |  | [optional] 
**concrete_block** | [**ConcreteBlock**](ConcreteBlock.md) |  | [optional] 
**anchor_type** | [**AnchorType**](AnchorType.md) |  | [optional] 
**washer_size** | **float** |  | [optional] 
**anchoring_length** | **float** |  | [optional] 
**hook_length** | **float** |  | [optional] 
**bolt_assembly** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**origin** | [**Point3D**](Point3D.md) |  | [optional] 
**axis_x** | [**Vector3D**](Vector3D.md) |  | [optional] 
**axis_y** | [**Vector3D**](Vector3D.md) |  | [optional] 
**axis_z** | [**Vector3D**](Vector3D.md) |  | [optional] 
**positions** | [**List[Point3D]**](Point3D.md) |  | [optional] 
**connected_parts** | [**List[ReferenceElement]**](ReferenceElement.md) |  | [optional] 
**name** | **str** |  | [optional] 
**length** | **float** |  | [optional] 
**id** | **int** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.anchor_grid import AnchorGrid

# TODO update the JSON string below
json = "{}"
# create an instance of AnchorGrid from a JSON string
anchor_grid_instance = AnchorGrid.from_json(json)
# print the JSON string representation of the object
print(anchor_grid_instance.to_json())

# convert the object into a dict
anchor_grid_dict = anchor_grid_instance.to_dict()
# create an instance of AnchorGrid from a dict
anchor_grid_from_dict = AnchorGrid.from_dict(anchor_grid_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


