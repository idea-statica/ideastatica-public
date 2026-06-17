# FemElement

Provides information about FEM element

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**type** | **int** | Type of the finite element  1 - triangular element 2D  2 - quadrilateral element 2D | [optional] 
**vertices** | **List[int]** | Get list of vertices of the fem element | [optional] 

## Example

```python
from ideastatica_connection_api.models.fem_element import FemElement

# TODO update the JSON string below
json = "{}"
# create an instance of FemElement from a JSON string
fem_element_instance = FemElement.from_json(json)
# print the JSON string representation of the object
print(fem_element_instance.to_json())

# convert the object into a dict
fem_element_dict = fem_element_instance.to_dict()
# create an instance of FemElement from a dict
fem_element_from_dict = FemElement.from_dict(fem_element_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


