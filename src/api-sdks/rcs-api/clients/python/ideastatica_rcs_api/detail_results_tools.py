from . import helpers

def get_section_result_map(detailResults):
    allSectionResults = {}

    for s in detailResults:
        allSectionResults[s.id] = s.section_result

    return allSectionResults

def get_result_by_type(extreme, resultType):
    res = extreme.check_results
    for checkRes in res:
        if(checkRes.result_type == resultType):
            return checkRes.check_results[0]
        
    return None
