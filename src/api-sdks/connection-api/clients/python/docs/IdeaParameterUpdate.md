# IdeaParameterUpdate


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**key** | **str** |  | [optional] 
**expression** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.idea_parameter_update import IdeaParameterUpdate

# TODO update the JSON string below
json = "{}"
# create an instance of IdeaParameterUpdate from a JSON string
idea_parameter_update_instance = IdeaParameterUpdate.from_json(json)
# print the JSON string representation of the object
print(IdeaParameterUpdate.to_json())

# convert the object into a dict
idea_parameter_update_dict = idea_parameter_update_instance.to_dict()
# create an instance of IdeaParameterUpdate from a dict
idea_parameter_update_from_dict = IdeaParameterUpdate.from_dict(idea_parameter_update_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


