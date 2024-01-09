import os
from ideastatica_rcs_client import idea_statica_setup
from ideastatica_rcs_client import ideastatica_rcs_client
from ideastatica_rcs_client import rcsproject
from ideastatica_rcs_client import brief_result_tools

ideaStatiCa_Version = r'23.1'

ideaSetupDir = idea_statica_setup.get_ideasetup_path(ideaStatiCa_Version)

print(ideaSetupDir)

freeTcp = idea_statica_setup.get_free_port()
print(freeTcp)

rcsClient = ideastatica_rcs_client.ideastatica_rcs_client(ideaSetupDir, freeTcp)
try:
    print(rcsClient.printServiceDetails())

    # try to open an project

    dir_path = os.path.dirname(os.path.realpath(__file__))
    rcs_project_file_path = os.path.join(dir_path, 'projects', 'Project1.IdeaRcs')
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

    # calculate all sections in the rcs project
    calc1_briefResults = rcsClient.Calculate(secIds)
    brief_result_tools.print_capacity_check_vals(rcsClient.Project, secIds, calc1_briefResults)

    # get detail results of all calculated rcs sections
    detailResults = rcsClient.GetResults(secIds)
    #print(detailResults)

    #read a rcs template from NAV file
    rcs_template_file_path = os.path.join(dir_path, 'templates', 'rect-L-3-2.nav')
    print(rcs_template_file_path)

    reinfCssTemplate = None
    with open(rcs_template_file_path, 'r') as file:
        reinfCssTemplate = file.read()

    # if reinfCssId is None a new reinforced cross-section will be created 
    importSetting = rcsproject.ReinforcedCrossSectionImportSetting(None, "Complete")

    newReinSect = rcsClient.ImportReinforcedCrossSection(importSetting, reinfCssTemplate)
    print("Id of a new reinforced cross-section", newReinSect.Id)

    # print all sections in the rcs project
    brief_result_tools.print_sections_in_project(rcsClient.Project)

    # print all reinforced cross-sections in the rcs project
    brief_result_tools.print_reinforced_cross_sections_in_project(rcsClient.Project)

    #set reinforced cross-section 2 to the section 1
    updateRes = rcsClient.UpdateReinforcedCrossSectionInSection(1, newReinSect.Id)
   
    # print all sections in the rcs project - sect 1 should be changed
    brief_result_tools.print_sections_in_project(rcsClient.Project)

    # re-calculate all sections in the rcs project
    calc2_briefResults = rcsClient.Calculate(secIds)
    brief_result_tools.print_capacity_check_vals(rcsClient.Project, secIds, calc2_briefResults)

    try:
        print("Calc 1 :", brief_result_tools.get_check_value(calc1_briefResults, "Capacity", 1), "Calc 2 : ", brief_result_tools.get_check_value(calc2_briefResults, "Capacity", 1))
    except Exception as ee:
        print("error", str(ee) )    

except Exception as e:
    message  = str(e)
    print(f"An error occurred: {message}")
    exit(1)

finally:
    if not rcsClient is None:
        del rcsClient

