import asyncio
import os
import aiohttp
import logging
import subprocess
from pathlib import Path
import socket
from typing import Optional
from ideastatica_connection_api.connection_api_client import ConnectionApiClient

logger = logging.getLogger(__name__)

class ConnectionApiServiceRunner:
    LOCALHOST_URL = "http://127.0.0.1"
    HEARTBEAT = "heartbeat"
    API_EXECUTABLE_NAME = "IdeaStatiCa.ConnectionRestApi.exe"

    def __init__(self, setup_dir: str):
        self.setup_dir = setup_dir
        self.service_process: Optional[subprocess.Popen] = None
        self.port: Optional[int] = None

    async def __aenter__(self):
        """Start the API service when entering the asynchronous context."""
        logger.info("Starting the API service...")
        self.port = self._get_available_port()
        executable_path = Path(os.path.join(self.setup_dir, self.API_EXECUTABLE_NAME))

        if not executable_path.exists():
            raise FileNotFoundError(f"API executable not found at path: {executable_path}")

        args = [str(executable_path), f"-port={self.port}"]
        self.service_process = subprocess.Popen(args, cwd=self.setup_dir, shell=False)

        # Wait for the service to become ready asynchronously
        api_url_base = f"{self.LOCALHOST_URL}:{self.port}"
        api_url_heartbeat = f"{api_url_base}/{self.HEARTBEAT}"  # Adjust endpoint if needed
        if not await self._wait_for_api_to_be_ready(api_url_heartbeat):
            raise RuntimeError(f"API service failed to start at {api_url_base}")

        logger.info(f"API service started at {api_url_base}")
        return self

    async def __aexit__(self, exc_type, exc_value, traceback):
        """Stop the API service when exiting the asynchronous context."""
        logger.info("Stopping the API service...")
        if self.service_process:
            self.service_process.terminate()
            self.service_process.wait()
            self.service_process = None
        logger.info("API service stopped.")

    def create_api_client(self) -> ConnectionApiClient:
        """Creates and returns an IdeaStatiCaClient attached to the API service."""
        if self.port is None:
            raise RuntimeError("The service must be started before creating a client.")

        base_url = f"{self.LOCALHOST_URL}:{self.port}"
        client = ConnectionApiClient(base_url)
        logger.info(f"Client created for service at {base_url}")
        return client

    def _get_available_port(self) -> int:
        """Finds an available port."""
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
            sock.bind(("", 0))
            return sock.getsockname()[1]

    async def _wait_for_api_to_be_ready(self, api_url: str, timeout: int = 50) -> bool:
        """Asynchronously waits for the API service to become ready."""

        async with aiohttp.ClientSession() as session:
            start_time = asyncio.get_event_loop().time()
            while asyncio.get_event_loop().time() - start_time < timeout:
                try:
                    async with session.get(api_url) as response:
                        if response.status == 200:
                            return True
                except aiohttp.ClientError:
                    # Service not ready, wait and retry
                    await asyncio.sleep(3)
        return False
