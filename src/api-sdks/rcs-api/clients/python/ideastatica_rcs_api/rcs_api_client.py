import logging
from ideastatica_rcs_api import Configuration, CalculationApi, CrossSectionApi, DesignMemberApi, InternalForcesApi, SectionApi
import ideastatica_rcs_api.api_client as api_client
import ideastatica_rcs_api.api_ext.project_ext_api as project_ext_api
from typing import Optional

logger = logging.getLogger(__name__)

class RcsApiClient:
    def __init__(self, base_url: str):
        self.base_url = base_url
        self.configuration = Configuration(host=self.base_url)
        
        self.client: Optional[api_client.ApiClient] = None
        self.client_id: Optional[str] = None

        self.calculation: Optional[CalculationApi] = None
        self.cross_section: Optional[CrossSectionApi] = None
        self.design_member: Optional[DesignMemberApi] = None
        self.internal_forces: Optional[InternalForcesApi] = None
        self.section: Optional[SectionApi] = None
        self.project: Optional[project_ext_api.ProjectExtApi] = None

    def __enter__(self):
        # Initialize the client with the provided config
        self.client = api_client.ApiClient(self.configuration)

        logger.info(f"Client connected with url: {self.configuration._base_path}")       

        self.calculation = CalculationApi(self.client)
        self.cross_section = CrossSectionApi(self.client)
        self.design_member = DesignMemberApi(self.client)
        self.internal_forces = InternalForcesApi(self.client)
        self.section = SectionApi(self.client)
        self.project = project_ext_api.ProjectExtApi(self.client)

        logger.info(f"Client ready to use.")  

        return self

    def __exit__(self, exc_type, exc_value, traceback):
        # Perform any necessary cleanup
        try:
            if self.project is not None and self.project.active_project_id is not None:
                logger.info(f"Closing project project_id:{self.project.active_project_id} client_id: {self.client_id}")
                self.project.close_project(self.project.active_project_id)
        finally:
            self.project = None