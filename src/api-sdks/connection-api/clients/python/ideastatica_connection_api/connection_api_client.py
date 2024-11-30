import logging
from ideastatica_connection_api import Configuration, ClientApi, CalculationApi, ConnectionApi, ExportApi, LoadEffectApi, MaterialApi, MemberApi, OperationApi, ParameterApi, PresentationApi, ReportApi, TemplateApi
import ideastatica_connection_api.api_client as api_client
import ideastatica_connection_api.api_ext.project_ext_api as project_ext_api
from typing import Optional

logger = logging.getLogger(__name__)

class ConnectionApiClient:
    def __init__(self, configuration: Configuration):
        self.configuration = configuration

        self.client: Optional[api_client.ApiClient] = api_client.ApiClient(self.configuration)
        client_api = ClientApi(self.client)
        self.client_id = client_api.connect_client()
        logger.info(f"Client connected with client_id: {self.client_id} url: {self.configuration._base_path}")

        self.project_id: Optional[str] = None
        self.project: Optional[project_ext_api.ProjectExtApi] = None

        self.calculation: Optional[CalculationApi] = CalculationApi(self.client)
        self.connection: Optional[ConnectionApi] = ConnectionApi(self.client)
        self.export: Optional[ExportApi] = ExportApi(self.client)
        self.load_effect: Optional[LoadEffectApi] = LoadEffectApi(self.client)
        self.material: Optional[MaterialApi] = MaterialApi(self.client)
        self.member: Optional[MemberApi] = MemberApi(self.client)
        self.operation: Optional[OperationApi] = OperationApi(self.client)
        self.parameter: Optional[ParameterApi] = ParameterApi(self.client)
        self.presentation: Optional[PresentationApi] = PresentationApi(self.client)
        self.project: Optional[project_ext_api.ProjectExtApi] = project_ext_api.ProjectExtApi(self.client)
        self.report: Optional[ReportApi] = ReportApi(self.client)
        self.template: Optional[TemplateApi] = TemplateApi(self.client)

        logger.info(f"Client successfully created")  