# BoltGrid

Data of the bolt grid

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**bolt_assembly_ref** | **str** |  | [optional] 
**id** | **int** | Unique Id of the bolt grid | [optional] 
**is_anchor** | **bool** | Is Anchor | [optional] 
**anchor_len** | **float** | Anchor lenght | [optional] 
**hole_diameter** | **float** | The diameter of the hole | [optional] 
**diameter** | **float** | The diameter of bolt | [optional] 
**head_diameter** | **float** | The head diameter of bolt | [optional] 
**diagonal_head_diameter** | **float** | The Diagonal Head Diameter of bolt | [optional] 
**head_height** | **float** | The Head Height of bolt | [optional] 
**bore_hole** | **float** | The BoreHole of bolt | [optional] 
**tensile_stress_area** | **float** | The Tensile Stress Area of bolt | [optional] 
**nut_thickness** | **float** | The Nut Thickness of bolt | [optional] 
**bolt_assembly_name** | **str** | The description of the bolt assembly | [optional] 
**standard** | **str** | The standard of the bolt assembly | [optional] 
**material** | **str** | The material of the bolt assembly | [optional] 
**origin** | [**Point3D**](Point3D.md) |  | [optional] 
**axis_x** | [**Vector3D**](Vector3D.md) |  | [optional] 
**axis_y** | [**Vector3D**](Vector3D.md) |  | [optional] 
**axis_z** | [**Vector3D**](Vector3D.md) |  | [optional] 
**positions** | [**List[Point3D]**](Point3D.md) | Positions of holes in the local coodinate system of the bolt grid | [optional] 
**connected_plates** | **List[int]** | Identifiers of the connected plates | [optional] 
**connected_part_ids** | **List[str]** | Id of the weld | [optional] 
**shear_in_thread** | **bool** | Indicates, whether a shear plane is in the thread of a bolt. | [optional] 
**bolt_interaction** | [**BoltShearType**](BoltShearType.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.bolt_grid import BoltGrid

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

