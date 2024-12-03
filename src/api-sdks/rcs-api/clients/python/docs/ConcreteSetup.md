# ConcreteSetup

Concrete setup base class

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.concrete_setup import ConcreteSetup

# TODO update the JSON string below
json = "{}"
# create an instance of ConcreteSetup from a JSON string
concrete_setup_instance = ConcreteSetup.from_json(json)
# print the JSON string representation of the object
print(ConcreteSetup.to_json())

# convert the object into a dict
concrete_setup_dict = concrete_setup_instance.to_dict()
# create an instance of ConcreteSetup from a dict
concrete_setup_from_dict = ConcreteSetup.from_dict(concrete_setup_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


