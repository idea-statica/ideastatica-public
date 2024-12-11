import logging
from ideastatica_rcs_api.rcs_api_client import RcsApiClient

logger = logging.getLogger(__name__)

class RcsApiServiceAttacher:
    def __init__(self, base_url: str):
        self.base_url = base_url

    def create_api_client(self) -> RcsApiClient:
        logger.info(f"Creating client attached to {self.base_url}")
        return RcsApiClient(self.base_url) 