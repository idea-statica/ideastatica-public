import asyncio
import json
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
    RUNTIME_CONFIG_NAME = "IdeaStatiCa.ConnectionRestApi.runtimeconfig.json"
    DOTNET_DOWNLOAD_URL = "https://dotnet.microsoft.com/download/dotnet/8.0"

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

        self._check_dotnet_runtimes(executable_path.parent)

        args = [str(executable_path), f"-port={self.port}"]
        self.service_process = subprocess.Popen(
            args,
            cwd=self.setup_dir,
            shell=False,
            stderr=subprocess.PIPE,
        )

        # Wait for the service to become ready asynchronously
        api_url_base = f"{self.LOCALHOST_URL}:{self.port}"
        api_url_heartbeat = f"{api_url_base}/{self.HEARTBEAT}"  # Adjust endpoint if needed
        ready, error_output = await self._wait_for_api_to_be_ready(api_url_heartbeat)
        if not ready:
            hint = (
                "\nHint: Make sure the required .NET runtimes are installed."
                f"\nDownload: {self.DOTNET_DOWNLOAD_URL}"
            )
            detail = f"\n--- process output ---\n{error_output}" if error_output else ""
            raise RuntimeError(f"API service failed to start at {api_url_base}{hint}{detail}")

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

    def _check_dotnet_runtimes(self, exe_dir: Path) -> None:
        """Checks that the required .NET runtimes are installed.

        Reads the runtimeconfig.json next to the exe, then compares the required
        framework names/versions against the output of ``dotnet --list-runtimes``.
        Raises RuntimeError with a clear install message if anything is missing.
        """
        runtimeconfig_path = exe_dir / self.RUNTIME_CONFIG_NAME
        if not runtimeconfig_path.exists():
            logger.debug("runtimeconfig.json not found, skipping runtime check")
            return

        try:
            with open(runtimeconfig_path, "r") as f:
                config = json.load(f)
            required = [
                (fw["name"], fw["version"])
                for fw in config.get("runtimeOptions", {}).get("frameworks", [])
            ]
        except Exception as exc:
            logger.debug(f"Could not parse runtimeconfig.json: {exc}")
            return

        if not required:
            return

        try:
            result = subprocess.run(
                ["dotnet", "--list-runtimes"],
                capture_output=True, text=True, timeout=10
            )
            installed_runtimes = result.stdout
        except FileNotFoundError:
            raise RuntimeError(
                ".NET is not installed or not on PATH.\n"
                f"Install the required runtimes from: {self.DOTNET_DOWNLOAD_URL}"
            )
        except Exception as exc:
            logger.debug(f"Could not query dotnet runtimes: {exc}")
            return

        missing = []
        for name, version in required:
            major = version.split(".")[0]
            # any patch of the required major is acceptable (roll-forward)
            if not any(f"{name} {major}." in line for line in installed_runtimes.splitlines()):
                missing.append(f"  {name} {version}")

        if missing:
            missing_list = "\n".join(missing)
            raise RuntimeError(
                f"The following required .NET runtimes are not installed:\n{missing_list}\n\n"
                "Install the missing runtimes:\n"
                f"  .NET 8 Desktop Runtime + ASP.NET Core 8 Runtime: {self.DOTNET_DOWNLOAD_URL}\n"
                "(Tip: the 'Windows Desktop Runtime' bundle covers all three requirements.)"
            )

    def _get_available_port(self) -> int:
        """Finds an available port."""
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
            sock.bind(("", 0))
            return sock.getsockname()[1]

    async def _wait_for_api_to_be_ready(self, api_url: str, timeout: int = 50):
        """Asynchronously waits for the API service to become ready.

        Returns:
            tuple[bool, str]: (ready, error_output) where error_output contains stderr
                              captured from the process if it exited prematurely.
        """
        async with aiohttp.ClientSession() as session:
            start_time = asyncio.get_event_loop().time()
            while asyncio.get_event_loop().time() - start_time < timeout:
                # Check if the process has already exited (e.g. missing .NET runtime)
                if self.service_process.poll() is not None:
                    stderr_bytes = self.service_process.stderr.read()
                    error_output = stderr_bytes.decode(errors="replace").strip() if stderr_bytes else ""
                    return False, error_output

                try:
                    async with session.get(api_url) as response:
                        if response.status == 200:
                            return True, ""
                except aiohttp.ClientError:
                    # Service not ready, wait and retry
                    await asyncio.sleep(3)
        return False, ""
