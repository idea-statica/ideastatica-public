# Reinforcement

Base class representing reinforcement in IDEA StatiCa Detail

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.reinforcement import Reinforcement

# TODO update the JSON string below
json = "{}"
# create an instance of Reinforcement from a JSON string
reinforcement_instance = Reinforcement.from_json(json)
# print the JSON string representation of the object
print(Reinforcement.to_json())

# convert the object into a dict
reinforcement_dict = reinforcement_instance.to_dict()
# create an instance of Reinforcement from a dict
reinforcement_from_dict = Reinforcement.from_dict(reinforcement_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


