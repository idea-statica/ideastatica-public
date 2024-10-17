# BucklingRes

Results of the buckling analysis

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**load_case_id** | **int** |  | [optional] 
**shape** | **int** | Shape lc calculated by solver | [optional] 
**factor** | **float** | Buckling factor | [optional] 

## Example

```python
from ideastatica_connection_api.models.buckling_res import BucklingRes

# TODO update the JSON string below
json = "{}"
# create an instance of BucklingRes from a JSON string
buckling_res_instance = BucklingRes.from_json(json)
# print the JSON string representation of the object
print(BucklingRes.to_json())

# convert the object into a dict
buckling_res_dict = buckling_res_instance.to_dict()
# create an instance of BucklingRes from a dict
buckling_res_from_dict = BucklingRes.from_dict(buckling_res_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


