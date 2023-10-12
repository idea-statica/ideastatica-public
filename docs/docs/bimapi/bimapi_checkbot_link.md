## Background

Checkbot is the preferred way to link external third-party applications and allows advanced model data/project management including model syncing, persistent property conversion, and more. 

To enable a consistent approach to the creation of BIM Links IDEA StatiCa provides a flexible project framework that allows developers to create and distribute their own BIM Links for a third-party application. 

The following provides information about whether an application is applicable to the Checkbot workflow, the basic features and framework of the application.

1. [Linking an Application with Checkbot](bimapi_linking_an_application_with_checkbot.md) - Understand how you can connect with Checkbot.
2. [Checkbot BIM Link Project Framework](bimapi_checkbot_project_framework.md) - Understand how some of the major Checkbot features work and can be implemented within your project.

## Example Projects

IDEA provides open-source example projects to show how different links are created and integrated with the BimApi:

Link | Type | Description | Link 
---------|----------|---------|---------
RSTAB Link | FEA | The Rstab link provides the source code of an FEA BIMLink between Rstab and Checkbot. The link is installed to Rstab for execution.  | [view](https://github.com/idea-statica/ideastatica-public/tree/main/src/bim-links/rstab/IdeaRstabPlugin)
RAM SS Link | FEA | The RAM Structural Systems link provides the source code of Importing a RAM Structural System Database file into the standalone version of Checkbot. This is a simpler version of a typical BIMLink as it only supports a one-time import of the RAM DB.  | [view](https://github.com/idea-statica/ideastatica-public/tree/main/src/bim-links/bentley-ram/IdeaStatiCa.RamToIdea)