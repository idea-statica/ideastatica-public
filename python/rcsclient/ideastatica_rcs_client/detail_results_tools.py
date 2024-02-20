from . import helpers

def get_section_results(detailResults):
    result_array = helpers.to_array(detailResults["ArrayOfRcsSectionResultDetailed"]['RcsSectionResultDetailed'])
    allSectionResults = {}

    for s in result_array:
        allSectionResults[s["Id"]] = s

    return allSectionResults

def get_extreme_results(allSectionResults, section, extreme):
    return allSectionResults[section]["SectionResult"]["ExtremeResults"]["ConcreteCheckResults"]["CheckResults"]["ConcreteCheckResult"]

def get_result_by_type(extreme_results, resultType):
    for res in extreme_results:
        if(res["ResultType"] == resultType):
            return res["CheckResults"]["ConcreteCheckResultBase"]
        
    return None