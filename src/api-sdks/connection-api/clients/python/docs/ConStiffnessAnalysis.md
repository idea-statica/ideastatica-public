# ConStiffnessAnalysis


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**theoretical_length_z** | **float** |  | [optional] 
**theoretical_length_y** | **float** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_stiffness_analysis import ConStiffnessAnalysis

# TODO update the JSON string below
json = "{}"
# create an instance of ConStiffnessAnalysis from a JSON string
con_stiffness_analysis_instance = ConStiffnessAnalysis.from_json(json)
# print the JSON string representation of the object
print(con_stiffness_analysis_instance.to_json())

# convert the object into a dict
con_stiffness_analysis_dict = con_stiffness_analysis_instance.to_dict()
# create an instance of ConStiffnessAnalysis from a dict
con_stiffness_analysis_from_dict = ConStiffnessAnalysis.from_dict(con_stiffness_analysis_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


