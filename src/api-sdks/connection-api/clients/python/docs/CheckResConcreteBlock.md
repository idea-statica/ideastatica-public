# CheckResConcreteBlock

Check value for Concrete Block

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** | Name of Concrete Block | [optional] 
**unity_check** | **float** | Unity Check | [optional] 
**check_status** | **bool** | Status of the Check | [optional] 
**load_case_id** | **int** | Id of Load Case | [optional] 

## Example

```python
from ideastatica_connection_api.models.check_res_concrete_block import CheckResConcreteBlock

# TODO update the JSON string below
json = "{}"
# create an instance of CheckResConcreteBlock from a JSON string
check_res_concrete_block_instance = CheckResConcreteBlock.from_json(json)
# print the JSON string representation of the object
print(CheckResConcreteBlock.to_json())

# convert the object into a dict
check_res_concrete_block_dict = check_res_concrete_block_instance.to_dict()
# create an instance of CheckResConcreteBlock from a dict
check_res_concrete_block_from_dict = CheckResConcreteBlock.from_dict(check_res_concrete_block_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


