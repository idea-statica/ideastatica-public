# IdeaStatiCa.ConnectionApi.Model.ConCrossSectionCustomComponentOutlineInner
Polymorphic root. Every element on the wire is one of the concrete subtypes listed in the discriminator mapping and carries the $type discriminator; $type is deliberately declared on each subtype schema (with its exact wire value as default) rather than here.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**SegmentType** | **string** |  | [optional] [readonly] 
**Start** | [**ConCssPoint2D**](ConCssPoint2D.md) |  | [optional] 
**End** | [**ConCssPoint2D**](ConCssPoint2D.md) |  | [optional] 
**Mid** | [**ConCssPoint2D**](ConCssPoint2D.md) |  | [optional] 
**Type** | **string** |  | [optional] [default to "IdeaStatiCa.Api.Connection.Model.Material.ConCssArcSegment, IdeaStatiCa.Api"]

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

