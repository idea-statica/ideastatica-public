# TemperatureCurve2D

Reperesents a thermal curve.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**points** | [**List[Point2D]**](Point2D.md) | List of polygon points | [optional] 

## Example

```python
from ideastatica_rcs_api.models.temperature_curve2_d import TemperatureCurve2D

# TODO update the JSON string below
json = "{}"
# create an instance of TemperatureCurve2D from a JSON string
temperature_curve2_d_instance = TemperatureCurve2D.from_json(json)
# print the JSON string representation of the object
print(TemperatureCurve2D.to_json())

# convert the object into a dict
temperature_curve2_d_dict = temperature_curve2_d_instance.to_dict()
# create an instance of TemperatureCurve2D from a dict
temperature_curve2_d_from_dict = TemperatureCurve2D.from_dict(temperature_curve2_d_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


