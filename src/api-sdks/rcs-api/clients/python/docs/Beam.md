# Beam

Representation of 1D member in IDEA StatiCa Detail

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.beam import Beam

# TODO update the JSON string below
json = "{}"
# create an instance of Beam from a JSON string
beam_instance = Beam.from_json(json)
# print the JSON string representation of the object
print(Beam.to_json())

# convert the object into a dict
beam_dict = beam_instance.to_dict()
# create an instance of Beam from a dict
beam_from_dict = Beam.from_dict(beam_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


