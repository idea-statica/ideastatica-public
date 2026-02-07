# ConcreteBlock


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**lenght** | **float** |  | [optional] 
**width** | **float** |  | [optional] 
**height** | **float** |  | [optional] 
**material** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.concrete_block import ConcreteBlock

# TODO update the JSON string below
json = "{}"
# create an instance of ConcreteBlock from a JSON string
concrete_block_instance = ConcreteBlock.from_json(json)
# print the JSON string representation of the object
print(concrete_block_instance.to_json())

# convert the object into a dict
concrete_block_dict = concrete_block_instance.to_dict()
# create an instance of ConcreteBlock from a dict
concrete_block_from_dict = ConcreteBlock.from_dict(concrete_block_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


