# ConnectionData

Provides data of the connection

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**conenction_point_id** | **int** | Connection Point Id | [optional] 
**beams** | [**List[BeamData]**](BeamData.md) | Connected beams | [optional] 
**plates** | [**List[PlateData]**](PlateData.md) | Plates of the connection | [optional] 
**folded_plates** | [**List[FoldedPlateData]**](FoldedPlateData.md) | Folded plate of the connection | [optional] 
**bolt_grids** | [**List[BoltGrid]**](BoltGrid.md) | Bolt grids which belongs to the connection | [optional] 
**anchor_grids** | [**List[AnchorGrid]**](AnchorGrid.md) | Anchor grids which belongs to the connection | [optional] 
**welds** | [**List[WeldData]**](WeldData.md) | Welds of the connection | [optional] 
**concrete_blocks** | [**List[ConcreteBlockData]**](ConcreteBlockData.md) | ConcreteBlocksof the connection | [optional] 
**cut_beam_by_beams** | [**List[CutBeamByBeamData]**](CutBeamByBeamData.md) | cut beam by beams | [optional] 

## Example

```python
from ideastatica_connection_api.models.connection_data import ConnectionData

# TODO update the JSON string below
json = "{}"
# create an instance of ConnectionData from a JSON string
connection_data_instance = ConnectionData.from_json(json)
# print the JSON string representation of the object
print(ConnectionData.to_json())

# convert the object into a dict
connection_data_dict = connection_data_instance.to_dict()
# create an instance of ConnectionData from a dict
connection_data_from_dict = ConnectionData.from_dict(connection_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


