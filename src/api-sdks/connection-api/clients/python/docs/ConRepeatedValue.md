# ConRepeatedValue


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**value** | **float** |  | [optional] 
**repeat_count** | **int** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_repeated_value import ConRepeatedValue

# TODO update the JSON string below
json = "{}"
# create an instance of ConRepeatedValue from a JSON string
con_repeated_value_instance = ConRepeatedValue.from_json(json)
# print the JSON string representation of the object
print(con_repeated_value_instance.to_json())

# convert the object into a dict
con_repeated_value_dict = con_repeated_value_instance.to_dict()
# create an instance of ConRepeatedValue from a dict
con_repeated_value_from_dict = ConRepeatedValue.from_dict(con_repeated_value_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


