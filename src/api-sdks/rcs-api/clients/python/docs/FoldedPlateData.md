# FoldedPlateData

Provides data of the folded plate

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**plates** | [**List[PlateData]**](PlateData.md) | List of plates belong to folded plate | [optional] 
**bends** | [**List[BendData]**](BendData.md) | List of bends connected plates of foldedplate | [optional] 

## Example

```python
from ideastatica_rcs_api.models.folded_plate_data import FoldedPlateData

# TODO update the JSON string below
json = "{}"
# create an instance of FoldedPlateData from a JSON string
folded_plate_data_instance = FoldedPlateData.from_json(json)
# print the JSON string representation of the object
print(FoldedPlateData.to_json())

# convert the object into a dict
folded_plate_data_dict = folded_plate_data_instance.to_dict()
# create an instance of FoldedPlateData from a dict
folded_plate_data_from_dict = FoldedPlateData.from_dict(folded_plate_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


