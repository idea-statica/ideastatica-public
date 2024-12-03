import logging
from ideastatica_connection_api.connection_api_client import ConnectionApiClient

logger = logging.getLogger(__name__)

class ConnectionApiServiceAttacher:
    def __init__(self, base_url: str):
        self.base_url = base_url

    def create_api_client(self) -> ConnectionApiClient:
        logger.info(f"Creating client attached to {self.base_url}")
        return ConnectionApiClient(self.base_url) 