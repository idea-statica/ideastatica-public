import os
from ideastatica_rcs_client import idea_statica_setup
from ideastatica_rcs_client import ideastatica_rcs_client
from ideastatica_rcs_client import rcsproject
from ideastatica_rcs_client import brief_result_tools

def print_sections_in_project():
     # print all sections in the rcs project
    print("Sections")
    for sec in rcsClient.Project.Sections.values():
        print(f'secId: {sec.Id} \'{sec.Description}\' rfCssId = {sec.RfCssId} memberId = {sec.CheckMemberId}')

def print_reinfcss_in_project():
    # print all reinforced cross-sections in the rcs project
    print('Reinforced cross-sections')
    for rfCss in rcsClient.Project.ReinfCrossSections.values():
        print(f'{rfCss.Id} \'{rfCss.Name}\' {rfCss.CssId}')

def print_capacity_check_vals(sectionIds, br):
    for secId in sectionIds:
        try:
            capacity = br[str(secId)]["Capacity"]
            print("Section \'{0}\' capacity {1}".format( rcsClient.Project.Sections[secId].Description, capacity["CheckValue"]))
        except:
            print("No results of capacity check in section", secId)

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
    print_sections_in_project()

    # print all reinforced cross-sections in the rcs project
    print_reinfcss_in_project()
   
    # get IDs of all sections in the rcs project
    secIds = []
    for s in rcsClient.Project.Sections.values():
        secIds.append(s.Id)

    # calculate all sections in the rcs project
    calc1_briefResults = rcsClient.Calculate(secIds)
    print_capacity_check_vals(secIds, calc1_briefResults)

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
    importSetting = rcsproject.ReinfCssImportSetting(None, "Complete")

    newReinSect = rcsClient.ImportReinfCss(importSetting, reinfCssTemplate)
    print("Id of a new reinfoced cross-section", newReinSect.Id)

    # print all sections in the rcs project
    print_sections_in_project()

    # print all reinforced cross-sections in the rcs project
    print_reinfcss_in_project()

    #set reinforced cross-section 2 to the section 1
    updateRes = rcsClient.UpdateReinfCssInSection(1, newReinSect.Id)
   
    # print all sections in the rcs project - sect 1 should be changed
    print_sections_in_project()

    # re-calculate all sections in the rcs project
    calc2_briefResults = rcsClient.Calculate(secIds)
    print_capacity_check_vals(secIds, calc2_briefResults)    

except Exception as e:
    message  = str(e)
    print(f"An error occurred: {message}")
    exit(1)

finally:
    if not rcsClient is None:
        del rcsClient

