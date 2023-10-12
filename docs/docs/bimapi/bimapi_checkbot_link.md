## Background

Checkbot is the preferred way to link external third-party applications and allows advanced model data/project management including model syncing, persistent property conversion, and more. 

To enable a consistent approach to the creation of BIM Links IDEA StatiCa provides a flexible project framework that allows developers to create and distribute their own BIM Links for a third-party application. 

The following provides information about whether an application is applicable to the Checkbot workflow, the basic features and framework of the application.

1. [Linking an Application with Checkbot](https://github.com/idea-statica/ideastatica-public/wiki/Linking-an-Application-with-Checkbot) - Understand how you can connect with Checkbot.
2. [Checkbot BIM Link Project Framework](https://github.com/idea-statica/ideastatica-public/wiki/BIM-Link-Checkbot-Project-Framework) - Understand how some of the major Checkbot features work and can be implemented within your project.

## Example Projects

Go step by step through an FEA BIM Link project. 
* [Step-by Step: Create a BIM Link Project (RSTAB-Link)](https://github.com/idea-statica/ideastatica-public/wiki/Example-Create-a-BIM-Link-Project-(RSTAB-Link)) 

IDEA provides open-source example projects to show how different links are created and integrated with the BimApi:

Link | Type | Description | Link 
---------|----------|---------|---------
RSTAB Link | FEA | The Rstab link provides the source code of an FEA BIMLink between Rstab and Checkbot. The link is installed to Rstab for execution.  | [view](https://github.com/idea-statica/ideastatica-public/tree/main/src/bim-links/rstab/IdeaRstabPlugin)
RAM SS Link | FEA | The RAM Structural Systems link provides the source code of Importing a RAM Structural System Database file into the standalone version of Checkbot. This is a simpler version of a typical BIMLink as it only supports a one-time import of the RAM DB.  | [view](https://github.com/idea-statica/ideastatica-public/tree/main/src/bim-links/bentley-ram/IdeaStatiCa.RamToIdea)

## Further Resources

The below provides in-depth documentation on how to convert Third-Party app objects to IOM and the typical issues that are faced. 

* [BimApi: Material and CrossSection Conversion](https://github.com/idea-statica/ideastatica-public/wiki/BimApi-Material-and-Cross-Sections)
* [BimApi: Dealing with Member Geometry](https://github.com/idea-statica/ideastatica-public/wiki/BimApi-Dealing-with-Member-Geometry)