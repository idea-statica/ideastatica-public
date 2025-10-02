import json
from . import helpers


def jsonRes_to_dict(json_data : str):
    detail_results = json.loads(json_data)
    return detail_results


def get_extremes(sectDetResDict):
        sectRes = sectDetResDict['sectionResult']
        extremes = sectRes['extremeResults'] 
        return extremes

def get_result_by_type(extreme, resultType):
    res = extreme['checkResults']
    for checkRes in res:
        if(checkRes['resultType'] == resultType):
            return checkRes['checkResults'][0]

    return None
