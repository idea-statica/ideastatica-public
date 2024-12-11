# ISDModel

Model of IDEA StatiCa Detail

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.isd_model import ISDModel

# TODO update the JSON string below
json = "{}"
# create an instance of ISDModel from a JSON string
isd_model_instance = ISDModel.from_json(json)
# print the JSON string representation of the object
print(ISDModel.to_json())

# convert the object into a dict
isd_model_dict = isd_model_instance.to_dict()
# create an instance of ISDModel from a dict
isd_model_from_dict = ISDModel.from_dict(isd_model_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


