# ConCutOperation


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**member** | [**ConCutMemberTarget**](ConCutMemberTarget.md) |  | [optional] 
**cut_by** | [**ConCutByTarget**](ConCutByTarget.md) |  | [optional] 
**cutting_method** | [**ConCuttingMethod**](ConCuttingMethod.md) |  | [optional] 
**extend_member** | **bool** |  | [optional] 
**cutting_plane** | [**ConCuttingPlane**](ConCuttingPlane.md) |  | [optional] 
**direction** | [**ConCuttingDirection**](ConCuttingDirection.md) |  | [optional] 
**offset** | **float** |  | [optional] 
**webs_weld** | [**ConWeldData**](ConWeldData.md) |  | [optional] 
**flanges_weld** | [**ConWeldData**](ConWeldData.md) |  | [optional] 
**is_imported** | **bool** |  | [optional] 
**operation_type** | **str** |  | [optional] 
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**active** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_cut_operation import ConCutOperation

# TODO update the JSON string below
json = "{}"
# create an instance of ConCutOperation from a JSON string
con_cut_operation_instance = ConCutOperation.from_json(json)
# print the JSON string representation of the object
print(con_cut_operation_instance.to_json())

# convert the object into a dict
con_cut_operation_dict = con_cut_operation_instance.to_dict()
# create an instance of ConCutOperation from a dict
con_cut_operation_from_dict = ConCutOperation.from_dict(con_cut_operation_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


