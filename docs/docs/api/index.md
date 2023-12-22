# IDEA StatiCa Design API's

IDEA StatiCa provides API's (Application Programing Interfaces) for some of our applications to allow automated usage of the program through code.

These can proivde many benefits for automation, whether a simple script or a more thought out application that you or your company can distribute to a wider community.

## IOM vs API

It is important to understand the difference between IOM and API and there roles in the IDEA StatiCa design workflow.

IOM is the an open source model definition that can be created and transfered (by .xml) between parties. API's are used to interact directly with a *Live* connection between a design application. Below are some of the other highlighted differences between IDEA Open Model (IOM) and API's:

* **IOM**
   * An OpenSource data model which can be transfered by XML.
   * As long as the schema is correct, anyone can create and distribute IOM for transfer of BIM or Analysis data.
   * Free to use

* **API**
    * Interacts directly via a live connection with a Design Application
    * Modifies internal data of the Design Applicaion program.
    * Requires and IDEA StatiCa license 

Alot of our API's allow the import of IDEA Open Model to 'create' application projects. For example, IOM is used to create IDEA Connection Models using its definition. First the user creates the IOM definition and then uses the API to import it. On import the IOM is converted into the internal structure of the program for users to perform more operations using the API such as calculations. 

In some instances you will likely see alot of similarties between IOM and the program inputs, however in some cases they are a little different. As our API's develop we will provide greater freedom to create App models without the need of generating IOM. 