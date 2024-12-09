# Taper

Defines haunches (variyng cross-sections) along the member.    One IdeaRS.OpenModel.Model.Taper may be assigned to multiple <see cref=\"T:IdeaRS.OpenModel.Model.Member1D\">Members</see>.  Sections of the member not covered by a span will use the member's cross-section.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.taper import Taper

# TODO update the JSON string below
json = "{}"
# create an instance of Taper from a JSON string
taper_instance = Taper.from_json(json)
# print the JSON string representation of the object
print(Taper.to_json())

# convert the object into a dict
taper_dict = taper_instance.to_dict()
# create an instance of Taper from a dict
taper_from_dict = Taper.from_dict(taper_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


