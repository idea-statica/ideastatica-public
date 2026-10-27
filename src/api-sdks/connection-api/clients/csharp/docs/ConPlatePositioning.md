# IdeaStatiCa.ConnectionApi.Model.ConPlatePositioning

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**ConPlatePositioningType** | **ConPlatePositioningEnum** |  | [optional] 
**ConcreteBlockIndex** | **int** | Used when conPlatePositioningType is concreteBlock. Despite the name, not a block index: the id of the operation that creates the concrete block, i.e. an anchor grid or contact grid whose blockType is new (list them with GET .../operations/anchor-grid and .../operations/contact-grid). The operation must come before the plate, i.e. have a lower id. | [optional] 
**ConcreteSurface** | **int** | Used when conPlatePositioningType is concreteBlock. The face of the block the plate is placed on, 1 to 6 for a rectangular block: 1 to 4 are the sides, 5 is the bottom and 6 is the top. | [optional] 
**PositionX** | **double** |  | [optional] 
**PositionY** | **double** |  | [optional] 
**RotationAngle** | **double** |  | [optional] 
**CoordinateSystem** | [**ConLocalCoordinateSystem**](ConLocalCoordinateSystem.md) |  | [optional] 
**Offset** | [**Point3D**](Point3D.md) |  | [optional] 
**Rotation** | [**Point3D**](Point3D.md) |  | [optional] 
**OnMemberId** | **int** |  | [optional] 
**OnMemberOperationId** | **int?** |  | [optional] 
**InputMethod** | **ConWorkPlaneMethod** |  | [optional] 
**NormalVector** | [**Vector3D**](Vector3D.md) |  | [optional] 
**PlateOnMemberIndex** | **int** |  | [optional] 
**Function** | **ConPlateFunction** |  | [optional] 
**Location** | **ConLocation** |  | [optional] 
**Pitch** | **double** |  | [optional] 
**PlateIndex** | **int** |  | [optional] 
**OperationId** | **int?** |  | [optional] 
**PlateEdgeIndex** | **int** |  | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

