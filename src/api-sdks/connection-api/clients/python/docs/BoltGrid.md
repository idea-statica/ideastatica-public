# BoltGrid


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**shear_in_thread** | **bool** |  | [optional] 
**bolt_interaction** | [**BoltShearType**](BoltShearType.md) |  | [optional] 
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
from ideastatica_connection_api.models.bolt_grid import BoltGrid

# TODO update the JSON string below
json = "{}"
# create an instance of BoltGrid from a JSON string
bolt_grid_instance = BoltGrid.from_json(json)
# print the JSON string representation of the object
print(bolt_grid_instance.to_json())

# convert the object into a dict
bolt_grid_dict = bolt_grid_instance.to_dict()
# create an instance of BoltGrid from a dict
bolt_grid_from_dict = BoltGrid.from_dict(bolt_grid_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


