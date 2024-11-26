import logging
from ideastatica_connection_api import Configuration
from ideastatica_connection_api.ideastatica_client import IdeaStatiCaClient

logger = logging.getLogger(__name__)

class ApiServiceAttacher:
    def __init__(self, configuration: Configuration):
        self.configuration = configuration

    def create_api_client(self, fileName: str) -> IdeaStatiCaClient:
        logger.info(f"Creating client attached to {self.configuration.host}")
        return IdeaStatiCaClient(configuration= self.configuration, fileName= fileName) 