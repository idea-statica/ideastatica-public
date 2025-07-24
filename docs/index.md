# Welcome to IDEA StatiCa Developer Site

The following should appeal to Software Developers and Computational Designers wishing to interface with IDEA StatiCa applications and services. The documentation is split into the below primary parts:

## Design API's (API)

IDEA StatiCa provides a number of general API's for the different apps and services that we provide. If you are a computational designer or software developer looking to perform automated structural design and optimisation tasks, this is a good place to start. 

Design API's are primarily targeted toward both Python and C#/.Net users. 

We provide an API for the following Applications:

- [Connection API](docs/api/connection-api/connection_api_overview.md)
- [RCS API](docs/api/rcs-api/rcs_api_getting_started.md)

## BIM Links with Checkbot (BIM API)

If you are a third-party software developer looking to create a seamless integration with IDEA StatiCa Checkbot then the BIM API is where you should start. The BIM Api and BIM Api Link framework allow third-parties to create customised BIM Links with in-built features such as library conversions, model syncing and more. 

Get Started here: 
- [BIM API](docs/bimapi/bimapi_checkbot_link.md)

## IDEA Open Model (IOM)

IDEA StatiCa's object model for exchanging data to and from FEA or CAD applications. IOM is open source and transfered using an XML file format. It is designed to interact natively with IDEA StatiCa applications. 

IOM is at the heart of all of our interoperability tools, therefore almost all advanced users will likely need to know the basics of Open Model. 

Get started here: 
-   [IDEA Open Model (IOM)](docs/iom/iom_getting_started.md) 


## Extensions

Extensions are plugins and programs which are built on-top of the IDEA Open Model and API's and target low-code users. If you are a structural engineer or computational designer looking to automate workflows through visual programming then this will interest you.

Our current extension are:

* [Plugin for Rhino/Grasshopper](docs/extensions/grasshopper/grasshopper_overview.md)