# CutBeamByBeamData

Provides data of the cut objec by object

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** | Name of the cut | [optional] 
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_connection_api.models.cut_beam_by_beam_data import CutBeamByBeamData

# TODO update the JSON string below
json = "{}"
# create an instance of CutBeamByBeamData from a JSON string
cut_beam_by_beam_data_instance = CutBeamByBeamData.from_json(json)
# print the JSON string representation of the object
print(cut_beam_by_beam_data_instance.to_json())

# convert the object into a dict
cut_beam_by_beam_data_dict = cut_beam_by_beam_data_instance.to_dict()
# create an instance of CutBeamByBeamData from a dict
cut_beam_by_beam_data_from_dict = CutBeamByBeamData.from_dict(cut_beam_by_beam_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


