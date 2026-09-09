import logging
from typing import Optional
from ideastatica_connection_api.connection_api_client import ConnectionApiClient

logger = logging.getLogger(__name__)

class ConnectionApiServiceAttacher:
    def __init__(self, base_url: str, client_application: Optional[str] = None,
                 client_application_version: Optional[str] = None):
        """
        :param base_url: URL of the running REST API service.
        :param client_application: Name of the application making the calls, for example
            "NorsokChecker" - a constant of the release, so it belongs to the factory rather than to
            a single call. Every client this factory creates is reported under it, which is what
            lets usage be attributed to an integration at all. Optional.
        :param client_application_version: Version of that application. Optional.
        """
        self.base_url = base_url
        self.client_application = client_application
        self.client_application_version = client_application_version

    def create_api_client(self) -> ConnectionApiClient:
        logger.info(f"Creating client attached to {self.base_url}")
        return ConnectionApiClient(self.base_url, self.client_application,
                                   self.client_application_version)