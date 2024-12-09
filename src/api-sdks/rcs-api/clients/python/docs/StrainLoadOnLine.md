# StrainLoadOnLine

Linearly distributed generalized strain load along a line.  Strain load is in local coordinate system and there are no possible eccentricities.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.strain_load_on_line import StrainLoadOnLine

# TODO update the JSON string below
json = "{}"
# create an instance of StrainLoadOnLine from a JSON string
strain_load_on_line_instance = StrainLoadOnLine.from_json(json)
# print the JSON string representation of the object
print(StrainLoadOnLine.to_json())

# convert the object into a dict
strain_load_on_line_dict = strain_load_on_line_instance.to_dict()
# create an instance of StrainLoadOnLine from a dict
strain_load_on_line_from_dict = StrainLoadOnLine.from_dict(strain_load_on_line_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


