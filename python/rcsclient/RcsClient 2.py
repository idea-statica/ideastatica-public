import requests
import subprocess
import json
import xmltodict
#python -m pip install {missing_module} ex.(python -m pip install xmltodict)

apiServicePath = 'C:\\Dev\\IdeaStatiCa\\bin\\Debug\\net6.0-windows\\IdeaStatiCa.RcsRestApi.exe'
port = '5504'

def startApi():
    try:
        process = subprocess.Popen([apiServicePath, f'-port={port}'])
        #process = subprocess.Popen([apiServicePath])
        print("Rest.exe executed successfully!")
        return process
    except subprocess.CalledProcessError as e:
        print(f"Rest.exe failed with error: {e}")

def OpenProject(ideaPath):
    project_info = { "IdeaProjectPath": ideaPath }
    json_data = json.dumps(project_info)
    response = requests.post(f'http://localhost:{port}/Project/OpenProject', data=json_data, headers={"Content-Type": "application/json"})
    if response.status_code == 200:
        projectId = response.text.replace('"', '')
        print(f'Project opened with id: {projectId}')
        return projectId

def ProjectOverview(projectId):
    response = requests.get(f'http://localhost:{port}/Project/{projectId}/ProjectOverview', headers={"Content-Type": "application/json"})
    if response.status_code == 200:
        parsed_data = xmltodict.parse(response.text)
        return parsed_data

def SectionDetails(projectId, sectionList):
    calculationParameters = { "Sections": sectionList }
    json_data = json.dumps(calculationParameters)
    response = requests.post(f'http://localhost:{port}/Calculations/{projectId}/SectionDetails', json_data,
        headers={
            'Accept': 'application/xml',
            'Content-Type': 'application/json'
        })
    if response.status_code == 200:
        parsed_data = xmltodict.parse(response.text)
        return parsed_data

def CalculateResults(projectId, sectionList):
    calculationParameters = { "Sections": sectionList }
    json_data = json.dumps(calculationParameters)
    response = requests.post(f'http://localhost:{port}/Calculations/{projectId}/CalculateResults', json_data,
        headers={
            'Accept': 'application/xml',
            'Content-Type': 'application/json'
        })
    if response.status_code == 200:
        parsed_data = xmltodict.parse(response.text)
        return parsed_data
    else:
        print('Failed')






















process = startApi()

projectId = OpenProject("C:\\Users\\TomasKohoutek\\Desktop\\Data\\Beam1.idearcs")
overview = ProjectOverview(projectId)
details = SectionDetails(projectId, [1])
results = CalculateResults(projectId, [1 ,5])

process.terminate()