# ParameterData


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**identifier** | **str** |  | [optional] 
**description** | **str** |  | [optional] 
**parameter_type** | **str** |  | [optional] 
**value** | **object** |  | [optional] 
**default_value** | **object** |  | [optional] 
**evaluated_value** | **object** |  | [optional] 
**evaluated_default_value** | **object** |  | [optional] 
**validation_value** | **str** |  | [optional] 
**evaluated_validation_value** | **str** |  | [optional] 
**validation_type** | [**ValidationType**](ValidationType.md) |  | [optional] 
**user_unit_id** | **int** |  | [optional] 
**is_visible_for_simple_connection** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.parameter_data import ParameterData

# TODO update the JSON string below
json = "{}"
# create an instance of ParameterData from a JSON string
parameter_data_instance = ParameterData.from_json(json)
# print the JSON string representation of the object
print(ParameterData.to_json())

# convert the object into a dict
parameter_data_dict = parameter_data_instance.to_dict()
# create an instance of ParameterData from a dict
parameter_data_from_dict = ParameterData.from_dict(parameter_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


