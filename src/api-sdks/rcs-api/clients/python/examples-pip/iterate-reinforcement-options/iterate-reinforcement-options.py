import os
from pprint import pprint
from urllib.parse import urljoin
import ideastatica_rcs_api
import ideastatica_rcs_api.rcs_api_service_attacher as rcs_api_service_attacher

baseUrl = "http://localhost:5000"

dir_path = os.path.dirname(os.path.realpath(__file__))
rcs_project_file_path = r'Project1.IdeaRcs'


with rcs_api_service_attacher.RcsApiServiceAttacher(baseUrl).create_api_client() as api_client:
    try:
        # Open project
        uploadRes = api_client.project.open_project_from_file(rcs_project_file_path)

        # Get the project data
        project_data = api_client.project.get_active_project()

        # print all sections in the rcs project
        sections = api_client.section.sections(api_client.project.active_project_id)
        print(sections)

        # print all reinforced cross-sections in the rcs project
        css = api_client.cross_section.reinforced_cross_sections(api_client.project.active_project_id)
        print(css)

        # choose which section to run loop on.
        section = sections[0]
        rcsId = section.rcs_id

        # test calculation for 3 different reinforcement scenarios
        reinforcement_options = ['rect-L-2-2.nav', 'rect-L-3-2.nav', 'rect-L-4-2.nav']

        for template in reinforcement_options:
            # read the rcs template from NAV file
            rcs_template_file_path = os.path.join(dir_path, template)
            print(rcs_template_file_path)

            reinfCssTemplate = None
            with open(rcs_template_file_path, 'r') as file:
                reinfCssTemplate = file.read()

            rcsImportSettings = ideastatica_rcs_api.RcsReinforcedCrosssSectionImportSetting()
            rcsImportSettings.reinforced_cross_section_id = rcsId  # Determine which CSS is being updated
            rcsImportSettings.parts_to_import = "Reinf"

            rcsImport = ideastatica_rcs_api.RcsReinforcedCrossSectionImportData()
            rcsImport.template = reinfCssTemplate
            rcsImport.setting = rcsImportSettings

            importedCss = api_client.cross_section.import_reinforced_cross_section(api_client.project.active_project_id, rcsImport)
            print("Id of a new reinforced cross-section", importedCss.id)

            calcParams = ideastatica_rcs_api.RcsCalculationParameters()
            calcParams.sections = [section.id]

            api_client.calculation.calculate(api_client.project.active_project_id, calcParams)

            resultParams = ideastatica_rcs_api.RcsResultParameters()
            resultParams.sections = [section.id]

            detailedResults1 = api_client.calculation.get_results(api_client.project.active_project_id, resultParams)

            print(detailedResults1)

            #save each version
            template_name = os.path.splitext(template)[0]
            new_filename = template_name + ".ideaRcs"
            save_path = os.path.join(dir_path, new_filename)
            
            api_client.project.save_project(api_client.project.active_project_id, save_path)

            try:
                print(f"Calc {template}:", detailedResults1[0].section_result)
            except Exception as ee:
                print("Error in result print:", str(ee))

    finally:
        # This will execute once after all loops and operations
        if api_client is not None:
            del api_client
