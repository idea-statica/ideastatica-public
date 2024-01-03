def to_array(obj):
    if(type(obj) == list):
        return obj
    else:
        return [obj]
    
def to_dictionary(objects, keyName : str, valueName : str):
    result = {}
    for item in objects:
        val = item
        if(valueName is not None):
            val =  item[valueName]

        result[str(item[keyName])] = val
    return result
