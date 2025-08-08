# ParameterUpdateResponse


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**set_to_model** | **bool** |  | [optional] 
**parameters** | [**List[IdeaParameter]**](IdeaParameter.md) |  | [optional] 
**failed_validations** | [**List[IdeaParameterValidationResponse]**](IdeaParameterValidationResponse.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.parameter_update_response import ParameterUpdateResponse

# TODO update the JSON string below
json = "{}"
# create an instance of ParameterUpdateResponse from a JSON string
parameter_update_response_instance = ParameterUpdateResponse.from_json(json)
# print the JSON string representation of the object
print(ParameterUpdateResponse.to_json())

# convert the object into a dict
parameter_update_response_dict = parameter_update_response_instance.to_dict()
# create an instance of ParameterUpdateResponse from a dict
parameter_update_response_from_dict = ParameterUpdateResponse.from_dict(parameter_update_response_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


