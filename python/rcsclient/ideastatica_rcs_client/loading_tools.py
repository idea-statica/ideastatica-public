from . import helpers

def get_extremes(loadingInSection):
    # Get extremes
    arr = loadingInSection['ArrayOfSectionExtremeBase']
    extremes = helpers.to_array(arr['SectionExtremeBase'])
    return extremes

def get_extreme(loadingInSection, extremeInx):
    # get extreme
    extremes = get_extremes(loadingInSection)
    return extremes[extremeInx]

def get_internalForce(loadingInSection, extremeInx, internalForceInx):
    #get internal force
    extreme = get_extreme(loadingInSection, extremeInx)
    loads = extreme['Loads']['Loads']['InputLoad']
    load = loads[internalForceInx];
    intForce = load['InternalForce']
    return intForce