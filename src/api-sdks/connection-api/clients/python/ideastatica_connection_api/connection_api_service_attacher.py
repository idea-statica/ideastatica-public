import logging
from typing import Optional
from ideastatica_connection_api.connection_api_client import ConnectionApiClient

logger = logging.getLogger(__name__)

class ConnectionApiServiceAttacher:
    def __init__(self, base_url: str):
        self.base_url = base_url

    def create_api_client(self, client_application: Optional[str] = None,
                          client_application_version: Optional[str] = None) -> ConnectionApiClient:
        """Creates a client attached to the running service.

        :param client_application: Name of the application making the calls, so its usage can be
            told apart from every other caller's. Optional.
        :param client_application_version: Version of that application. Optional.
        """
        logger.info(f"Creating client attached to {self.base_url}")
        return ConnectionApiClient(self.base_url, client_application, client_application_version)