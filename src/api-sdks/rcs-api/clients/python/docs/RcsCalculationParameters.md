# RcsCalculationParameters


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**sections** | **List[int]** |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_calculation_parameters import RcsCalculationParameters

# TODO update the JSON string below
json = "{}"
# create an instance of RcsCalculationParameters from a JSON string
rcs_calculation_parameters_instance = RcsCalculationParameters.from_json(json)
# print the JSON string representation of the object
print(RcsCalculationParameters.to_json())

# convert the object into a dict
rcs_calculation_parameters_dict = rcs_calculation_parameters_instance.to_dict()
# create an instance of RcsCalculationParameters from a dict
rcs_calculation_parameters_from_dict = RcsCalculationParameters.from_dict(rcs_calculation_parameters_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


