import logging
from ideastatica_connection_api import Configuration, ClientApi, CalculationApi, CalculationJobsApi, ConnectionApi, ConnectionLibraryApi, ExportApi, \
    LoadEffectApi, MaterialApi, MemberApi, OperationApi, ParameterApi, PresentationApi, ReportApi, TemplateApi, \
    ConversionApi, SettingsApi
import ideastatica_connection_api.api_client as api_client
import ideastatica_connection_api.api_ext.project_ext_api as project_ext_api
import ideastatica_connection_api.api_ext.export_ext_api as export_ext_api
import ideastatica_connection_api.api_ext.report_ext_api as report_ext_api
import ideastatica_connection_api.api_ext.connection_library_ext_api as connection_library_ext_api
from ideastatica_connection_api.client_application_identity import ClientApplicationIdentity
from typing import Optional

logger = logging.getLogger(__name__)

class ConnectionApiClient:
    def __init__(self, base_url: str, client_application: Optional[str] = None,
                 client_application_version: Optional[str] = None):
        """
        :param base_url: URL of the REST API service.
        :param client_application: Name of the application making the calls, for example
            "NorsokChecker" - a constant of the release, not anything about the machine, the
            project or the user. It is reported with every call the service serves, which is what
            lets usage be attributed to an integration at all; see
            :class:`ClientApplicationIdentity`. Optional.
        :param client_application_version: Version of that application. Optional.
        """
        self.base_url = base_url
        self.configuration = Configuration(host=self.base_url)

        self.client_application = ClientApplicationIdentity.format(client_application,
                                                                   client_application_version)

        self.client: Optional[api_client.ApiClient] = None
        self.client_id: Optional[str] = None

        self.calculation: Optional[CalculationApi] = None
        self.calculation_jobs: Optional[CalculationJobsApi] = None
        self.connection: Optional[ConnectionApi] = None
        self.connection_library: Optional[connection_library_ext_api.ConnectionLibraryExtApi] = None
        self.export: Optional[export_ext_api.ExportExtApi] = None
        self.load_effect: Optional[LoadEffectApi] = None
        self.material: Optional[MaterialApi] = None
        self.member: Optional[MemberApi] = None
        self.operation: Optional[OperationApi] = None
        self.parameter: Optional[ParameterApi] = None
        self.presentation: Optional[PresentationApi] = None
        self.project: Optional[project_ext_api.ProjectExtApi] = None
        self.report: Optional[report_ext_api.ReportExtApi] = None
        self.template: Optional[TemplateApi] = None
        self.conversion: Optional[ConversionApi] = None
        self.settings: Optional[SettingsApi] = None

    def __enter__(self):
        # Initialize the client with the provided config
        self.client = api_client.ApiClient(self.configuration)

        # Set before the first call, so the service can attribute the connect itself. A service
        # that does not know the header ignores it, so this is safe against any version.
        if self.client_application is not None:
            self.client.default_headers[ClientApplicationIdentity.HEADER_NAME] = self.client_application

        client_api = ClientApi(self.client)
        self.client_id = client_api.connect_client()

        logger.info(f"Client connected with client_id: {self.client_id} url: {self.configuration._base_path}")

        # Add your ClientId to HTTP header
        self.client.default_headers['ClientId'] = self.client_id        

        self.calculation = CalculationApi(self.client)
        self.calculation_jobs = CalculationJobsApi(self.client)
        self.connection = ConnectionApi(self.client)
        self.connection_library = connection_library_ext_api.ConnectionLibraryExtApi(self.client)
        self.export = export_ext_api.ExportExtApi(self.client)
        self.load_effect = LoadEffectApi(self.client)
        self.material = MaterialApi(self.client)
        self.member = MemberApi(self.client)
        self.operation = OperationApi(self.client)
        self.parameter = ParameterApi(self.client)
        self.presentation = PresentationApi(self.client)
        self.project = project_ext_api.ProjectExtApi(self.client)
        self.report = report_ext_api.ReportExtApi(self.client)
        self.template = TemplateApi(self.client)
        self.conversion = ConversionApi(self.client)
        self.settings = SettingsApi(self.client)

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