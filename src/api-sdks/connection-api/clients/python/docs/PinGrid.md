# PinGrid

Data of the pin grid

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**is_replaceable** | **bool** | Replaceable pin | [optional] 
**pin** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
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
from ideastatica_connection_api.models.pin_grid import PinGrid

# TODO update the JSON string below
json = "{}"
# create an instance of PinGrid from a JSON string
pin_grid_instance = PinGrid.from_json(json)
# print the JSON string representation of the object
print(PinGrid.to_json())

# convert the object into a dict
pin_grid_dict = pin_grid_instance.to_dict()
# create an instance of PinGrid from a dict
pin_grid_from_dict = PinGrid.from_dict(pin_grid_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


