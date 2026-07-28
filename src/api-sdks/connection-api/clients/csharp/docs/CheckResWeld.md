# IdeaStatiCa.ConnectionApi.Model.CheckResWeld
Check value for Weld

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Name** | **string** | Name of Weld | [optional] 
**Id** | **int** | Unique id of weld | [optional] 
**UnityCheck** | **double** | Unity Check Stress. NaN when the weld has no computed stress utilisation,  see IdeaRS.OpenModel.Connection.CheckResWeld.IsNotStressRated | [optional] 
**CheckStatus** | **bool** | Status of the Check | [optional] 
**IsNotStressRated** | **bool** | True when the check does not rate this weld by a stress utilisation, so its check is  satisfied by definition. Set for butt/bevel welds (e.g. CJP) and for any weld placed  edge-to-edge - note the latter includes fillet welds, which are not full strength, so  this flag is not a statement about the weld developing the capacity of the connected  plates. IdeaRS.OpenModel.Connection.CheckResWeld.UnityCheck is NaN and IdeaRS.OpenModel.Connection.CheckResWeld.CheckStatus is true in that case | [optional] 
**LoadCaseId** | **int** | Id of Load Case | [optional] 
**Items** | **List&lt;int&gt;** | In case of presentation of groups plates (uncoiled beams) | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

