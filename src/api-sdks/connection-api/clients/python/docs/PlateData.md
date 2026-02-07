# PlateData


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** |  | [optional] 
**thickness** | **float** |  | [optional] 
**material** | **str** |  | [optional] 
**outline_points** | [**List[Point2D]**](Point2D.md) |  | [optional] 
**origin** | [**Point3D**](Point3D.md) |  | [optional] 
**axis_x** | [**Vector3D**](Vector3D.md) |  | [optional] 
**axis_y** | [**Vector3D**](Vector3D.md) |  | [optional] 
**axis_z** | [**Vector3D**](Vector3D.md) |  | [optional] 
**region** | **str** |  | [optional] 
**geometry** | [**Region2D**](Region2D.md) |  | [optional] 
**original_model_id** | **str** |  | [optional] 
**is_negative_object** | **bool** |  | [optional] 
**id** | **int** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.plate_data import PlateData

# TODO update the JSON string below
json = "{}"
# create an instance of PlateData from a JSON string
plate_data_instance = PlateData.from_json(json)
# print the JSON string representation of the object
print(plate_data_instance.to_json())

# convert the object into a dict
plate_data_dict = plate_data_instance.to_dict()
# create an instance of PlateData from a dict
plate_data_from_dict = PlateData.from_dict(plate_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


