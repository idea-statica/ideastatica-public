import logging
from ideastatica_connection_api import Configuration, ClientApi, CalculationApi, ConnectionApi, ExportApi, LoadEffectApi, MaterialApi, MemberApi, OperationApi, ParameterApi, PresentationApi, ReportApi, TemplateApi
import ideastatica_connection_api.api_client as api_client
import ideastatica_connection_api.api_ext.project_ext_api as project_ext_api
from typing import Optional

logger = logging.getLogger(__name__)

class ConnectionApiClient:
    def __init__(self, base_url: str):
        self.base_url = base_url
        self.configuration = Configuration(host=self.base_url)
        
        self.client: Optional[api_client.ApiClient] = None
        self.client_id: Optional[str] = None

        self.calculation: Optional[CalculationApi] = None
        self.connection: Optional[ConnectionApi] = None
        self.export: Optional[ExportApi] = None
        self.load_effect: Optional[LoadEffectApi] = None
        self.material: Optional[MaterialApi] = None
        self.member: Optional[MemberApi] = None
        self.operation: Optional[OperationApi] = None
        self.parameter: Optional[ParameterApi] = None
        self.presentation: Optional[PresentationApi] = None
        self.project: Optional[project_ext_api.ProjectExtApi] = None
        self.report: Optional[ReportApi] = None
        self.template: Optional[TemplateApi] = None

    def __enter__(self):
        # Initialize the client with the provided config
        self.client = api_client.ApiClient(self.configuration)

        client_api = ClientApi(self.client)
        self.client_id = client_api.connect_client()

        logger.info(f"Client connected with client_id: {self.client_id} url: {self.configuration._base_path}")

        # Add your ClientId to HTTP header
        self.client.default_headers['ClientId'] = self.client_id        

        self.calculation = CalculationApi(self.client)
        self.connection = ConnectionApi(self.client)
        self.export = ExportApi(self.client)
        self.load_effect = LoadEffectApi(self.client)
        self.material = MaterialApi(self.client)
        self.member = MemberApi(self.client)
        self.operation = OperationApi(self.client)
        self.parameter = ParameterApi(self.client)
        self.presentation = PresentationApi(self.client)
        self.project = project_ext_api.ProjectExtApi(self.client)
        self.report = ReportApi(self.client)
        self.template = TemplateApi(self.client)

        logger.info(f"Client ready to use.")  

        return self

    def __exit__(self, exc_type, exc_value, traceback):
        # Perform any necessary cleanup
        try:
            if self.project:
                logger.info(f"Closing project project_id:{self.project.active_project_id} client_id: {self.client_id}")
                self.project.close_project(self.project.active_project_id)
        finally:
            self.project = None