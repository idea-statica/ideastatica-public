import os
import logging
from ideastatica_rcs_client import idea_statica_setup
from ideastatica_rcs_client import rcs_client
from ideastatica_rcs_client import rcsproject
from ideastatica_rcs_client import brief_result_tools
from ideastatica_rcs_client import loading_tools
from ideastatica_rcs_client import detail_results_tools

ideaStatiCa_Version = r'24.0'
logging.basicConfig(level = logging.INFO)

ideaSetupDir = idea_statica_setup.get_ideasetup_path(ideaStatiCa_Version)

print(ideaSetupDir)

freeTcp = idea_statica_setup.get_free_port()
print(freeTcp)

rcsClient = rcs_client.RcsClient(ideaSetupDir, freeTcp)
try:
    print(rcsClient.printServiceDetails())

    # try to open an project

    dir_path = os.path.dirname(os.path.realpath(__file__))
    rcs_project_file_path = os.path.join(dir_path, 'projects', 'Project2.IdeaRcs')
    print(rcs_project_file_path)

    projectId = rcsClient.OpenProject(rcs_project_file_path)

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

    capacity_res = detail_results_tools.get_result_by_type(extreme, "Capacity")

    fu_my =  float(capacity_res["Fu"]["My"])
    print("Fu.My = ", fu_my)

    # get loading in the first section
    loadingInSection = rcsClient.GetLoadingInSection(firstSectionId)
    intForce = loading_tools.get_internalForce(loadingInSection, 0, 0)

    my = float(capacity_res["InternalFores"]["My"])
    capacity = []
    capacity.append({"my": my, "capacity" :  brief_result_tools.get_check_value(calc1_briefResults, "Capacity", firstSectionId) });

    my = my + 5000

    
    while my < fu_my:       
        intForce['My'] = my
        rcsClient.SetLoadingInSection(firstSectionId, loadingInSection)
        briefResults = rcsClient.Calculate(secIds)
        capacity.append({"my": my, "capacity" :  brief_result_tools.get_check_value(briefResults, "Capacity", firstSectionId) });
        my = my + 5000


    my = fu_my
    intForce['My'] = my
    rcsClient.SetLoadingInSection(firstSectionId, loadingInSection)
    briefResults = rcsClient.Calculate(secIds)
    capacity.append({"my": my, "capacity" :  brief_result_tools.get_check_value(briefResults, "Capacity", firstSectionId) });


    print(capacity)

except Exception as e:
    message  = str(e)
    print(f"An error occurred: {message}")
    exit(1)

finally:
    if not rcsClient is None:
        del rcsClient

