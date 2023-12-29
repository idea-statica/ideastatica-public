from . import helpers

def get_checks_in_section(briefResults):
    # parse a raw brief rcs results and return a dictionary of brief results where a key is section id   
    allItemsInSection = helpers.to_array(briefResults["ArrayOfRcsSectionResultOverview"]['RcsSectionResultOverview'])
    results_in_section = dict(map(lambda s: (s["SectionId"], helpers.to_dictionary((s.get("OverallItems")).get('ConcreteCheckResultOverallItem'), "ResultType", None)), allItemsInSection)) 
    return results_in_section