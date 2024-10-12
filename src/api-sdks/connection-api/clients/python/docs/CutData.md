# CutData

Provides data of the cut beam

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**plane_point** | [**Point3D**](Point3D.md) |  | [optional] 
**normal_vector** | [**Vector3D**](Vector3D.md) |  | [optional] 
**direction** | [**CutOrientation**](CutOrientation.md) |  | [optional] 
**offset** | **float** | Offset - shift of cut | [optional] 

## Example

```python
from ideastatica_connection_api.models.cut_data import CutData

# TODO update the JSON string below
json = "{}"
# create an instance of CutData from a JSON string
cut_data_instance = CutData.from_json(json)
# print the JSON string representation of the object
print(CutData.to_json())

# convert the object into a dict
cut_data_dict = cut_data_instance.to_dict()
# create an instance of CutData from a dict
cut_data_from_dict = CutData.from_dict(cut_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


