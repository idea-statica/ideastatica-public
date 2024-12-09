# Tendon

Tendon base class

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.tendon import Tendon

# TODO update the JSON string below
json = "{}"
# create an instance of Tendon from a JSON string
tendon_instance = Tendon.from_json(json)
# print the JSON string representation of the object
print(Tendon.to_json())

# convert the object into a dict
tendon_dict = tendon_instance.to_dict()
# create an instance of Tendon from a dict
tendon_from_dict = Tendon.from_dict(tendon_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


