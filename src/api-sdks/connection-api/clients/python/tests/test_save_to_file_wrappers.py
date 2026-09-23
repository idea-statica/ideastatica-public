from ideastatica_connection_api import ApiClient, Configuration
from ideastatica_connection_api.api_ext.export_ext_api import ExportExtApi
from ideastatica_connection_api.api_ext.report_ext_api import ReportExtApi
from ideastatica_connection_api.rest import RESTResponse

# Every file-producing operation has a wrapper that saves the produced file: it asks the service for
# the raw bytes and writes them, unchanged, to the path the caller names. These need no running
# service - the transport is replaced by a canned answer.

PROJECT_ID = "6f9619ff-8b86-d011-b42d-00c04fc964ff"
ZIP_CONTENT = b"PK\x03\x04\x0a\x00\x00\x00"
DWG_CONTENT = b"AC1027\x00\x00"


class _ProducedFile:
    """What urllib3 hands back for a 200 whose body is the produced file."""

    def __init__(self, content):
        self.status = 200
        self.reason = "OK"
        self.data = content
        self.headers = {"content-type": "application/octet-stream"}


def _service_producing(content, monkeypatch):
    api_client = ApiClient(Configuration(host="http://localhost:5000"))
    requests = []

    def request(method, url, headers=None, body=None, post_params=None, _request_timeout=None):
        requests.append((method, url))
        return RESTResponse(_ProducedFile(content))

    monkeypatch.setattr(api_client.rest_client, "request", request)
    return api_client, requests


def test_save_report_html_zip_writes_the_zip_the_service_returns(tmp_path, monkeypatch):
    api_client, requests = _service_producing(ZIP_CONTENT, monkeypatch)
    target = tmp_path / "report.zip"

    ReportExtApi(api_client).save_report_html_zip(PROJECT_ID, 7, str(target))

    assert target.read_bytes() == ZIP_CONTENT
    assert requests == [
        ("GET", f"http://localhost:5000/api/5/projects/{PROJECT_ID}/connections/7/reports/htmlZip")
    ]


def test_export_dwg_file_writes_the_drawing_the_service_returns(tmp_path, monkeypatch):
    api_client, requests = _service_producing(DWG_CONTENT, monkeypatch)
    target = tmp_path / "connection.dwg"

    ExportExtApi(api_client).export_dwg_file(PROJECT_ID, 7, str(target))

    assert target.read_bytes() == DWG_CONTENT
    assert requests == [
        ("GET", f"http://localhost:5000/api/5/projects/{PROJECT_ID}/connections/7/export-dwg")
    ]
