# ConWeldData


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**kind** | [**ConWeldDataKind**](ConWeldDataKind.md) |  | [optional] 
**size** | **float** |  | [optional] 
**type** | [**ConWeldType**](ConWeldType.md) |  | [optional] 
**material_id** | **int** |  | [optional] 
**begin_offset** | **float** |  | [optional] 
**end_offset** | **float** |  | [optional] 
**length** | **float** |  | [optional] 
**gap** | **float** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_weld_data import ConWeldData

# TODO update the JSON string below
json = "{}"
# create an instance of ConWeldData from a JSON string
con_weld_data_instance = ConWeldData.from_json(json)
# print the JSON string representation of the object
print(con_weld_data_instance.to_json())

# convert the object into a dict
con_weld_data_dict = con_weld_data_instance.to_dict()
# create an instance of ConWeldData from a dict
con_weld_data_from_dict = ConWeldData.from_dict(con_weld_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


