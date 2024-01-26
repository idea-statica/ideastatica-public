# WARNING
# DO NOT MODIFY THIS FILE. IT IS USED TO GENERATE DOCUMENTATION for doc/api/rcs/rcs_api_client_basics.md
# WARNING

from ideastatica_rcs_client import idea_statica_setup
from ideastatica_rcs_client import ideastatica_rcs_client
import os

ideaStatiCa_Version = r'23.1'

# Use the avaliable set-up tools to automatically find the IDEA StatiCa install path
# Alternatively we can provide a direct path
ideaSetupDir = idea_statica_setup.get_ideasetup_path(ideaStatiCa_Version)

#Print found setup directory
print(ideaSetupDir)

#Use set-up tools to dind an avaliable port on the local machine to run communication 
freeTcp = idea_statica_setup.get_free_port()
print(freeTcp)

#Create the client and establish communication with RCS.
rcsClient = ideastatica_rcs_client.ideastatica_rcs_client(ideaSetupDir, freeTcp)

#We can retrieve and print the details of the RCS communication. 
print(rcsClient.printServiceDetails())

try:
    # Open Project Options - Choose 1
    from_iom = False
    
    if from_iom:
        #Get path of an existing RCS project on disk.
        rcs_project_file_path = "ProjectPath.IdeaRcs"
        #Use the RCS API Client to open/load the project.
        projectId = rcsClient.OpenProject(rcs_project_file_path)
    else:
        #Create RCS project from IOM filepath.
        rcs_iom_file_path = "OpenModelPath.xml"
        #Use the RCS API Client to create a project from a IOM file on disc.
        projectId = rcsClient.OpenProjectFromIOMFile(rcs_iom_file_path)
    
    #print the Id of the avaliable project.
    print(projectId)

    #INSERT PROJECT OPERATIONS
    #~Code for project operations
    #INSERT PROJECT OPERATIONS

except Exception as e:
    message  = str(e)
    print(f"An error occurred: {message}")
    exit(1)

finally:
    if not rcsClient is None:
        del rcsClient

