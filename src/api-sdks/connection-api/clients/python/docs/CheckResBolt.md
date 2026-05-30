# CheckResBolt

Check value for Bolts.    <strong>Bolt identification:</strong> Bolt identifiers (used as dictionary keys in raw CBFEM results  and reflected in IdeaRS.OpenModel.Connection.CheckResBolt.Name) are opaque internal solver identifiers. They may start at any number,  are not necessarily sequential, and may contain gaps. When a bolt group is exploded or bolt positions  are modified, the identifiers may shift. Do not perform arithmetic on bolt identifiers or assume  they correspond to a zero-based or one-based index. To map bolts to sequential positions,  sort the bolt keys numerically and use the resulting order.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** | Name of the bolt (e.g., \&quot;B15\&quot;). The numeric suffix is an opaque CBFEM solver identifier —  it may not start at 1 and may not be sequential. See IdeaRS.OpenModel.Connection.CheckResBolt remarks for details. | [optional] 
**unity_check** | **float** | Unity Check | [optional] 
**check_status** | **bool** | Status of the Check | [optional] 

## Example

```python
from ideastatica_connection_api.models.check_res_bolt import CheckResBolt

# TODO update the JSON string below
json = "{}"
# create an instance of CheckResBolt from a JSON string
check_res_bolt_instance = CheckResBolt.from_json(json)
# print the JSON string representation of the object
print(check_res_bolt_instance.to_json())

# convert the object into a dict
check_res_bolt_dict = check_res_bolt_instance.to_dict()
# create an instance of CheckResBolt from a dict
check_res_bolt_from_dict = CheckResBolt.from_dict(check_res_bolt_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


