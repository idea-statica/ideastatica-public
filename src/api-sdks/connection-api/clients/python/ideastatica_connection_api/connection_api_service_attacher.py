import logging
from ideastatica_connection_api import Configuration
from ideastatica_connection_api.connection_api_client import ConnectionApiClient

logger = logging.getLogger(__name__)

class ConnectionApiServiceAttacher:
    def __init__(self, configuration: Configuration):
        self.configuration = configuration

    def create_api_client(self) -> ConnectionApiClient:
        logger.info(f"Creating client attached to {self.configuration.host}")
        return ConnectionApiClient(configuration= self.configuration) 