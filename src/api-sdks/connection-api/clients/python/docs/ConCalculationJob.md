# ConCalculationJob


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**job_id** | **str** |  | [optional] 
**project_id** | **str** |  | [optional] 
**status** | [**ConCalculationJobStatusEnum**](ConCalculationJobStatusEnum.md) |  | [optional] 
**connection_ids** | **List[int]** |  | [optional] 
**connections_completed** | **int** |  | [optional] 
**current_connection_id** | **int** |  | [optional] 
**percent** | **float** |  | [optional] 
**message** | **str** |  | [optional] 
**created_at** | **datetime** |  | [optional] 
**finished_at** | **datetime** |  | [optional] 
**results** | [**List[ConResultSummary]**](ConResultSummary.md) |  | [optional] 
**error** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_calculation_job import ConCalculationJob

# TODO update the JSON string below
json = "{}"
# create an instance of ConCalculationJob from a JSON string
con_calculation_job_instance = ConCalculationJob.from_json(json)
# print the JSON string representation of the object
print(con_calculation_job_instance.to_json())

# convert the object into a dict
con_calculation_job_dict = con_calculation_job_instance.to_dict()
# create an instance of ConCalculationJob from a dict
con_calculation_job_from_dict = ConCalculationJob.from_dict(con_calculation_job_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


