# ConAnchorGridOperation


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**connected_items** | [**List[ConConnectedItem]**](ConConnectedItem.md) |  | [optional] 
**anchor_type** | [**ConAnchorType**](ConAnchorType.md) |  | [optional] 
**anchor_assembly_id** | **int** |  | [optional] 
**embedment_depth** | **float** |  | [optional] 
**hook_length** | **float** |  | [optional] 
**anchor_diameter** | **float** |  | [optional] 
**head_diameter** | **float** |  | [optional] 
**headed_stud_material_id** | **int** |  | [optional] 
**washer_plate_shape** | [**ConWasherPlateShape**](ConWasherPlateShape.md) |  | [optional] 
**washer_plate_size** | **float** |  | [optional] 
**reinforcement_material_id** | **int** |  | [optional] 
**reinforcement_shape** | [**ConReinforcementAnchorShape**](ConReinforcementAnchorShape.md) |  | [optional] 
**mandrel_diameter** | **float** |  | [optional] 
**hook_rotations** | **List[float]** |  | [optional] 
**is_exploded** | **bool** |  | [optional] 
**geometry** | [**ConGridGeometry**](ConGridGeometry.md) |  | [optional] 
**defined_by** | [**ConDefinedBy**](ConDefinedBy.md) |  | [optional] 
**coordinate_system** | [**ConLocalCoordinateSystem**](ConLocalCoordinateSystem.md) |  | [optional] 
**slotted_holes** | [**List[ConSlottedHole]**](ConSlottedHole.md) |  | [optional] 
**foundation_block** | [**ConFoundationBlockDto**](ConFoundationBlockDto.md) |  | [optional] 
**block_type** | [**ConBlockType**](ConBlockType.md) |  | [optional] 
**existing_block_operation_id** | **int** |  | [optional] 
**plate_side** | [**ConPlateSide**](ConPlateSide.md) |  | [optional] 
**is_imported** | **bool** |  | [optional] 
**operation_type** | **str** |  | [optional] 
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**active** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_anchor_grid_operation import ConAnchorGridOperation

# TODO update the JSON string below
json = "{}"
# create an instance of ConAnchorGridOperation from a JSON string
con_anchor_grid_operation_instance = ConAnchorGridOperation.from_json(json)
# print the JSON string representation of the object
print(con_anchor_grid_operation_instance.to_json())

# convert the object into a dict
con_anchor_grid_operation_dict = con_anchor_grid_operation_instance.to_dict()
# create an instance of ConAnchorGridOperation from a dict
con_anchor_grid_operation_from_dict = ConAnchorGridOperation.from_dict(con_anchor_grid_operation_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


