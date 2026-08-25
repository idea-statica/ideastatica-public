# IdeaStatiCa.ConnectionApi.Model.ConnectionCheckRes
Results for connection in project

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**CheckResSummary** | [**List&lt;CheckResSummary&gt;**](CheckResSummary.md) | List of CheckResSummary | [optional] 
**CheckResPlate** | [**List&lt;CheckResPlate&gt;**](CheckResPlate.md) | List of check results for plates | [optional] 
**CheckResWeld** | [**List&lt;CheckResWeld&gt;**](CheckResWeld.md) | List of check results for welds | [optional] 
**CheckResBolt** | [**List&lt;CheckResBolt&gt;**](CheckResBolt.md) | List of check results for bolts | [optional] 
**CheckResAnchor** | [**List&lt;CheckResAnchor&gt;**](CheckResAnchor.md) | List of check results for anchors | [optional] 
**CheckResConcreteBlock** | [**List&lt;CheckResConcreteBlock&gt;**](CheckResConcreteBlock.md) | List of check results for concrete blocks | [optional] 
**BucklingResults** | [**List&lt;BucklingRes&gt;**](BucklingRes.md) | Results of the linear buckling analysis - one row per buckling mode and load case,  so IdeaRS.OpenModel.Connection.BucklingRes.Shape repeats for every load case. The critical buckling factor  of the connection is the minimal positive IdeaRS.OpenModel.Connection.BucklingRes.Factor in the list  (also reported as the Buckling row of IdeaRS.OpenModel.Connection.ConnectionCheckRes.CheckResSummary) | [optional] 
**Name** | **string** | Name of connection | [optional] 
**ConnectionID** | **Guid** | Guid of connection | [optional] 
**Id** | **int** | Integer Id of connection | [optional] 
**Messages** | [**OpenMessages**](OpenMessages.md) |  | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

