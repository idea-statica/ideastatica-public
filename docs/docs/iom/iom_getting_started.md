# Getting Started with IDEA Open Model

## What is IDEA OpenModel

## The OpenModel class

`OpenModel` is the high-level class that holds all the information required for CAD or FEA model transfer (except model results). Information is stored in Lists of separate lists for different object types.  

To start creating a Model definition, create a new OpenModel.

```
OpenModel model = new OpenModel();
```

## OpenElementId

Each class that is placed into a Type list contained in the OpenModel object inherits from the OpenModel base class object, OpenElementId.

The OpenElementId has one property 'Id' which all concrete classes inherit. This Id is an Integer and must be greater than 0 when added to the appropriate list in the OpenModel.

## Adding Elements to Open Model

If you do not care about the Id which is assigned to the OpenElementId object when adding to a list you can automate the Id selection by using the `AddObject()` method on the OpenModel object.

```csharp
int assignedId = openModel.AddObject(myIOMObject);
```
You can also get the highest Id of all objects in a particular list by using the 'GetMaxId()' method on the OpenModel object. Add 1 to get the next available in the list.

```csharp
int nextAvaliableId = openModel.GetMaxId(myIOMObject) + 1;
```

## Reference Elements

Reference Elements are used to make references between different Type lists in the Model. A classic example is a Cross-section needing to reference a Material from the Material List. 

Reference Elements can either be created by a concrete object or simply by providing the Id and Object Type.




