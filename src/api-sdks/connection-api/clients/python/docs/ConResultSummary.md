# ConResultSummary


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**passed** | **bool** |  | [optional] 
**result_summary** | [**List[CheckResSummary]**](CheckResSummary.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_result_summary import ConResultSummary

# TODO update the JSON string below
json = "{}"
# create an instance of ConResultSummary from a JSON string
con_result_summary_instance = ConResultSummary.from_json(json)
# print the JSON string representation of the object
print(ConResultSummary.to_json())

# convert the object into a dict
con_result_summary_dict = con_result_summary_instance.to_dict()
# create an instance of ConResultSummary from a dict
con_result_summary_from_dict = ConResultSummary.from_dict(con_result_summary_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


