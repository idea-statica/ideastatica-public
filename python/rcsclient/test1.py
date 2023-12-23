import os
from ideastatica_rcs_client import idea_statica_setup
from ideastatica_rcs_client import ideastatica_rcs_client

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
    print('Sections')
    for sec in rcsClient.Project.Sections.values():
        print(f'secId = {sec.Id} \'{sec.Description}\' rfCssId = {sec.RfCssId} memberId = {sec.CheckMemberId}')

    # print all reinforced cross-sections in the rcs project
    print('Reinforced cross-sections')
    for rfCss in rcsClient.Project.ReinfCrossSections.values():
        print(f'{rfCss.Id} \'{rfCss.Name}\' {rfCss.CssId}')

    
    # get IDs of all sections in the rcs project
    secIds = []
    for s in rcsClient.Project.Sections.values():
        secIds.append(s.Id)

    # calculate all sections in the rcs project
    briefResults = rcsClient.Calculate(secIds)
    print(briefResults)

    # get detail results of all calculated rcs sections
    detailResults = rcsClient.GetResults(secIds)
    print(detailResults)

    #set reinforced cross-section 2 to the section 1
    updateRes = rcsClient.UpdateReinfCssInSection(1, 2)
    
    # print all sections in the rcs project
    print('Sections')
    for sec in rcsClient.Project.Sections.values():
        print(f'secId: {sec.Id} \'{sec.Description}\' rfCssId = {sec.RfCssId} memberId = {sec.CheckMemberId}')

except Exception as e:
    message  = str(e)
    print(f"An error occurred: {message}")
    exit(1)

finally:
    if not rcsClient is None:
        del rcsClient


