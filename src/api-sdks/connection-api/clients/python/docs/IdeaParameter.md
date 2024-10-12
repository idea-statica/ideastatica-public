# IdeaParameter


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**key** | **str** |  | [optional] 
**expression** | **str** |  | [optional] 
**value** | **object** |  | [optional] 
**unit** | **str** |  | [optional] 
**parameter_type** | **str** |  | [optional] 
**validation_expression** | **str** |  | [optional] 
**description** | **str** |  | [optional] 
**validation_status** | **str** |  | [optional] 
**is_visible** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.idea_parameter import IdeaParameter

# TODO update the JSON string below
json = "{}"
# create an instance of IdeaParameter from a JSON string
idea_parameter_instance = IdeaParameter.from_json(json)
# print the JSON string representation of the object
print(IdeaParameter.to_json())

# convert the object into a dict
idea_parameter_dict = idea_parameter_instance.to_dict()
# create an instance of IdeaParameter from a dict
idea_parameter_from_dict = IdeaParameter.from_dict(idea_parameter_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


