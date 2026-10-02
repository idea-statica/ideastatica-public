# ConPlateCutOperation


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**modified_object** | [**ConCutByTarget**](ConCutByTarget.md) |  | [optional] 
**cut_by** | [**ConCutByTarget**](ConCutByTarget.md) |  | [optional] 
**cutting_method** | [**ConCuttingMethod**](ConCuttingMethod.md) |  | [optional] 
**remaining_part** | [**ConPlateSide**](ConPlateSide.md) |  | [optional] 
**offset** | **float** |  | [optional] 
**weld** | [**ConWeldData**](ConWeldData.md) |  | [optional] 
**is_imported** | **bool** |  | [optional] 
**operation_type** | **str** |  | [optional] 
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**active** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_plate_cut_operation import ConPlateCutOperation

# TODO update the JSON string below
json = "{}"
# create an instance of ConPlateCutOperation from a JSON string
con_plate_cut_operation_instance = ConPlateCutOperation.from_json(json)
# print the JSON string representation of the object
print(con_plate_cut_operation_instance.to_json())

# convert the object into a dict
con_plate_cut_operation_dict = con_plate_cut_operation_instance.to_dict()
# create an instance of ConPlateCutOperation from a dict
con_plate_cut_operation_from_dict = ConPlateCutOperation.from_dict(con_plate_cut_operation_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


