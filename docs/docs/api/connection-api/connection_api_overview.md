# Connection API

Welcome to the IDEA StatiCa Connection API (Application Programming Interface).

The Connection API can be used to interact with IDEA StatiCa Connection to create and optimize steel connection designs, plus many other things.

> [!IMPORTANT]
> As of version **24.1** a new Connection API is avaliable. This new API **replaces** the older ConnHiddenCalculator API which was avaliable in IdeaStatiCa.Plugin.

## API Architecture

The Connection API is built on REST OpenAPI architecture and runs over a http protocal. The current version of the API creates REST server which is hosted locally on a users computer. In the future we would also like provide the possibility to run calculations on remote machines.

Users can interact with the Connection API using one of the provided clients or by calling the REST API directly from any programming language. We recommend using one of the provided IDEA StatiCa clients for **.Net** or **Python**.