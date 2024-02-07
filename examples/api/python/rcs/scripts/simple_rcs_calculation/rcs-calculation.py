from ideastatica_rcs_client import idea_statica_setup
from ideastatica_rcs_client import rcs_client
import os
import logging

ideaStatiCa_Version = r'23.1'
rcs_project_name = r"Project1.IdeaRcs"

logging.basicConfig(level = logging.INFO)
logger = logging.getLogger('rcs-calculation')
logger.setLevel(logging.INFO)

# Use the avaliable set-up tools to automatically find the IDEA StatiCa install path
# Alternatively we can provide a direct path
ideaSetupDir = idea_statica_setup.get_ideasetup_path(ideaStatiCa_Version)

#Print found setup directory
logger.info(f"idea setup dir : '{ideaSetupDir}'")

#Use set-up tools to find an avaliable port on the local machine to run communication 
freeTcp = idea_statica_setup.get_free_port()
logger.info(f"used tcp port : {freeTcp}")

logger.debug("creating ideastatica_rcs_client")

#Create the client and establish communication with RCS.
rcsClient = rcs_client.RcsClient(ideaSetupDir, freeTcp)

#We can retrieve and print the details of the RCS communication. 
logger.info(rcsClient.printServiceDetails())

try:

    # Get path of an existing RCS project on disk.
    dir_path = os.path.dirname(os.path.realpath(__file__))
    rcs_project_file_path = os.path.join(dir_path, )
    logger.info(f"rcs project file path '{rcs_project_file_path}")

    #Use the RCS API Client to open/load the project.
    projectId = rcsClient.OpenProject(rcs_project_name)

    #print the Id of the avaliable project.
    logger.info(f"Open project projectId = {projectId}")

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
    logger.warning(f"An error occurred: {message}")
    exit(1)

finally:
    if not rcsClient is None:
        logger.debug("closing ideastatica_rcs_client")
        del rcsClient

