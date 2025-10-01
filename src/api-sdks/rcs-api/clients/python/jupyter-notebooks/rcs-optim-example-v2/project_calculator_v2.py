import ideastatica_rcs_api
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