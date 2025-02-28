# StringStringValuesKeyValuePair


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**key** | **str** |  | [optional] 
**value** | **List[str]** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.string_string_values_key_value_pair import StringStringValuesKeyValuePair

# TODO update the JSON string below
json = "{}"
# create an instance of StringStringValuesKeyValuePair from a JSON string
string_string_values_key_value_pair_instance = StringStringValuesKeyValuePair.from_json(json)
# print the JSON string representation of the object
print(StringStringValuesKeyValuePair.to_json())

# convert the object into a dict
string_string_values_key_value_pair_dict = string_string_values_key_value_pair_instance.to_dict()
# create an instance of StringStringValuesKeyValuePair from a dict
string_string_values_key_value_pair_from_dict = StringStringValuesKeyValuePair.from_dict(string_string_values_key_value_pair_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


