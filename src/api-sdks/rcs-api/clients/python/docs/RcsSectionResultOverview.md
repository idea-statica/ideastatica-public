# RcsSectionResultOverview


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**section_id** | **int** |  | [optional] 
**overall_items** | [**List[ConcreteCheckResultOverallItem]**](ConcreteCheckResultOverallItem.md) |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_section_result_overview import RcsSectionResultOverview

# TODO update the JSON string below
json = "{}"
# create an instance of RcsSectionResultOverview from a JSON string
rcs_section_result_overview_instance = RcsSectionResultOverview.from_json(json)
# print the JSON string representation of the object
print(RcsSectionResultOverview.to_json())

# convert the object into a dict
rcs_section_result_overview_dict = rcs_section_result_overview_instance.to_dict()
# create an instance of RcsSectionResultOverview from a dict
rcs_section_result_overview_from_dict = RcsSectionResultOverview.from_dict(rcs_section_result_overview_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


