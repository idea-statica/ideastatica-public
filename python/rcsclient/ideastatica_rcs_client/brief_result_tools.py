from . import helpers

def get_checks_in_section(briefResults):
    # parse a raw brief rcs results and return a dictionary of brief results where a key is section id   
    allItemsInSection = helpers.to_array(briefResults["ArrayOfRcsSectionResultOverview"]['RcsSectionResultOverview'])
    results_in_section = dict(map(lambda s: (s["SectionId"], helpers.to_dictionary((s.get("OverallItems")).get('ConcreteCheckResultOverallItem'), "ResultType", None)), allItemsInSection)) 
    return results_in_section

def get_check_value(briefResults, checkType, sectionId):
    # extract a check value from a check from an instance of brief results (brief results are provided by function Calculate)
    check = briefResults[str(sectionId)][checkType]
    capacity_check_val = float(check["CheckValue"])
    return capacity_check_val

def print_sections_in_project(rcs_project):
     # print all sections in the rcs project
    print("Sections in the project")
    for sec in rcs_project.Sections.values():
        print(f'secId: {sec.Id} \'{sec.Description}\' rfCssId = {sec.ReinforcedCrossSectionId} memberId = {sec.CheckMemberId}')

def print_reinforced_cross_sections_in_project(rcs_project):
    # print all reinforced cross-sections in the rcs project
    print('Reinforced cross-sections in the project')
    for rfCss in rcs_project.ReinfCrossSections.values():
        print(f'{rfCss.Id} \'{rfCss.Name}\' {rfCss.CssId}')   

def print_capacity_check_vals(rcsProject, sectionIds, br):
    for secId in sectionIds:
        try:
            print("Section \'{0}\' capacity {1}".format(rcsProject.Sections[secId].Description, get_check_value(br, "Capacity", secId)))
        except:
            print("No results of capacity check in section", secId)