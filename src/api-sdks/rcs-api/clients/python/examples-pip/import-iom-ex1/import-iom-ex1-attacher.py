import sys
import os
import asyncio
from pprint import pprint
from urllib.parse import urljoin

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
        # Import project from IOM file
        project_data = api_client.project.create_project_from_iom_file(iom_file_path)

        calcParams = ideastatica_rcs_api.RcsCalculationParameters()
        calcParams.sections = [project_data.sections[0].id]

            # run stress-strain analysis for the connection
        cal_results = api_client.calculation.calculate(api_client.project.active_project_id, calcParams)

        section_results = cal_results[0]

        print(f"Results section id : {section_results.section_id}\n")
        for item in section_results.overall_items:
            print(f"Status: {item.result_type} Check Value: {item.check_value}")

        downloaded_rcs_project_filename = os.path.join(dir_path, r'..\..\projects', 'GeneratedRcsProject.ideaRcs')

        api_client.project.save_project(project_data.project_id, downloaded_rcs_project_filename)


asyncio.run(main())