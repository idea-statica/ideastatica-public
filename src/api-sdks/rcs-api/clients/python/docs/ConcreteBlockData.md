# ConcreteBlockData

Provides data of the single concrete block

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Plate unique ID | [optional] 
**name** | **str** | Name of the concrete block | [optional] 
**depth** | **float** | Depth of the concrete block | [optional] 
**material** | **str** | Name of the material | [optional] 
**center** | [**Point3D**](Point3D.md) |  | [optional] 
**outline_points** | [**List[Point2D]**](Point2D.md) | Outline points | [optional] 
**origin** | [**Point3D**](Point3D.md) |  | [optional] 
**axis_x** | [**Vector3D**](Vector3D.md) |  | [optional] 
**axis_y** | [**Vector3D**](Vector3D.md) |  | [optional] 
**axis_z** | [**Vector3D**](Vector3D.md) |  | [optional] 
**region** | **str** | Geometry of the concrete block in svg format | [optional] 
**original_model_id** | **str** | Get or set the identification in the original model  In the case of the imported connection from another application | [optional] 

## Example

```python
from ideastatica_rcs_api.models.concrete_block_data import ConcreteBlockData

# TODO update the JSON string below
json = "{}"
# create an instance of ConcreteBlockData from a JSON string
concrete_block_data_instance = ConcreteBlockData.from_json(json)
# print the JSON string representation of the object
print(ConcreteBlockData.to_json())

# convert the object into a dict
concrete_block_data_dict = concrete_block_data_instance.to_dict()
# create an instance of ConcreteBlockData from a dict
concrete_block_data_from_dict = ConcreteBlockData.from_dict(concrete_block_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


