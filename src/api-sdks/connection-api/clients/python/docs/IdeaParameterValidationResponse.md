# IdeaParameterValidationResponse


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**key** | **str** |  | [optional] 
**message** | **str** |  | [optional] 
**validation_status** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.idea_parameter_validation_response import IdeaParameterValidationResponse

# TODO update the JSON string below
json = "{}"
# create an instance of IdeaParameterValidationResponse from a JSON string
idea_parameter_validation_response_instance = IdeaParameterValidationResponse.from_json(json)
# print the JSON string representation of the object
print(IdeaParameterValidationResponse.to_json())

# convert the object into a dict
idea_parameter_validation_response_dict = idea_parameter_validation_response_instance.to_dict()
# create an instance of IdeaParameterValidationResponse from a dict
idea_parameter_validation_response_from_dict = IdeaParameterValidationResponse.from_dict(idea_parameter_validation_response_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


