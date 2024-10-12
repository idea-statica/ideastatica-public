# OpenMessages

Open messages collection

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**messages** | [**List[OpenMessage]**](OpenMessage.md) | List of messages | [optional] 

## Example

```python
from ideastatica_connection_api.models.open_messages import OpenMessages

# TODO update the JSON string below
json = "{}"
# create an instance of OpenMessages from a JSON string
open_messages_instance = OpenMessages.from_json(json)
# print the JSON string representation of the object
print(OpenMessages.to_json())

# convert the object into a dict
open_messages_dict = open_messages_instance.to_dict()
# create an instance of OpenMessages from a dict
open_messages_from_dict = OpenMessages.from_dict(open_messages_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


