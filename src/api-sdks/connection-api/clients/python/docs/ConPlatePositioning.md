# ConPlatePositioning


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**con_plate_positioning_type** | [**ConPlatePositioningEnum**](ConPlatePositioningEnum.md) |  | [optional] 
**concrete_block_index** | **int** | Used when conPlatePositioningType is concreteBlock. Despite the name, not a block index: the id of the operation that creates the concrete block, i.e. an anchor grid or contact grid whose blockType is new (list them with GET .../operations/anchor-grid and .../operations/contact-grid). The operation must come before the plate, i.e. have a lower id. | [optional] 
**concrete_surface** | **int** | Used when conPlatePositioningType is concreteBlock. The face of the block the plate is placed on, 1 to 6 for a rectangular block: 1 to 4 are the sides, 5 is the bottom and 6 is the top. | [optional] 
**position_x** | **float** |  | [optional] 
**position_y** | **float** |  | [optional] 
**rotation_angle** | **float** |  | [optional] 
**coordinate_system** | [**ConLocalCoordinateSystem**](ConLocalCoordinateSystem.md) |  | [optional] 
**offset** | [**Point3D**](Point3D.md) |  | [optional] 
**rotation** | [**Point3D**](Point3D.md) |  | [optional] 
**on_member_id** | **int** |  | [optional] 
**on_member_operation_id** | **int** |  | [optional] 
**input_method** | [**ConWorkPlaneMethod**](ConWorkPlaneMethod.md) |  | [optional] 
**normal_vector** | [**Vector3D**](Vector3D.md) |  | [optional] 
**plate_on_member_index** | **int** |  | [optional] 
**function** | [**ConPlateFunction**](ConPlateFunction.md) |  | [optional] 
**location** | [**ConLocation**](ConLocation.md) |  | [optional] 
**pitch** | **float** |  | [optional] 
**plate_index** | **int** |  | [optional] 
**operation_id** | **int** |  | [optional] 
**plate_edge_index** | **int** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_plate_positioning import ConPlatePositioning

# TODO update the JSON string below
json = "{}"
# create an instance of ConPlatePositioning from a JSON string
con_plate_positioning_instance = ConPlatePositioning.from_json(json)
# print the JSON string representation of the object
print(con_plate_positioning_instance.to_json())

# convert the object into a dict
con_plate_positioning_dict = con_plate_positioning_instance.to_dict()
# create an instance of ConPlatePositioning from a dict
con_plate_positioning_from_dict = ConPlatePositioning.from_dict(con_plate_positioning_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


