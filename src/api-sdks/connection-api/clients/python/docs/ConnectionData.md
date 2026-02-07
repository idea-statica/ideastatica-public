# ConnectionData


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**connection_point** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**beams** | [**List[BeamData]**](BeamData.md) |  | [optional] 
**plates** | [**List[PlateData]**](PlateData.md) |  | [optional] 
**folded_plates** | [**List[FoldedPlateData]**](FoldedPlateData.md) |  | [optional] 
**bolt_grids** | [**List[BoltGrid]**](BoltGrid.md) |  | [optional] 
**anchor_grids** | [**List[AnchorGrid]**](AnchorGrid.md) |  | [optional] 
**pin_grids** | [**List[PinGrid]**](PinGrid.md) |  | [optional] 
**welds** | [**List[WeldData]**](WeldData.md) |  | [optional] 
**concrete_blocks** | [**List[ConcreteBlockData]**](ConcreteBlockData.md) |  | [optional] 
**cut_beam_by_beams** | [**List[CutBeamByBeamData]**](CutBeamByBeamData.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.connection_data import ConnectionData

# TODO update the JSON string below
json = "{}"
# create an instance of ConnectionData from a JSON string
connection_data_instance = ConnectionData.from_json(json)
# print the JSON string representation of the object
print(connection_data_instance.to_json())

# convert the object into a dict
connection_data_dict = connection_data_instance.to_dict()
# create an instance of ConnectionData from a dict
connection_data_from_dict = ConnectionData.from_dict(connection_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


