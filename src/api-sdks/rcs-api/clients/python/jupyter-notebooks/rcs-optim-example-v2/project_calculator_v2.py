import ideastatica_rcs_api
import ideastatica_rcs_api.helpers as helpers
import ideastatica_rcs_api.loading_tools as loading_tools
import ideastatica_rcs_api.brief_result_tools as brief_result_tools
import ideastatica_rcs_api.raw_results_tools as raw_results_tools

from client_factory import create_client
import pandas as pd

baseUrl = "http://localhost:5000"

def get_section_details(rcs_project_filename : str):
    print(rcs_project_filename)

    with create_client() as api_client:
        # Open project
        uploadRes = api_client.project.open_project_from_file(rcs_project_filename)

        # Get the project data
        project_data = api_client.project.get_active_project()

        ids = []
        names = []
        rfCssIds = []
        dmIds = []
        
        for sec in project_data.sections:
            ids.append(sec.id)
            names.append(sec.description)
            rfCssIds.append(sec.rcs_id)
            dmIds.append(sec.check_member_id)
        
        data = {"Id" : ids, "Name" : names, "RfCssId" : rfCssIds, "Member" : dmIds}
        
        df_sections = pd.DataFrame.from_dict(data)
        return df_sections
    
def calc_rcs_crack_width(rcs_project_filename, section_to_calculate, moment_step):
    
    with create_client() as api_client:
        # Open project
        uploadRes = api_client.project.open_project_from_file(rcs_project_filename)

        # Get the project data
        project_data = api_client.project.get_active_project()

        # get IDs of all sections in the rcs project
        secIds = []
        for s in  project_data.sections:
            secIds.append(s.id)

        firstSectionId = secIds[0]


        calcParams = ideastatica_rcs_api.RcsCalculationParameters()
        calcParams.sections = secIds

        # get loading
        sectionLoadingXml = api_client.internal_forces.get_section_loading(api_client.project.active_project_id, firstSectionId)
        sectionLoadingDict = helpers.xml_to_dict(sectionLoadingXml)

        #get extremes in section
        extremesInSection = loading_tools.get_extremes(sectionLoadingDict)
 
        # run stress-strain analysis for the connection
        calc1_briefResults = api_client.calculation.calculate(api_client.project.active_project_id, calcParams)

        resultsParams = ideastatica_rcs_api.RcsResultParameters()
        resultsParams.sections = secIds

        detail_results_json = api_client.calculation.get_raw_results(api_client.project.active_project_id, resultsParams)

        detail_results = raw_results_tools.jsonRes_to_dict(detail_results_json)

        # take from the array which has just one section
        detail_results1 = detail_results[0]

        extremesInSectionRes = raw_results_tools.get_extremes(detail_results1)

        capacities = []

        counter = 0

        results = []

        resForExtreme = extremesInSectionRes[counter]
        capacity_res = raw_results_tools.get_result_by_type(resForExtreme, "capacity")
        fu1 = capacity_res['fu1']
        fu2 = capacity_res['fu2']
        fu = capacity_res['fu']

        fu_my =  fu["my"]

        my = capacity_res["internalFores"]["my"]
        capacities.append({"SecId": secIds[0], "Fu.My" : fu_my})

        df_capacity = pd.DataFrame.from_records(capacities)
        
        df_capacity

        # crack with
        resForExtreme = extremesInSectionRes[0]
        crack_width_res = raw_results_tools.get_result_by_type(resForExtreme, "crackWidth")
        crack_width = crack_width_res["w"] 

        # get loading in the first section
        extreme = extremesInSection[0]
        intForceULS = loading_tools.get_internalForce(extreme, 0)
        intForceSLS1 = loading_tools.get_internalForce(extreme, 4)
        intForceSLS2 = loading_tools.get_internalForce(extreme, 6)

        results.append({"my": my, "capacity" :  capacity_res["checkValue"], "CrackWidth [mm]" : crack_width*1000});

        my = my + moment_step

        # increase and recalculate loading up to max My
        while my < fu_my:
            # ULS checks       
            intForceULS['My'] = my
            intForceSLS1['My'] = my
            intForceSLS2['My'] = my

            xml = helpers.dict_to_xml(sectionLoadingDict)

            loadingInSection = ideastatica_rcs_api.RcsSectionLoading()
            loadingInSection.section_id = firstSectionId
            loadingInSection.loading_xml = xml

            updateLoadingRes = api_client.internal_forces.set_section_loading(api_client.project.active_project_id, firstSectionId, loadingInSection)

            calc1_briefResults = api_client.calculation.calculate(api_client.project.active_project_id, calcParams)

            detail_results_json = api_client.calculation.get_raw_results(api_client.project.active_project_id, resultsParams)

            detail_results = raw_results_tools.jsonRes_to_dict(detail_results_json)

            # take from the array which has just one section
            detail_results1 = detail_results[0]

            extremesInSectionRes = raw_results_tools.get_extremes(detail_results1)

            counter = 0

            resForExtreme = extremesInSectionRes[counter]
            crack_width_res = raw_results_tools.get_result_by_type(resForExtreme, "crackWidth")
            capacity_res = raw_results_tools.get_result_by_type(resForExtreme, "capacity")

            crack_width = crack_width_res["w"]

            results.append({"my": my, "capacity" :  capacity_res["checkValue"], "CrackWidth [mm]" : crack_width*1000});

            my = my + moment_step

        # last calculation - capacity is 100%
        my = fu_my
        intForceULS['My'] = my
        intForceSLS1['My'] = my
        intForceSLS2['My'] = my

        xml = helpers.dict_to_xml(sectionLoadingDict)

        loadingInSection = ideastatica_rcs_api.RcsSectionLoading()
        loadingInSection.section_id = firstSectionId
        loadingInSection.loading_xml = xml

        updateLoadingRes = api_client.internal_forces.set_section_loading(api_client.project.active_project_id, firstSectionId, loadingInSection)

        calc1_briefResults = api_client.calculation.calculate(api_client.project.active_project_id, calcParams)

        detail_results_json = api_client.calculation.get_raw_results(api_client.project.active_project_id, resultsParams)

        detail_results = raw_results_tools.jsonRes_to_dict(detail_results_json)

        # take from the array which has just one section
        detail_results1 = detail_results[0]

        extremesInSectionRes = raw_results_tools.get_extremes(detail_results1)

        counter = 0

        resForExtreme = extremesInSectionRes[counter]
        crack_width_res = raw_results_tools.get_result_by_type(resForExtreme, "crackWidth")
        capacity_res = raw_results_tools.get_result_by_type(resForExtreme, "capacity")

        crack_width = crack_width_res["w"]

        results.append({"my": my, "capacity" :  capacity_res["checkValue"], "CrackWidth [mm]" : crack_width*1000})

        df_check = pd.DataFrame.from_records(results)
        return df_check                  