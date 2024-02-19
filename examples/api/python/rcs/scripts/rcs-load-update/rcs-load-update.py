import os
import logging
import ideastatica_rcs_client
from ideastatica_rcs_client import idea_statica_setup
from ideastatica_rcs_client import rcs_client
from ideastatica_rcs_client import brief_result_tools
from ideastatica_rcs_client import detail_results_tools
from ideastatica_rcs_client import loading_tools
 
ideaStatiCa_Version = r'23.1'
rcs_project_name = r"Project2.IdeaRcs"

logging.basicConfig(level = logging.INFO)
logger = logging.getLogger('rcs-load-update')
logger.setLevel(logging.INFO)

# Use the avaliable set-up tools to automatically find the IDEA StatiCa install path
# Alternatively we can provide a direct path
ideaSetupDir = idea_statica_setup.get_ideasetup_path(ideaStatiCa_Version)

#Print found setup directory
logger.info(f"ideaSetupDir = '{ideaSetupDir}'")

#Use set-up tools to dind an avaliable port on the local machine to run communication 
freeTcp = idea_statica_setup.get_free_port()
logger.info(f"freeTcp = {freeTcp}")

#Create the client and establish communication with RCS.
rcsClient = rcs_client.RcsClient(ideaSetupDir, freeTcp)

#We can retrieve and print the details of the RCS communication. 
logger.info(rcsClient.printServiceDetails())

try:
    # Get path of an existing RCS project on disk.
    dir_path = os.path.dirname(os.path.realpath(__file__))
    rcs_project_file_path = os.path.join(dir_path, rcs_project_name)
    logger.info(f"rcs_project_file_path = '{rcs_project_file_path}'")

    #Use the RCS API Client to open/load the project.
    projectId = rcsClient.OpenProject(rcs_project_file_path)

    #print the Id of the avaliable project.
    logger.info(f"projectId = '{projectId}")

    # print all sections in the rcs project
    brief_result_tools.print_sections_in_project(rcsClient.Project)

    # print all reinforced cross-sections in the rcs project
    brief_result_tools.print_reinforced_cross_sections_in_project(rcsClient.Project)
   
    # get IDs of all sections in the rcs project
    secIds = []
    for s in rcsClient.Project.Sections.values():
        secIds.append(s.Id)

    firstSectionId = secIds[0]

    # calculate all sections in the rcs project
    calc1_briefResults = rcsClient.Calculate(secIds)
    brief_result_tools.print_capacity_check_vals(rcsClient.Project, secIds, calc1_briefResults)
    detailedResults = rcsClient.GetResults(secIds)

    sectionRes = detail_results_tools.get_section_results(detailedResults)

    extreme = detail_results_tools.get_extreme_results(sectionRes, firstSectionId, 1)

    # get detailed results of a capacity check in the first section
    capacity_res = detail_results_tools.get_result_by_type(extreme, "Capacity")

    # crack with for short term extreme
    crack_width_res = detail_results_tools.get_result_by_type(extreme, "CrackWidth")
    crackWidth_short_term = crack_width_res[0]
    crack_width = crackWidth_short_term["W"]

    # this is the max My bending moment for reinforced cross-section
    fu_my =  float(capacity_res["Fu"]["My"])
    print("Fu.My = ", fu_my)

    # get loading in the first section
    loadingInSection = rcsClient.GetLoadingInSection(firstSectionId)
    intForceULS = loading_tools.get_internalForce(loadingInSection, 0, 0)
    intForceSLS1 = loading_tools.get_internalForce(loadingInSection, 0, 4)
    intForceSLS2 = loading_tools.get_internalForce(loadingInSection, 0, 6)

    my = float(capacity_res["InternalFores"]["My"])
    results = []
    results.append({"my": my, "capacity" :  brief_result_tools.get_check_value(calc1_briefResults, "Capacity", firstSectionId), "CrackWidth" : crack_width });

    my = my + 5000

    # increase and recalculate loading up to max My
    while my < fu_my:
        # ULS checks       
        intForceULS['My'] = my
        intForceSLS1['My'] = my
        intForceSLS2['My'] = my

        #SLS checks (crack width)
        rcsClient.SetLoadingInSection(firstSectionId, loadingInSection)
        briefResults = rcsClient.Calculate(secIds)

        detailedResults = rcsClient.GetResults(secIds)

        sectionRes = detail_results_tools.get_section_results(detailedResults)

        extreme = detail_results_tools.get_extreme_results(sectionRes, firstSectionId, 1)

        # get crack width for the actual loading
        crack_width_res = detail_results_tools.get_result_by_type(extreme, "CrackWidth")
        crackWidth_short_term = crack_width_res[0]
        crack_width = crackWidth_short_term["W"]

        results.append({"my": my, "capacity" :  brief_result_tools.get_check_value(briefResults, "Capacity", firstSectionId), "CrackWidth" : crack_width});
        my = my + 5000

    # last calculation - capacity is 100%
    my = fu_my
    intForceULS['My'] = my
    intForceSLS1['My'] = my
    intForceSLS2['My'] = my   
    rcsClient.SetLoadingInSection(firstSectionId, loadingInSection)
    briefResults = rcsClient.Calculate(secIds)

    detailedResults = rcsClient.GetResults(secIds)

    sectionRes = detail_results_tools.get_section_results(detailedResults)

    extreme = detail_results_tools.get_extreme_results(sectionRes, firstSectionId, 1)

    # get crack width for the actual loading
    crack_width_res = detail_results_tools.get_result_by_type(extreme, "CrackWidth")
    crackWidth_short_term = crack_width_res[0]
    crack_width = crackWidth_short_term["W"]

    results.append({"my": my, "capacity" :  brief_result_tools.get_check_value(briefResults, "Capacity", firstSectionId), "CrackWidth" : crack_width });

    print(results)

except Exception as e:
    message  = str(e)
    print(f"An error occurred: {message}")
    exit(1)

finally:
    if not rcsClient is None:
        del rcsClient


