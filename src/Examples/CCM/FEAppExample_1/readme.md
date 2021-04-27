 ### FEAppExample_1
 shows how to connect IDEA CCM (IDEA Code Check Manager) to any FEA or CAD application. It uses [IdeaStatiCa.Plugin](https://github.com/idea-statica/ideastatica-plugin) for communication with *IdeaCodeCheck.exe*.

### ![CCM + FakeFEA](../Images/fake-fea.png?raw=true)

It is requiered :
<ol>
  <li>To have IdeaStatica v20.1 installed on the PC. Free trial version version can be obtained [here](https://www.ideastatica.com/free-trial).</li>
  <li>To set the path to IdeaStatiCa Code Check Manager (IdeaCodeCheck.exe) in the project settings (or in the config file)</li>
</ol>


In the file *FEAppExample_1.exe.config* should be the correct path

```xml
        <FEAppExample_1.Properties.Settings>
            <setting name="IdeaStatiCaDir" serializeAs="String">
                <value>C:\Program Files\IDEA StatiCa\StatiCa 20.1\IdeaCodeCheck.exe</value>
          </setting>
```

When *Some FE Application* starts model should be created - it can be done creating default model - see the button **Default** or be opening model from xml - see the button **Load model**. IDEA CCM opens by clicking on the button **Run IDEA StatiCa CCM**.
It is possible to get all materials and cross-sections in an open project as well as in IDEA StatiCa MPRL.

In file [member_project.xml](FEA_Model_Examples/member_project.xml) there is example how to import member into CCM.

### ![Member in CCM](../Images/member-project.png?raw=true)

In the test application do following :
* Set path to _IdeaCodeCheck.exe_
* Build the project _FEAppExample_1.csproj_
* Run _FEAppExample_1.exe_
* Load IOM model from the file _member_project.xml_
* Click on the button _Run IDEA StatiCa CCM_
* Create a new project - data will be stored in the folder _C:\Users\YOUR-USER-NAME\Documents\FEAppExample_1\member_project\IdeaStatiCa-member_project_
* Import a member from IOM model to the CCM project by clicking on the button **Member**
* In CCM configure the connections #2 and #3
* Configure the memeber #2
* Open the column #2 in the application IDEA StatiCa Member by clicking on the button **Open**


All examples of FEA ModelBIM Models can be found in folder [FEA Examples](FEA_Model_Examples/)

### Improvements in IDEA StatiCa v 20.1

BIM plugin can work as CAD application and provide data about a geometry of a connection. It enables to export connections from BIM applications together with their geometrical data. In the example FEAppExample_1
 you must check the check box **Is CAD** then open a model from [CAD ModelBIM examples](CAD_Model_Examples).

#### Column-On-BasePlate

![Column-On-BasePlate](../Images/column-on-baseplate.png?raw=true)

[ModelBIM for Column-On-BasePlate](CAD_Model_Examples/Column-On-BasePlate.xml)

#### [Column-CHS-EndPlates

![Column-CHS-EndPlates](../Images/column-chs-endplates.png?raw=true)

[ModelBIM for Column-CHS-EndPlates](CAD_Model_Examples/Column-CHS-EndPlates.xml)

#### Cleat connection between the beam and the column

![Cleat connection between the beam and the column](../Images/cleats-as.png?raw=true)

![Imported cleats to Idea ](../Images/cleats-ideacon.png?raw=true)

[The original model created in AUTO Desk Advance Steel 2019](CAD_Model_Examples/cleats_beam_to_column.dwg?raw=true)

[ModelBIM for the cleat connection between the beam and the column](CAD_Model_Examples/cleats-beam-to-column.xml)

### IDEA CCM diagnostics
There are new diagnostics features in IdeaStatiCa which are available from version v 20.1.4367.1 (release on Dec 17, 2020)
More details about configuration of the [diagnostics](../../../../docs/ccm-diagnostics.md)
![CCM Diagnostics](../../../../Images/ccm-diagnostics.png)

### FAQ related to [ModelBIM](../../IdeaStatiCa.Plugin/ModelBIM.cs)
* The list **Items** contains only nodes in which imported connections exist.
* For each newly created **ConnectionPoint** the property **ProjectFileName** must be set - e.g. _<ProjectFileName>Connections\conn-1.ideaCon</ProjectFileName>_
* Section **Messages** must be defined in ModelBIM (xml file) - there is a bug which causes crashes in CCM - it will be fixed in the next version
