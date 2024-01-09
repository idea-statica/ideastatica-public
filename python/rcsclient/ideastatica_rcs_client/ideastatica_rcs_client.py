import os
import requests
import subprocess
import time
import xmltodict
import json
from . import rcsproject 
from . import brief_result_tools

class ideastatica_rcs_client:
    def __init__(self, ideaStatiCaSetupDir, tcpPort):
        self.tcpPort = tcpPort
        self.ideaStatiCaSetupDir = ideaStatiCaSetupDir
        self.rcsApiServicePath =  full_path = os.path.join(ideaStatiCaSetupDir, r'net6.0-windows', r'IdeaStatiCa.RcsRestApi.exe')
        self.rcsApiProcess = subprocess.Popen([self.rcsApiServicePath, f'-port={tcpPort}'])
        # TODO synchronization - we need to wait till server is fully running
        time.sleep(5)

    def __del__(self):
        # stop a process IdeaStatiCa.RcsRestApi.exe if it is running  
        if not self.rcsApiProcess is None:
            self.rcsApiProcess.kill()

    def printServiceDetails(self):
        # print details of the connection to RCS service
        print(f"TcpPort : {self.tcpPort}")
        print(f"IdeaStatiCaSetupDir : {self.ideaStatiCaSetupDir}")
        print(f"RcsApiServicePath : {self.rcsApiServicePath}")

    def OpenProject(self, ideaPath):
        # Open IdeaRcs project. ideaPath is path to the project file on a disk to open
        binaryData = None
        with open(ideaPath, 'rb') as f:
            binaryData = f.read()

        if binaryData is None:
            raise Exception(f'Can not open the file {ideaPath}')
        
        headers = {"Content-Type": "application/octet-stream"}   
        
        response = requests.post(f'http://localhost:{self.tcpPort}/Project/OpenProject', data=binaryData, headers=headers)
        if response.status_code == 200:
            self.projectId = response.text.replace('"', '')
            self.SetProjectSummary()
            return self.projectId
        else:
            raise ValueError(f'file {ideaPath} can not be open {response.content}')
        
    def SetProjectSummary(self):
        # Get a project summary of the active project and store it locally 
        if self.projectId is None:
            raise Exception(r'Any project is not open')
        
        self.projectSummaryData = None
        self.Project = None
        
        response = requests.get(f'http://localhost:{self.tcpPort}/Project/{self.projectId}/ProjectSummary', headers={"Content-Type": "application/json"})
        if response.status_code == 200:
            parsed_data = xmltodict.parse(response.text)
            self.projectSummaryData = parsed_data[r'RcsProjectSummary']
            self.Project = rcsproject.RcsProject(parsed_data[r'RcsProjectSummary'])

    def Calculate(self, sectionList):
        # Calculate selection of rcs sections. IDs of the sections to calculate are passed in the parameter sectionList
        calculationParameters = { "Sections": sectionList }
        json_data = json.dumps(calculationParameters)
        response = requests.post(f'http://localhost:{self.tcpPort}/Calculations/{self.projectId}/Calculate', json_data,
            headers={
                'Accept': 'application/xml',
                'Content-Type': 'application/json'
            })
        if response.status_code == 200:
            parsed_data = xmltodict.parse(response.text)
            brief_results = brief_result_tools.get_checks_in_section(parsed_data)
            return brief_results
        else:
            raise Exception('Calculation failed')

    def GetResults(self, sectionList):
        # Get detailed check results for a selection of rcs sections. IDs of sections are passed in the parameter sectionList 
        calculationParameters = { "Sections": sectionList }
        json_data = json.dumps(calculationParameters)
        response = requests.post(f'http://localhost:{self.tcpPort}/Calculations/{self.projectId}/GetResults', json_data,
            headers={
                'Accept': 'application/xml',
                'Content-Type': 'application/json'
            })
        if response.status_code == 200:
            parsed_data = xmltodict.parse(response.text)
            return parsed_data
        else:
            raise Exception('GetResults failed')

    def UpdateReinforcedCrossSectionInSection(self, sectionId, newReinforcedCrossSectionId):
        # Get detailed check results for a selection of rcs sections. IDs of sections are passed in the parameter sectionList 
        rcsSection = {"Id":sectionId, "RCSId":newReinforcedCrossSectionId}
        json_data = json.dumps(rcsSection)
        response = requests.put(f'http://localhost:{self.tcpPort}/Section/{self.projectId}/UpdateSection', json_data,
            headers={
                'Content-Type': 'application/json'
            })
        if response.status_code == 200:  
            parsed_data = response.json()
            self.SetProjectSummary()
            return parsed_data
        else:
            raise Exception('Update failed')

    def ImportReinforcedCrossSection(self, reinforcedCrossSectionImportSetting : rcsproject.ReinforcedCrossSectionImportSetting, template : str):
        # import an rcs template into the active project
        reinforcedCrossSectionImportData = None
        if(reinforcedCrossSectionImportSetting.reinforcedCrossSectionId is None):
            reinforcedCrossSectionImportData = {"Setting": {"PartsToImport":reinforcedCrossSectionImportSetting.partsToImport}, "Template": template}
        else:
            reinforcedCrossSectionImportData = {"Setting": {"ReinforcedCrossSectionId":reinforcedCrossSectionImportSetting.reinforcedCrossSectionId, "PartsToImport":reinforcedCrossSectionImportSetting.partsToImport}, "Template": template}
        
        json_data = json.dumps(reinforcedCrossSectionImportData)

        response = requests.post(f'http://localhost:{self.tcpPort}/Section/{self.projectId}/ImportReinforcedCrossSection', json_data,
            headers={
                'Content-Type': 'application/json'
            })
        
        if response.status_code == 200:  
            parsed_data = response.json()
            rfCssId = parsed_data['id']
            self.SetProjectSummary()
            return self.Project.ReinfCrossSections[str(rfCssId)]
        else:
            raise Exception('ImportReinforcedCrossSection failed')


    @property
    def Project(self) -> rcsproject.RcsProject:
        # get the active RCS project
        return self._project
        
    @Project.setter
    def Project(self, value : rcsproject.RcsProject):
        self._project = value