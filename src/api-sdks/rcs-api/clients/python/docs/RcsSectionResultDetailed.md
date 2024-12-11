# RcsSectionResultDetailed


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**section_result** | [**SectionConcreteCheckResult**](SectionConcreteCheckResult.md) |  | [optional] 
**issues** | [**List[NonConformityIssue]**](NonConformityIssue.md) |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_section_result_detailed import RcsSectionResultDetailed

# TODO update the JSON string below
json = "{}"
# create an instance of RcsSectionResultDetailed from a JSON string
rcs_section_result_detailed_instance = RcsSectionResultDetailed.from_json(json)
# print the JSON string representation of the object
print(RcsSectionResultDetailed.to_json())

# convert the object into a dict
rcs_section_result_detailed_dict = rcs_section_result_detailed_instance.to_dict()
# create an instance of RcsSectionResultDetailed from a dict
rcs_section_result_detailed_from_dict = RcsSectionResultDetailed.from_dict(rcs_section_result_detailed_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


