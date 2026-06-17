# IdeaStatiCa.ConnectionApi.Model.CheckResBolt
Check value for Bolts.    <strong>Bolt identification:</strong> Bolt identifiers (used as dictionary keys in raw CBFEM results  and reflected in IdeaRS.OpenModel.Connection.CheckResBolt.Name) are opaque internal solver identifiers. They may start at any number,  are not necessarily sequential, and may contain gaps. When a bolt group is exploded or bolt positions  are modified, the identifiers may shift. Do not perform arithmetic on bolt identifiers or assume  they correspond to a zero-based or one-based index. To map bolts to sequential positions,  sort the bolt keys numerically and use the resulting order.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Name** | **string** | Name of the bolt (e.g., \&quot;B15\&quot;). The numeric suffix is an opaque CBFEM solver identifier —  it may not start at 1 and may not be sequential. See IdeaRS.OpenModel.Connection.CheckResBolt remarks for details. | [optional] 
**UnityCheck** | **double** | Unity Check | [optional] 
**CheckStatus** | **bool** | Status of the Check | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

