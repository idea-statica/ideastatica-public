# OpenMessage

Open message base class

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**number** | [**MessageNumber**](MessageNumber.md) |  | [optional] 
**description** | **str** | Description of message | [optional] 

## Example

```python
from ideastatica_connection_api.models.open_message import OpenMessage

# TODO update the JSON string below
json = "{}"
# create an instance of OpenMessage from a JSON string
open_message_instance = OpenMessage.from_json(json)
# print the JSON string representation of the object
print(OpenMessage.to_json())

# convert the object into a dict
open_message_dict = open_message_instance.to_dict()
# create an instance of OpenMessage from a dict
open_message_from_dict = OpenMessage.from_dict(open_message_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


