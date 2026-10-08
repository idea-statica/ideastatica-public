# CheckResWeld

Check value for Weld

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** | Name of Weld | [optional] 
**id** | **int** | Unique id of weld | [optional] 
**unity_check** | **float** | Unity Check Stress. NaN when the weld has no computed stress utilisation -  full-strength welds are not stress-checked, see IdeaRS.OpenModel.Connection.CheckResWeld.IsFullStrength | [optional] 
**check_status** | **bool** | Status of the Check | [optional] 
**is_full_strength** | **bool** | True when the weld is not rated by a stress utilisation and its check is satisfied by definition -  the check treats it as a full-strength weld. This applies to butt/bevel welds (e.g. CJP) and to  welds placed edge-to-edge. IdeaRS.OpenModel.Connection.CheckResWeld.UnityCheck is NaN and IdeaRS.OpenModel.Connection.CheckResWeld.CheckStatus is true  in that case | [optional] 
**load_case_id** | **int** | Id of Load Case | [optional] 
**items** | **List[int]** | In case of presentation of groups plates (uncoiled beams) | [optional] 

## Example

```python
from ideastatica_connection_api.models.check_res_weld import CheckResWeld

# TODO update the JSON string below
json = "{}"
# create an instance of CheckResWeld from a JSON string
check_res_weld_instance = CheckResWeld.from_json(json)
# print the JSON string representation of the object
print(check_res_weld_instance.to_json())

# convert the object into a dict
check_res_weld_dict = check_res_weld_instance.to_dict()
# create an instance of CheckResWeld from a dict
check_res_weld_from_dict = CheckResWeld.from_dict(check_res_weld_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


