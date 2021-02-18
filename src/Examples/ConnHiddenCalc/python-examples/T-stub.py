import clr
import sys
import json
import math
import os

script_dir  = os.path.abspath(os.curdir) 
print(f"script = {script_dir}")

# the name of idea connection project which is used in this script
ideaCon_filename = r"T-stub.ideaCon"

# the path to the idea connection installation directory
idea_path = r"C:\Program Files\IDEA StatiCa\StatiCa 21.0"

assembly_path = idea_path

connection_project_path = os.path.join(script_dir, 'projects', ideaCon_filename) 

# modify path to be able to load .net assemblies
sys.path.append(assembly_path)

# load the assembly IdeaStatiCa.Plugin which is responsible for communication with IdeaStatiCa 
clr.AddReference('IdeaStatiCa.Plugin')
from IdeaStatiCa.Plugin import ConnHiddenClientFactory

# create the instance of the client which communicates with IdeaStatiCa
factory = ConnHiddenClientFactory(idea_path)
ideaConnectionClient = factory.Create()

# open idea connection project 
ideaConnectionClient.OpenProject(connection_project_path)

# get information about connections in the project and print them
projectInfo = ideaConnectionClient.GetProjectInfo()
connections = projectInfo.Connections

for conn in connections:
    print(f'{conn.Name} { conn.Identifier}')

firstCon = connections[0]

# get parameters from the idea connection project
params_json_string = ideaConnectionClient.GetParametersJSON(firstCon.Identifier)
connectionParams = json.loads(params_json_string)

# get loading from the idea connection project
loading_json_string = ideaConnectionClient.GetConnectionLoadingJSON(firstCon.Identifier)
connectionLoading = json.loads(loading_json_string)

firstLoadCase = connectionLoading[0]
loadedSegments = firstLoadCase['forcesOnSegments']
secondLoadedSegment = loadedSegments[1]
normalForce = secondLoadedSegment['n']/1000

# create new csv file
import csv
with open('T-stub.csv', mode='w', newline='') as CSV_file:
    CSV_file = csv.writer(CSV_file, delimiter=',', quotechar='"', quoting=csv.QUOTE_MINIMAL)
    CSV_file.writerow(['Plate thickness [mm]', 'Applied load [kN]', 'Plastic strain in plates [%]', 'Bolt Utilization [%]', 'Average force in bolt [kN]'])    

# variables related to the thickness of the baseplate
pltThickStep = 0.002
pltThick = 0.004 - pltThickStep

plateStepInx = 0
# loop for modification of the base plate thickness
while plateStepInx < 10:
    pltThick += pltThickStep
    plateStepInx += 1   

    # modify the thickness of the base plate
    pltThickParam = connectionParams[0]
    pltThickParam['value'] = pltThick

    
    # update the model of the connection
    updated_params_json_string = json.dumps(connectionParams)
    ideaConnectionClient.ApplyParameters(firstCon.Identifier, updated_params_json_string)

    # read loading
    Load = loading_json_string

    # calculate the modified connection and get brief results
    briefResults = ideaConnectionClient.Calculate(firstCon.Identifier)
    summary_res_analysis = briefResults.ConnectionCheckRes[0].CheckResSummary[0]
    summary_res_plates = briefResults.ConnectionCheckRes[0].CheckResSummary[1]
    summary_res_bolts = briefResults.ConnectionCheckRes[0].CheckResSummary[2]
    print(f'Plate thickness = {pltThick*1000} mm')
    appliedLoad = summary_res_analysis.CheckValue*normalForce/100
    print(f'Normal force = {appliedLoad} kN')
    plateStrain = summary_res_plates.CheckValue
    print(f'Plates = {summary_res_plates.CheckValue} %')
    boltUt = summary_res_bolts.CheckValue
    print(f'Bolts = {summary_res_bolts.CheckValue} %')

    # get the results of the check of the first connection
    checkResults_json_string = ideaConnectionClient.GetCheckResultsJSON(firstCon.Identifier)
    checkResults = json.loads(checkResults_json_string)

    # results of bolts
    bolts = checkResults['bolts']
    listOfBolts = []   

    # print forces in all bolts
    boltInx = 1
    for key in bolts:
        bolt = bolts[key]
        forceInBolt = bolt['boltTensionForce']
        print(f'Tension force in bolt {boltInx} = {forceInBolt}')
        listOfBolts.append(forceInBolt)
        boltInx += 1 

    averageForceInBolt = sum(listOfBolts) / len(listOfBolts) / 1000

    # write into csv file
    with open('T-stub.csv', mode='a', newline='') as CSV_file:
        CSV_file = csv.writer(CSV_file, delimiter=',', quotechar='"', quoting=csv.QUOTE_MINIMAL)
        CSV_file.writerow([pltThick*1000, appliedLoad, plateStrain, boltUt, averageForceInBolt])

# close idea connection project
ideaConnectionClient.Close()
