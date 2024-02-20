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

    # Get path of an existing RCS project on disk.
    dir_path = os.path.dirname(os.path.realpath(__file__))
    rcs_project_file_path = os.path.join(dir_path, "Project1.IdeaRcs")
    print(rcs_project_file_path)

    #Use the RCS API Client to open/load the project.
    projectId = rcsClient.OpenProject(rcs_project_file_path)

    #print the Id of the avaliable project.
    print(projectId)

    # Get IDs of all sections in the RCS project.
    secIds = []
    for s in rcsClient.Project.Sections.values():
        secIds.append(s.Id)

    # Calculate all sections in the RCS project.
    calc1_briefResults = rcsClient.Calculate(secIds)

    #Print the brief results.
    print(calc1_briefResults)
 
except Exception as e:
    message  = str(e)
    print(f"An error occurred: {message}")
    exit(1)

finally:
    if not rcsClient is None:
        del rcsClient

