# ReferenceElement

Reference element class

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**type_name** | **str** | Element type name | [optional] 
**id** | **int** | Element Id | [optional] 
**element** | [**OpenElementId**](OpenElementId.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.reference_element import ReferenceElement

# TODO update the JSON string below
json = "{}"
# create an instance of ReferenceElement from a JSON string
reference_element_instance = ReferenceElement.from_json(json)
# print the JSON string representation of the object
print(ReferenceElement.to_json())

# convert the object into a dict
reference_element_dict = reference_element_instance.to_dict()
# create an instance of ReferenceElement from a dict
reference_element_from_dict = ReferenceElement.from_dict(reference_element_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


