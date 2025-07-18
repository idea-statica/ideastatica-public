# IdeaParameterValidation


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**validation_expression** | **str** |  | [optional] 
**validation_expression_evaluated** | **bool** |  | [optional] 
**lower_bound** | **str** |  | [optional] 
**lower_bound_evaluated** | **float** |  | [optional] 
**upper_bound** | **str** |  | [optional] 
**upper_bound_evaluated** | **float** |  | [optional] 
**validation_status** | **str** |  | [optional] 
**message** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.idea_parameter_validation import IdeaParameterValidation

# TODO update the JSON string below
json = "{}"
# create an instance of IdeaParameterValidation from a JSON string
idea_parameter_validation_instance = IdeaParameterValidation.from_json(json)
# print the JSON string representation of the object
print(IdeaParameterValidation.to_json())

# convert the object into a dict
idea_parameter_validation_dict = idea_parameter_validation_instance.to_dict()
# create an instance of IdeaParameterValidation from a dict
idea_parameter_validation_from_dict = IdeaParameterValidation.from_dict(idea_parameter_validation_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


