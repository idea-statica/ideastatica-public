from ideastatica_rcs_client import idea_statica_setup
from ideastatica_rcs_client import ideastatica_rcs_client
from ideastatica_rcs_client import rcsproject 
import os

ideaStatiCa_Version = r'23.1'

def print_sections_in_project(rcs_project):
     # print all sections in the rcs project
    print("Sections in the project")
    for sec in rcs_project.Sections.values():
        print(f'secId: {sec.Id} \'{sec.Description}\' rfCssId = {sec.RfCssId} memberId = {sec.CheckMemberId}')

def print_reinfcss_in_project(rcs_project):
    # print all reinforced cross-sections in the rcs project
    print('Reinforced cross-sections in the project')
    for rfCss in rcs_project.ReinfCrossSections.values():
        print(f'{rfCss.Id} \'{rfCss.Name}\' {rfCss.CssId}')

def get_capacity_check_val(secId, br):
    capacity = br[str(secId)]["Capacity"]
    return capacity["CheckValue"]

def print_capacity_check_vals(rcs_project, sectionIds, br):
    for secId in sectionIds:
        try:
            print("Section \'{0}\' capacity {1}".format( rcs_project.Sections[secId].Description, get_capacity_check_val(secId, br)))
        except:
            print("No results of capacity check in section", secId)

def get_section_details(rcs_project_filename):
    ideaSetupDir = idea_statica_setup.get_ideasetup_path(ideaStatiCa_Version)

    #print("IDEA StatiCa found : ", ideaSetupDir)

    freeTcp = idea_statica_setup.get_free_port()
    #print("Free tcp port on localhost :", freeTcp)
    
    try:
        rcsClient = ideastatica_rcs_client.ideastatica_rcs_client(ideaSetupDir, freeTcp)

        print(rcsClient.printServiceDetails())

        projectId = rcsClient.OpenProject(rcs_project_filename)
    
        print_sections_in_project(rcsClient.Project)
        
        print_reinfcss_in_project(rcsClient.Project)
        
    finally:
        if not rcsClient is None:
            print("closing IdeaStatiCa.RcsRestApi.exe")
            del rcsClient
            

def calc_rcs_proj_variants(project_to_calculate, section_to_calculate, reinforced_css_templates):
    ideaSetupDir = idea_statica_setup.get_ideasetup_path(ideaStatiCa_Version)
    freeTcp = idea_statica_setup.get_free_port()
    capacity_checks = []
    try:
        rcsClient = ideastatica_rcs_client.ideastatica_rcs_client(ideaSetupDir, freeTcp)

        #print(rcsClient.printServiceDetails())

        projectId = rcsClient.OpenProject(project_to_calculate)

        secIds = [section_to_calculate]
        
        section_detail = rcsClient.Project.Sections[str(section_to_calculate)]

    
        # update existing reinforced cross-section by template in the project
        #importSetting = rcsproject.ReinfCssImportSetting(section_detail.RfCssId, "Complete")        
        importSetting = rcsproject.ReinfCssImportSetting(None, "Complete") 
        
        for template in reinforced_css_templates:
            print(template)
            reinfCssTemplate = None
            with open(template, 'r') as file:
                reinfCssTemplate = file.read()
  
            newReinSect = rcsClient.ImportReinfCss(importSetting, reinfCssTemplate)
            print(newReinSect.Id)
            updateRes = rcsClient.UpdateReinfCssInSection(section_to_calculate, newReinSect.Id)
            briefResult1 = rcsClient.Calculate(secIds)
        
            capacity = briefResult1[str(section_to_calculate)]["Capacity"]
            capacity_check_val = capacity["CheckValue"]
            capacity_checks.append(capacity_check_val)
            print(capacity_check_val)        
        
    finally:
        if not rcsClient is None:
            print("closing IdeaStatiCa.RcsRestApi.exe")
            del rcsClient