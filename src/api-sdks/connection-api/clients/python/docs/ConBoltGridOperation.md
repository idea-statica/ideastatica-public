# ConBoltGridOperation


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**connected_items** | [**List[ConConnectedItem]**](ConConnectedItem.md) |  | [optional] 
**bolt_assembly_id** | **int** |  | [optional] 
**bolt_shear_force_transfer** | [**ConBoltShearTransfer**](ConBoltShearTransfer.md) |  | [optional] 
**is_exploded** | **bool** |  | [optional] 
**geometry** | [**ConGridGeometry**](ConGridGeometry.md) |  | [optional] 
**defined_by** | [**ConDefinedBy**](ConDefinedBy.md) |  | [optional] 
**coordinate_system** | [**ConLocalCoordinateSystem**](ConLocalCoordinateSystem.md) |  | [optional] 
**slotted_holes** | [**List[ConSlottedHole]**](ConSlottedHole.md) |  | [optional] 
**is_imported** | **bool** |  | [optional] 
**operation_type** | **str** |  | [optional] 
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**active** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_bolt_grid_operation import ConBoltGridOperation

# TODO update the JSON string below
json = "{}"
# create an instance of ConBoltGridOperation from a JSON string
con_bolt_grid_operation_instance = ConBoltGridOperation.from_json(json)
# print the JSON string representation of the object
print(con_bolt_grid_operation_instance.to_json())

# convert the object into a dict
con_bolt_grid_operation_dict = con_bolt_grid_operation_instance.to_dict()
# create an instance of ConBoltGridOperation from a dict
con_bolt_grid_operation_from_dict = ConBoltGridOperation.from_dict(con_bolt_grid_operation_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


