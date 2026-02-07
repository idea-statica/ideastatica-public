# ConnectionCheckRes


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**check_res_summary** | [**List[CheckResSummary]**](CheckResSummary.md) |  | [optional] 
**check_res_plate** | [**List[CheckResPlate]**](CheckResPlate.md) |  | [optional] 
**check_res_weld** | [**List[CheckResWeld]**](CheckResWeld.md) |  | [optional] 
**check_res_bolt** | [**List[CheckResBolt]**](CheckResBolt.md) |  | [optional] 
**check_res_anchor** | [**List[CheckResAnchor]**](CheckResAnchor.md) |  | [optional] 
**check_res_concrete_block** | [**List[CheckResConcreteBlock]**](CheckResConcreteBlock.md) |  | [optional] 
**buckling_results** | [**List[BucklingRes]**](BucklingRes.md) |  | [optional] 
**name** | **str** |  | [optional] 
**connection_id** | **str** |  | [optional] 
**id** | **int** |  | [optional] 
**messages** | [**OpenMessages**](OpenMessages.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.connection_check_res import ConnectionCheckRes

# TODO update the JSON string below
json = "{}"
# create an instance of ConnectionCheckRes from a JSON string
connection_check_res_instance = ConnectionCheckRes.from_json(json)
# print the JSON string representation of the object
print(connection_check_res_instance.to_json())

# convert the object into a dict
connection_check_res_dict = connection_check_res_instance.to_dict()
# create an instance of ConnectionCheckRes from a dict
connection_check_res_from_dict = ConnectionCheckRes.from_dict(connection_check_res_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


