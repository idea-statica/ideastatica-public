# IdeaStatiCa.ConnectionApi.Model.CheckResWeld
Check value for Weld

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Name** | **string** | Name of Weld | [optional] 
**Id** | **int** | Unique id of weld | [optional] 
**UnityCheck** | **double** | Unity Check Stress. NaN when the weld has no computed stress utilisation -  full-strength welds are not stress-checked, see IdeaRS.OpenModel.Connection.CheckResWeld.IsFullStrength | [optional] 
**CheckStatus** | **bool** | Status of the Check | [optional] 
**IsFullStrength** | **bool** | True when the weld is not rated by a stress utilisation and its check is satisfied by definition -  the check treats it as a full-strength weld. This applies to butt/bevel welds (e.g. CJP) and to  welds placed edge-to-edge. IdeaRS.OpenModel.Connection.CheckResWeld.UnityCheck is NaN and IdeaRS.OpenModel.Connection.CheckResWeld.CheckStatus is true  in that case | [optional] 
**LoadCaseId** | **int** | Id of Load Case | [optional] 
**Items** | **List&lt;int&gt;** | In case of presentation of groups plates (uncoiled beams) | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

