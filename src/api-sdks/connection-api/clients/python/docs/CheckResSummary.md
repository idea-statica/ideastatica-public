# CheckResSummary

Check summary

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**check_value** | **float** | Check value | [optional] 
**check_status** | **bool** | Status of check | [optional] 
**load_case_id** | **int** | Id of Load Case | [optional] 
**name** | **str** | Name | [optional] 
**unity_check_message** | **str** | Detail message about overall check | [optional] 
**skipped** | **bool** | Whether the check was calculated or not.  If true, the check was not calculated and IdeaRS.OpenModel.Connection.CheckResSummary.CheckValue should be ignored, otherwise false. | [optional] 

## Example

```python
from ideastatica_connection_api.models.check_res_summary import CheckResSummary

# TODO update the JSON string below
json = "{}"
# create an instance of CheckResSummary from a JSON string
check_res_summary_instance = CheckResSummary.from_json(json)
# print the JSON string representation of the object
print(CheckResSummary.to_json())

# convert the object into a dict
check_res_summary_dict = check_res_summary_instance.to_dict()
# create an instance of CheckResSummary from a dict
check_res_summary_from_dict = CheckResSummary.from_dict(check_res_summary_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


