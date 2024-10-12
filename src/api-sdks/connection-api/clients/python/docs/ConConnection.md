# ConConnection


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**identifier** | **str** |  | [optional] 
**name** | **str** |  | [optional] 
**description** | **str** |  | [optional] 
**analysis_type** | [**ConAnalysisTypeEnum**](ConAnalysisTypeEnum.md) |  | [optional] 
**bearing_member_id** | **int** |  | [optional] 
**is_calculated** | **bool** |  | [optional] [readonly] 

## Example

```python
from ideastatica_connection_api.models.con_connection import ConConnection

# TODO update the JSON string below
json = "{}"
# create an instance of ConConnection from a JSON string
con_connection_instance = ConConnection.from_json(json)
# print the JSON string representation of the object
print(ConConnection.to_json())

# convert the object into a dict
con_connection_dict = con_connection_instance.to_dict()
# create an instance of ConConnection from a dict
con_connection_from_dict = ConConnection.from_dict(con_connection_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


