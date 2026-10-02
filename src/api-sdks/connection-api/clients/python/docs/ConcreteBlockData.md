# ConcreteBlockData

Provides data of the single concrete block

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_connection_api.models.concrete_block_data import ConcreteBlockData

# TODO update the JSON string below
json = "{}"
# create an instance of ConcreteBlockData from a JSON string
concrete_block_data_instance = ConcreteBlockData.from_json(json)
# print the JSON string representation of the object
print(concrete_block_data_instance.to_json())

# convert the object into a dict
concrete_block_data_dict = concrete_block_data_instance.to_dict()
# create an instance of ConcreteBlockData from a dict
concrete_block_data_from_dict = ConcreteBlockData.from_dict(concrete_block_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


