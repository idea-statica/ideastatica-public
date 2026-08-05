# Grasshopper Example Files

Download links for each example can be found in the brief descriptions below.

- **Creating a Project from IOM**\
  Opens an IOM project, creates a connection from it, calculates it, and saves the project.\
  [Download Example 01](../../../../examples/extensions/grasshopper/Example_01-Open_IOM_and_calculate.zip)

- **Parameter Sweep on a Connection Using Colibri**\
  Opens a project and runs a parameter sweep on selected parameters using Colibri, writing the results to a CSV file.\
  [Download Example 02](../../../../examples/extensions/grasshopper/Example_02-Parameter_sweep_colibri.zip)

- **Applying a Template and Iterating a Parameter Using Anemone**\
  Applies a parametric template to an empty connection and iterates a parameter until the design check passes.\
  [Download Example 03](../../../../examples/extensions/grasshopper/Example_03-Apply_template+parameter_sweep_anemone.zip)

- **Importing Geometry from Rhino and Creating a Connection Using IOM**\
  Loads plate geometry and member axes from Rhino, trims the geometry using IOM, and creates a connection.\
  [Download Example 04](../../../../examples/extensions/grasshopper/Example_04-Import_plates.zip)

- **Cost Estimation for Various Connection Configurations**\
  A connection with an applied parametric template is iterated over selected parameters using Colibri, and the estimated cost is recorded for each iteration. A calculation step is also included, recording the maximum unity check — this is disabled by default due to longer computation time.\
  [Download Example 05](../../../../examples/extensions/grasshopper/Example_05-Cost_estimation.zip)

- **Bulk Assessment of Similar Connections**\
  A template with configurable parameters is applied to selected nodes of an IOM model, followed by assessment and graphical visualization of the check results.\
  [Download Example 06](../../../../examples/extensions/grasshopper/Example_06-Bulk_calculation.zip)

- **Propose Template from the Template Database**\
  For a project with multiple connections, a template is selected from the database based on configured parameters. The selection is visualized graphically in Rhino. An assessment can then be performed, with results displayed graphically on the selected templates.\
  [Download Example 07](../../../../examples/extensions/grasshopper/Example_07-Propose-template.zip)

- **Building Structured Brep Geometry from IFC**\
  Shows what you can do with the output of the IDEA StatiCa Grasshopper components using your own native C# scripting component. It takes the output of the Export IFC component and builds structured brep geometry (beams, plates, bolts, and welds), each enriched with the element names and descriptions carried in the IFC.\
  [Download Example 08](../../../../examples/extensions/grasshopper/Example_08-IFC_to_brep.zip)

## Version Overview

| Example | IDEA StatiCa | Grasshopper plugin |
| --- | --- | --- |
| 01 – Creating a Project from IOM | 26.0.2 | 26.0.2 |
| 02 – Parameter Sweep Using Colibri | 26.0.2 | 26.0.2 |
| 03 – Applying a Template and Iterating Using Anemone | 26.0.2 | 26.0.2 |
| 04 – Importing Geometry from Rhino | 26.0.2 | 26.0.2 |
| 05 – Cost Estimation | 26.0.2 | 26.0.2 |
| 06 – Bulk Assessment | 26.0.2 | 26.0.2 |
| 07 – Propose Template | 26.0.2 | 26.0.2 |
| 08 – Building Structured Brep Geometry from IFC | 26.0.3 | 26.0.2 |
