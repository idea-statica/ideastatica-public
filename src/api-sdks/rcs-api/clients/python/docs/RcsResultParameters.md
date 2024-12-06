# RcsResultParameters


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**sections** | **List[int]** |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_result_parameters import RcsResultParameters

# TODO update the JSON string below
json = "{}"
# create an instance of RcsResultParameters from a JSON string
rcs_result_parameters_instance = RcsResultParameters.from_json(json)
# print the JSON string representation of the object
print(RcsResultParameters.to_json())

# convert the object into a dict
rcs_result_parameters_dict = rcs_result_parameters_instance.to_dict()
# create an instance of RcsResultParameters from a dict
rcs_result_parameters_from_dict = RcsResultParameters.from_dict(rcs_result_parameters_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


