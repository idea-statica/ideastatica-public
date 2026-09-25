# ConGridGeometry


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**type** | [**ConGridGeometryType**](ConGridGeometryType.md) |  | [optional] 
**shear_plane_in_thread** | **bool** |  | [optional] 
**rows_grid_layout** | [**ConGridLayout**](ConGridLayout.md) |  | [optional] 
**top_layers** | [**ConGridPositions**](ConGridPositions.md) |  | [optional] 
**bottom_layers** | [**ConGridPositions**](ConGridPositions.md) |  | [optional] 
**left_layers** | [**ConGridPositions**](ConGridPositions.md) |  | [optional] 
**right_layers** | [**ConGridPositions**](ConGridPositions.md) |  | [optional] 
**polar_input** | [**ConPolarInputType**](ConPolarInputType.md) |  | [optional] 
**radii** | [**ConGridPositions**](ConGridPositions.md) |  | [optional] 
**polar_counts** | **List[int]** |  | [optional] 
**angles** | [**ConGridPositions**](ConGridPositions.md) |  | [optional] 
**user_positions** | [**List[ConFastenerPosition]**](ConFastenerPosition.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_grid_geometry import ConGridGeometry

# TODO update the JSON string below
json = "{}"
# create an instance of ConGridGeometry from a JSON string
con_grid_geometry_instance = ConGridGeometry.from_json(json)
# print the JSON string representation of the object
print(con_grid_geometry_instance.to_json())

# convert the object into a dict
con_grid_geometry_dict = con_grid_geometry_instance.to_dict()
# create an instance of ConGridGeometry from a dict
con_grid_geometry_from_dict = ConGridGeometry.from_dict(con_grid_geometry_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


