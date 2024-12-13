# BoltGrid


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**shear_in_thread** | **bool** | Indicates, whether a shear plane is in the thread of a bolt. | [optional] 
**bolt_interaction** | [**BoltShearType**](BoltShearType.md) |  | [optional] 
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
from ideastatica_rcs_api.models.bolt_grid import BoltGrid

# TODO update the JSON string below
json = "{}"
# create an instance of BoltGrid from a JSON string
bolt_grid_instance = BoltGrid.from_json(json)
# print the JSON string representation of the object
print(BoltGrid.to_json())

# convert the object into a dict
bolt_grid_dict = bolt_grid_instance.to_dict()
# create an instance of BoltGrid from a dict
bolt_grid_from_dict = BoltGrid.from_dict(bolt_grid_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


