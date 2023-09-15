IDEA StatiCa provides a flexible framework for third-party developers to implement advanced Links with a wide range of FEA and CAD applications. 

The below briefly explains the typical framework of a Checkbot BIM Link as well as some of the features that are available in the current Project framework. 

The support-center article [Checkbot bulk BIM Workflows](https://www.ideastatica.com/support-center/checkbot-bulk-bim-workflows) explains some of these features.

# General

The typical (and preferred) framework for a BIM Link with Checkbot is to enable execution from the base application. This is typically in the form of a button or command from the application.

On execution of the command, the Checkbot application will start. A new folder located in the application file directory is created to hold the connection/member files which are generated. If an existing folder already exists, then that project is loaded back into checkbot so the session can continue.

As a general rule, the folder that is specified should be generated in a folder where user permission is granted.

# Object Selection

Once Checkbot is opened, the user should select which type of object (nodes or members) they would like to select. 

If a node is selected, the program will collect all the members connected to that node in order to create the connection. If a member is selected, the program will import only the common nodes of the selected members. Once a selection is confirmed the members and connections are created in IDEAStatica checkbot. At that point, a persistence file in JSON format is written to the folder. 

Object selection can therefore be executed again at any time to allow the import of more of the structure. The previous object selection should be retained, and the new items merged with the existing selection. This allows for Checkbot to confirm that duplicate members do not get created when importing additional members or joints into Checkbot.

To enable object selection, it is required that current selection can be retrieved from the base application. If not, the entire model may need to be sent and selection neglected. 

# Sync

Syncing of the Structural model allows updates to the base application model to be synced with the Checkbot project. For example, if a member size is changed in the base application syncing allows for the checkbot application model to be updated. 

In BIM applications this also applies to connection items such as bolt placements.

### Syncing Limitations 

* The framework does not provide updating of the base model from changes made in Checkbot or the Connection Files.

# Import Session and Persistence



# Material and Property Conversion

Checkbot allows for materials and properties (sections, welds, bolt assembly etc.) to be imported from external programs. The developer where possible should decide to create native IOM materials and properties that will be directly created in the Checkbot database. However, in some instances, this is not possible, and the Material or Property can be added ‘By Name’. By default, the IDEA StatiCa library will be searched and if not found will be provided the item to the conversion tab of Checkbot. Refer below for how checkbot manages user-selected conversion. 

Refer to the IOM Material and IOM Section Property Conversion sections for more info.

## Conversion Files

//How are conversion files stored and saved?

Conversion files are persistent across the checkbot program and stored locally on a user's machine. **This is managed by the checkbot application.**

Each code has a separate file.

# Logging and Debugging

//To-do
