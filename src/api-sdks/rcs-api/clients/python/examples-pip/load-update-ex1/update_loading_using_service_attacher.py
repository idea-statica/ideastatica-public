import sys
import os
import asyncio
from pprint import pprint
from urllib.parse import urljoin
import xml.etree.ElementTree as ET

dir_path = os.path.dirname(os.path.realpath(__file__))
iom_file_path = os.path.join(dir_path, r'..\..\projects', 'ImportOpenModel.xml')
rcs_project_file_path = os.path.join(dir_path, r'..\..\projects', 'Project1.ideaRcs')

# uncomment these lines to use the local ideastatica_rcs_api

# # Get the parent directory
# parent_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), r'../..'))

# # Add the parent directory to sys.path
# sys.path.append(parent_dir)

import ideastatica_rcs_api
import ideastatica_rcs_api.rcs_api_service_attacher as rcs_api_service_attacher

baseUrl = "http://localhost:5000"

# Defining the host is optional and defaults to http://localhost
# See configuration.py for a list of all supported configuration parameters.
configuration = ideastatica_rcs_api.Configuration(
    host = baseUrl
)


async def main():
    with rcs_api_service_attacher.RcsApiServiceAttacher(baseUrl).create_api_client() as api_client:
        # Open project
        upload_res = api_client.project.open_project_from_file(rcs_project_file_path)

        # Get the project data
        project_data = api_client.project.get_active_project()

        # Prepare calculation parameters
        calc_params = ideastatica_rcs_api.RcsCalculationParameters()
        first_sect_id = project_data.sections[0].id
        calc_params.sections = [first_sect_id]

        # get existing loading from the section 1
        sect1_loading_xml = api_client.internal_forces.get_section_loading(api_client.project.active_project_id, first_sect_id)

        # Run stress-strain analysis for the connection
        calc_results = api_client.calculation.calculate(
            api_client.project.active_project_id,
            calc_params
        )

        # Get results
        section_results = calc_results[0]

        print(f"Results section id : {section_results.section_id}\n")
        for item in section_results.overall_items:
            print(f"Status: {item.result_type} Check Value: {item.check_value}")


        root =  ET.fromstring(sect1_loading_xml)      
        extremes = root.findall('SectionExtremeBase')

        # get the first extreme
        first_extreme = extremes[0]

        # gat all InputLoad in first_extreme
        inputLoads = first_extreme.findall("./Loads/Loads/InputLoad")

        #get the first inputLoads
        first_input_load = inputLoads[0]

        # print type of combination amd My
        combiType = first_input_load.find('CombiType').text

        my_element = first_input_load.find('InternalForce/My')
        my = float(my_element.text)

        print(f"CombiType: {combiType} My: {my}")

        my = my * 2  # increase my
        my_element.text = str(my)

        # get update xml
        updated_loading_xml = ET.tostring(root, encoding='unicode', method='xml')

        updated_loading = ideastatica_rcs_api.RcsSectionLoading()
        updated_loading.section_id = first_sect_id
        updated_loading.loading_xml = updated_loading_xml

        # update loading in the section 1
        api_client.internal_forces.set_section_loading(api_client.project.active_project_id, first_sect_id, updated_loading)

        # reculate the project with updated loading
        calc_results = api_client.calculation.calculate(
            api_client.project.active_project_id,
            calc_params
        )

        # Get results
        section_results = calc_results[0]

        # print recalculated results
        print(f"Results section id : {section_results.section_id}\n")
        for item in section_results.overall_items:
            print(f"Status: {item.result_type} Check Value: {item.check_value}")


asyncio.run(main())
