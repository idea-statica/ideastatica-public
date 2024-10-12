# Line


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**color** | **List[int]** |  | [optional] 
**pairs** | **List[int]** |  | [optional] 
**thickness** | **float** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.line import Line

# TODO update the JSON string below
json = "{}"
# create an instance of Line from a JSON string
line_instance = Line.from_json(json)
# print the JSON string representation of the object
print(Line.to_json())

# convert the object into a dict
line_dict = line_instance.to_dict()
# create an instance of Line from a dict
line_from_dict = Line.from_dict(line_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


