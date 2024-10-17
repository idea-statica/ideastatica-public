# ConCalculationParameter


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**connection_ids** | **List[int]** |  | [optional] 
**analysis_type** | [**ConAnalysisTypeEnum**](ConAnalysisTypeEnum.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_calculation_parameter import ConCalculationParameter

# TODO update the JSON string below
json = "{}"
# create an instance of ConCalculationParameter from a JSON string
con_calculation_parameter_instance = ConCalculationParameter.from_json(json)
# print the JSON string representation of the object
print(ConCalculationParameter.to_json())

# convert the object into a dict
con_calculation_parameter_dict = con_calculation_parameter_instance.to_dict()
# create an instance of ConCalculationParameter from a dict
con_calculation_parameter_from_dict = ConCalculationParameter.from_dict(con_calculation_parameter_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


