from ideastatica_rcs_client import idea_statica_setup
from ideastatica_rcs_client import ideastatica_rcs_client
from ideastatica_rcs_client import rcsproject
from ideastatica_rcs_client import brief_result_tools 
import os
import pandas as pd

ideaStatiCa_Version = r'23.1'

def get_capacity_check_val(secId, br):
    capacity = br[str(secId)]["Capacity"]
    return capacity["CheckValue"]

def get_section_details(rcs_project_filename):
    ideaSetupDir = idea_statica_setup.get_ideasetup_path(ideaStatiCa_Version)

    #print("IDEA StatiCa found : ", ideaSetupDir)

    freeTcp = idea_statica_setup.get_free_port()
    #print("Free tcp port on localhost :", freeTcp)
    
    try:
        rcsClient = ideastatica_rcs_client.ideastatica_rcs_client(ideaSetupDir, freeTcp)

        print(rcsClient.printServiceDetails())

        projectId = rcsClient.OpenProject(rcs_project_filename)
    
        #brief_result_tools.print_sections_in_project(rcsClient.Project)
        
        #brief_result_tools.print_reinfcss_in_project(rcsClient.Project)
        
        #sections_in_project = rcsClient.Project.Sections
        
        ids = []
        names = []
        rfCssIds = []
        dmIds = []
        
        for sec in rcsClient.Project.Sections.values():
            ids.append(sec.Id)
            names.append(sec.Description)
            rfCssIds.append(sec.ReinforcedCrossSectionId)
            dmIds.append(sec.CheckMemberId)
        
        data = {"Id" : ids, "Name" : names, "RfCssId" : rfCssIds, "Member" : dmIds}
        
        df_sections = pd.DataFrame.from_dict(data)
        return df_sections
            
    finally:
        if not rcsClient is None:
            print("closing IdeaStatiCa.RcsRestApi.exe")
            del rcsClient
            

def calc_rcs_proj_variants(project_to_calculate, section_to_calculate, reinforced_cross_section_templates):
    ideaSetupDir = idea_statica_setup.get_ideasetup_path(ideaStatiCa_Version)
    freeTcp = idea_statica_setup.get_free_port()
    
    try:
        rcsClient = ideastatica_rcs_client.ideastatica_rcs_client(ideaSetupDir, freeTcp)

        #print(rcsClient.printServiceDetails())

        projectId = rcsClient.OpenProject(project_to_calculate)

        secIds = [section_to_calculate]
        
        section_detail = rcsClient.Project.Sections[str(section_to_calculate)]

    
        # update existing reinforced cross-section by template in the project
        #importSetting = rcsproject.ReinforcedCrossSectionImportSetting(section_detail.RfCssId, "Complete")        
        importSetting = rcsproject.ReinforcedCrossSectionImportSetting(None, "Complete") 
        
        secIDs = []
        secNames = []
        templates = []
        rfIds = []
        checkVals = []
        capacity_checks = []
        crack_width_checks = []
        interaction_checks = []
        shear_checks = []
        stresslimitation_checks = []
        response_checks = []

        for template in reinforced_cross_section_templates:
            #print(template)
            reinforcedCrossSectionTemplate = None
            with open(template, 'r') as file:
                reinforcedCrossSectionTemplate = file.read()
  
            newReinSect = rcsClient.ImportReinforcedCrossSection(importSetting, reinforcedCrossSectionTemplate)
            #print(newReinSect.Id)
            updateRes = rcsClient.UpdateReinforcedCrossSectionInSection(section_to_calculate, newReinSect.Id)
            briefResult1 = rcsClient.Calculate(secIds)
        
            capacity_check_val = brief_result_tools.get_check_value(briefResult1, "Capacity", section_to_calculate)
            capacity_checks.append(capacity_check_val)
            
            crackwidth_check_val = brief_result_tools.get_check_value(briefResult1, "CrackWidth", section_to_calculate)
            crack_width_checks.append(crackwidth_check_val)
            
            shear_val = brief_result_tools.get_check_value(briefResult1, "Shear", section_to_calculate)
            shear_checks.append(shear_val)            
  
            interaction_val = brief_result_tools.get_check_value(briefResult1, "Interaction", section_to_calculate)
            interaction_checks.append(interaction_val)

            response_val = brief_result_tools.get_check_value(briefResult1, "Response", section_to_calculate)
            response_checks.append(response_val)        
        
            stresslimitation_val = brief_result_tools.get_check_value(briefResult1, "StressLimitation", section_to_calculate)
            stresslimitation_checks.append(response_val)          
        
            #print(capacity_check_val)
            secIDs.append(section_to_calculate)
            secNames.append(section_detail.Description)
            templates.append(template)
            rfIds.append(newReinSect.Id)
            checkVals.append(capacity_check_val)

            
        data = {"SecId" : secIDs, "SecName" : secNames, "Template" : templates, "RfId" : rfIds, "Capacity" : checkVals, "Shear" : shear_checks, "Interaction" : interaction_checks, "CrackWidth" : crack_width_checks, "Response" : response_checks, "StressLimitation" : response_checks}
        
        df_sectionChecks = pd.DataFrame.from_dict(data)
        return df_sectionChecks
        
    finally:
        if not rcsClient is None:
            print("closing IdeaStatiCa.RcsRestApi.exe")
            del rcsClient