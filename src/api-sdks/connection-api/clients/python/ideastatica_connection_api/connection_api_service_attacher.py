import logging
from ideastatica_connection_api import Configuration
from ideastatica_connection_api.connection_api_client import ConnectionApiClient

logger = logging.getLogger(__name__)

class ConnectionApiServiceAttacher:
    """
    ConnectionApiServiceAttacher attaches to an existing Connection REST API service and provides a client to communicate with it.
    """
    def __init__(self, configuration: Configuration):
        """
        Constructor of the ConnectionApiServiceAttacher class that takes the configuration for the API client.
        
        :param configuration: Configuration object for the API client.
        """
        self.configuration = configuration

    def create_api_client(self) -> ConnectionApiClient:
        """
        Creates and returns a ConnectionApiClient attached to the specified host in the configuration.
        
        :return: ConnectionApiClient instance.
        """
        logger.info(f"Creating client attached to {self.configuration.host}")
        return ConnectionApiClient(configuration=self.configuration)