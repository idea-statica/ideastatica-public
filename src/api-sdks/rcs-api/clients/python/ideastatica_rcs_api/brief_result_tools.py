from . import helpers

def get_check_value(briefResults, checkType):
    # extract a check value from a check from an instance of brief results (brief results are provided by function Calculate)
    for res in briefResults.overall_items:
        if res.result_type == checkType:
            check_val = float(res.check_value)
            return check_val
    return None