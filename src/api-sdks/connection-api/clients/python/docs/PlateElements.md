# PlateElements

Data of plate elements

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**plate_type** | [**StructuralPlateType**](StructuralPlateType.md) |  | [optional] 
**plate_uid** | **int** | Plate UID | [optional] 
**elements** | [**List[FemElement]**](FemElement.md) | List of fem elements for plate UID | [optional] 

## Example

```python
from ideastatica_connection_api.models.plate_elements import PlateElements

# TODO update the JSON string below
json = "{}"
# create an instance of PlateElements from a JSON string
plate_elements_instance = PlateElements.from_json(json)
# print the JSON string representation of the object
print(plate_elements_instance.to_json())

# convert the object into a dict
plate_elements_dict = plate_elements_instance.to_dict()
# create an instance of PlateElements from a dict
plate_elements_from_dict = PlateElements.from_dict(plate_elements_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


