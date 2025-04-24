# RCS API

Welcome to the IDEA StatiCa RCS API (Application Programming Interface).

The RCS API can be used to interact with IDEA StatiCa RCS to create and optimize designs of reinforced cross-sections.

> [!IMPORTANT]
> Please review the API history below. If you are using an older version of the API it is highly reccommended to update to the latest version.

## RCS API History

* **24.1.1** - Release of a revamped REST API with new C# and python clients was released. This release alighns Connection API and RCS API terminology. The original REST API was reformatted to take into account a full redesign of the Connection API and alighn to a uniform API methodolgy accross all design apps.
* **23.1.3** - A new RCS API based on REST architecture was made avaliable. It has since been replaced with a new version.  This API should continue to run, but it is highly recommended to update to the latest version.
* **Before 23.1.3** - Before recent upgrades to a new REST API we had avaliable the RcsController. This API is no longer supported.

## API Architecture

The RCS API is built on REST Open API architecture and runs over a http protocal. The current version of the API creates REST server which is hosted locally on a users computer. However, in the future we may also provide the possibility to run calculations on remote machines.

Users can interact with the RCS API using one of the provided clients or by calling the REST API directly from any programming language. We recommend using one of the provided IDEA StatiCa wrapper clients for **.Net** or **Python**.