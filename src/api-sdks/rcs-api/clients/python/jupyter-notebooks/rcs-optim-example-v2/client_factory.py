import ideastatica_rcs_api
import ideastatica_rcs_api.rcs_api_service_attacher as rcs_api_service_attacher

baseUrl = "http://localhost:5000"

def create_client() -> ideastatica_rcs_api.rcs_api_service_attacher.RcsApiServiceAttacher:
    return rcs_api_service_attacher.RcsApiServiceAttacher(baseUrl).create_api_client()
