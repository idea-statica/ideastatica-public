import os
import logging
from ideastatica_connection_api import Configuration, ClientApi, CalculationApi, ConnectionApi, ExportApi, LoadEffectApi, MaterialApi, MemberApi, OperationApi, ParameterApi, PresentationApi, ReportApi, TemplateApi
import ideastatica_connection_api.api_client as api_client
import ideastatica_connection_api.api_ext.project_ext_api as project_ext_api
from typing import Optional

logger = logging.getLogger(__name__)

class IdeaStatiCaClient:
    def __init__(self, configuration: Configuration, fileName: str):
        self.fileName = fileName
        self.configuration = configuration

        self.client: Optional[api_client.ApiClient] = None
        self.project_id: Optional[str] = None
        self.client_id: Optional[str] = None
        self.project: Optional[project_ext_api.ProjectExtApi] = None

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
        logger.info(f"Opening the project: {self.fileName}")
        try:
            with open(self.fileName, 'rb') as file:
                byte_array = file.read()

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

            _, ext = os.path.splitext(self.fileName)
            ext = ext.lower()

            if ext == '.ideacon':
                uploadRes = self.project.open_project(idea_con_file=byte_array, _content_type='multipart/form-data')
                self.project_id = uploadRes.project_id

            elif ext == '.xml' or ext == '.iom':
                uploadRes = self.project.import_iom(byte_array, _content_type='multipart/form-data')
                self.project_id = uploadRes.project_id
                
            else:
                raise ValueError(f"Unsupported file type: {ext}")

            logger.info(f"Open project_id: {self.project_id}")  

            return self
        
        except Exception as e:
            logger.error(f"Failed to open the project: {self.fileName}", exc_info=True)
            raise e

    def __exit__(self, exc_type, exc_value, traceback):
        # Perform any necessary cleanup
        try:
            if self.project:
                logger.info(f"Closing project project_id:{self.project_id} client_id: {self.client_id}")
                self.project.close_project(self.project_id)
                self.project = None
        finally:
            self.project = None