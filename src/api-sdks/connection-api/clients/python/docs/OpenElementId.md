# OpenElementId

Open element base class  POS - class can not be abstract -it causes problems with serialization

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_connection_api.models.open_element_id import OpenElementId

# TODO update the JSON string below
json = "{}"
# create an instance of OpenElementId from a JSON string
open_element_id_instance = OpenElementId.from_json(json)
# print the JSON string representation of the object
print(OpenElementId.to_json())

# convert the object into a dict
open_element_id_dict = open_element_id_instance.to_dict()
# create an instance of OpenElementId from a dict
open_element_id_from_dict = OpenElementId.from_dict(open_element_id_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


