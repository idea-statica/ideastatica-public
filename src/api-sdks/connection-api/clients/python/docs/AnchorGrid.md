# AnchorGrid

Data of the anchor grid

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**shear_in_thread** | **bool** | Indicates, whether a shear plane is in the thread of a bolt. | [optional] 
**concrete_block** | [**ConcreteBlock**](ConcreteBlock.md) |  | [optional] 
**anchor_type** | [**AnchorType**](AnchorType.md) |  | [optional] 
**washer_size** | **float** | Washer Size used if AnchorType is washer | [optional] 
**anchoring_length** | **float** | Anchoring Length | [optional] 
**hook_length** | **float** | Length of anchor hook&lt;br /&gt;  (distance from the inner surface of the anchor shaft to the outer tip of the hook specified as an anchor diameter multiplier) | [optional] 
**bolt_assembly** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**origin** | [**Point3D**](Point3D.md) |  | [optional] 
**axis_x** | [**Vector3D**](Vector3D.md) |  | [optional] 
**axis_y** | [**Vector3D**](Vector3D.md) |  | [optional] 
**axis_z** | [**Vector3D**](Vector3D.md) |  | [optional] 
**positions** | [**List[Point3D]**](Point3D.md) | Positions of holes in the local coordinate system of the grid | [optional] 
**connected_parts** | [**List[ReferenceElement]**](ReferenceElement.md) | List of the connected parts | [optional] 
**name** | **str** | Name | [optional] 
**length** | **float** | Length | [optional] 
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_connection_api.models.anchor_grid import AnchorGrid

# TODO update the JSON string below
json = "{}"
# create an instance of AnchorGrid from a JSON string
anchor_grid_instance = AnchorGrid.from_json(json)
# print the JSON string representation of the object
print(AnchorGrid.to_json())

# convert the object into a dict
anchor_grid_dict = anchor_grid_instance.to_dict()
# create an instance of AnchorGrid from a dict
anchor_grid_from_dict = AnchorGrid.from_dict(anchor_grid_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


