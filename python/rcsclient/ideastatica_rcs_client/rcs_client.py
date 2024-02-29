import os
import requests
import subprocess
import time
import xmltodict
import json
import logging
from . import rcsproject 
from . import brief_result_tools

logger = logging.getLogger('RcsClient')

class RcsClient:
    def __init__(self, ideaStatiCaSetupDir, tcpPort):
        if(ideaStatiCaSetupDir.startswith('http')):
            logger.info(f"constructor {ideaStatiCaSetupDir} {tcpPort} - attaching to the existing endpoint")
            self.tcpPort = tcpPort
            self.restApiUrl = ideaStatiCaSetupDir
            self.rcsApiProcess = None
            self.ideaStatiCaSetupDir = ""
            self.rcsApiServicePath = f"{self.restApiUrl}:{self.tcpPort}"
        else:
            logger.info(f"constructor {ideaStatiCaSetupDir} {tcpPort} - starting new RcsRestApi")
            self.tcpPort = tcpPort
            self.restApiUrl = 'http://localhost'
            self.ideaStatiCaSetupDir = ideaStatiCaSetupDir
            self.rcsApiServicePath =  full_path = os.path.join(ideaStatiCaSetupDir, r'net6.0-windows', r'IdeaStatiCa.RcsRestApi.exe')
            self.rcsApiProcess = subprocess.Popen([self.rcsApiServicePath, f'-port={tcpPort}'])
            # TODO synchronization - we need to wait till server is fully running
            time.sleep(5)

    def __del__(self):
        # stop a process IdeaStatiCa.RcsRestApi.exe if it is running  
        if not self.rcsApiProcess is None:
            self.rcsApiProcess.kill()

    def getServiceDetails(self):
        # returns details of the connection to RCS service
        logger.info("getServiceDetails")
        res = {}
        res['TcpPort'] = self.tcpPort
        res['IdeaStatiCaSetupDir'] = self.ideaStatiCaSetupDir
        res['RcsApiServicePath'] = self.rcsApiServicePath
        return res

    def printServiceDetails(self):
        # print details of the connection to RCS service
        logger.info("printServiceDetails")
        print(self.getServiceDetails())

    def OpenProject(self, ideaPath):
        # Open IdeaRcs project. ideaPath is path to the project file on a disk to open
        logger.info(f"OpenProject ideaPath = '{ideaPath}'")
        binaryData = None
        with open(ideaPath, 'rb') as f:
            binaryData = f.read()

        if binaryData is None:
            raise Exception(f'Can not open the file {ideaPath}')
        
        headers = {"Content-Type": "application/octet-stream"}   
        
        response = requests.post(f'{self.restApiUrl}:{self.tcpPort}/Project/OpenProject', data=binaryData, headers=headers)
        if response.status_code == 200:
            self.projectId = response.text.replace('"', '')
            self.SetProjectSummary()
            logger.info(f"OpenProject passed projectId = '{self.projectId}'")
            return self.projectId
        else:
            logger.warn(f"OpenProject failed  {ideaPath} can not be open {response.content}")
            raise ValueError(f'file {ideaPath} can not be open {response.content}')
        
    def SetProjectSummary(self):
        # Get a project summary of the active project and store it locally 
        logger.info("SetProjectSummary")
        if self.projectId is None:
            logger.warning("Any project is not open - projectId is None")
            raise Exception(r'Any project is not open')
        
        self.projectSummaryData = None
        self.Project = None
        
        response = requests.get(f'{self.restApiUrl}:{self.tcpPort}/Project/{self.projectId}/ProjectSummary', headers={"Content-Type": "application/json"})
        if response.status_code == 200:
            parsed_data = xmltodict.parse(response.text)
            self.projectSummaryData = parsed_data[r'RcsProjectSummary']
            self.Project = rcsproject.RcsProject(parsed_data[r'RcsProjectSummary'])
            logger.info("SetProjectSummary passed projectId")
        else:
            logger.warning("SetProjectSummary FAILED")
            raise Exception(f'SetProjectSummary {response.content}')            

    def Calculate(self, sectionList):
        # Calculate selection of rcs sections. IDs of the sections to calculate are passed in the parameter sectionList
        logger.info(f"Calculate sectionList = '{sectionList}'")
        calculationParameters = { "Sections": sectionList }
        json_data = json.dumps(calculationParameters)
        response = requests.post(f'{self.restApiUrl}:{self.tcpPort}/Calculations/{self.projectId}/Calculate', json_data,
            headers={
                'Accept': 'application/xml',
                'Content-Type': 'application/json'
            })
        if response.status_code == 200:
            parsed_data = xmltodict.parse(response.text)
            brief_results = brief_result_tools.get_checks_in_section(parsed_data)
            
        
            return brief_results
        else:
            logger.warning(f"Calculate failed '{response.text}'")
            raise Exception(f"Calculation failed '{response.text}'")

    def GetResults(self, sectionList):
        # Get detailed check results for a selection of rcs sections. IDs of sections are passed in the parameter sectionList 
        logger.info(f"GetResults sectionList = '{sectionList}'")
        calculationParameters = { "Sections": sectionList }
        json_data = json.dumps(calculationParameters)
        response = requests.post(f'{self.restApiUrl}:{self.tcpPort}/Calculations/{self.projectId}/GetResults', json_data,
            headers={
                'Accept': 'application/xml',
                'Content-Type': 'application/json'
            })
        if response.status_code == 200:
            parsed_data = xmltodict.parse(response.text)
            return parsed_data
        else:
            logger.warning(f"GetResults failed '{response.text}'")
            raise Exception('GetResults failed')

    def UpdateReinforcedCrossSectionInSection(self, sectionId, newReinforcedCrossSectionId):
        # Get detailed check results for a selection of rcs sections. IDs of sections are passed in the parameter sectionList 
        logger.info(f"UpdateReinforcedCrossSectionInSection sectionId = '{sectionId}' newReinforcedCrossSectionId = {newReinforcedCrossSectionId}")
        rcsSection = {"Id":sectionId, "RCSId":newReinforcedCrossSectionId}
        json_data = json.dumps(rcsSection)
        response = requests.put(f'{self.restApiUrl}:{self.tcpPort}/Section/{self.projectId}/UpdateSection', json_data,
            headers={
                'Content-Type': 'application/json'
            })
        if response.status_code == 200:  
            parsed_data = response.json()
            self.SetProjectSummary()
            return parsed_data
        else:
            logger.warning(f"UpdateReinforcedCrossSectionInSection failed '{response.text}'")
            raise Exception('UpdateReinforcedCrossSectionInSection failed')

    def ImportReinforcedCrossSection(self, reinforcedCrossSectionImportSetting : rcsproject.ReinforcedCrossSectionImportSetting, template : str):
        # import an rcs template into the active project
        logger.info("ImportReinforcedCrossSection")
        reinforcedCrossSectionImportData = None
        if(reinforcedCrossSectionImportSetting.reinforcedCrossSectionId is None):
            reinforcedCrossSectionImportData = {"Setting": {"PartsToImport":reinforcedCrossSectionImportSetting.partsToImport}, "Template": template}
        else:
            reinforcedCrossSectionImportData = {"Setting": {"ReinforcedCrossSectionId":reinforcedCrossSectionImportSetting.reinforcedCrossSectionId, "PartsToImport":reinforcedCrossSectionImportSetting.partsToImport}, "Template": template}
        
        json_data = json.dumps(reinforcedCrossSectionImportData)

        response = requests.post(f'{self.restApiUrl}:{self.tcpPort}/Section/{self.projectId}/ImportReinforcedCrossSection', json_data,
            headers={
                'Content-Type': 'application/json'
            })
        
        if response.status_code == 200:  
            parsed_data = response.json()
            rfCssId = parsed_data['id']
            self.SetProjectSummary()
            return self.Project.ReinfCrossSections[str(rfCssId)]
        else:
            logger.warning(f"ImportReinforcedCrossSection failed '{response.text}'")
            raise Exception('ImportReinforcedCrossSection failed')

    def GetLoadingInSection(self, sectionId):
        # Get loading in the section sectionId
        logger.info(f"GetLoadingInSection sectionId = '{sectionId}'")
        response = requests.get(f'{self.restApiUrl}:{self.tcpPort}/Section/{self.projectId}/GetLoadingInSection?sectionId={sectionId}', 
            headers={
                'Content-Type': 'text/plain'
            })
        if response.status_code == 200:  
            parsed_data = xmltodict.parse(response.text)
            return parsed_data
        else:
            logger.warning(f"GetLoadingInSection failed '{response.text}'")
            raise Exception('GetLoadingInSection failed')

    def SetLoadingInSection(self, sectionId, loadingDict):
        # Set loading in the section sectionId 
        logger.info(f"SetLoadingInSection sectionId = '{sectionId}'")
        loadingXml = xmltodict.unparse(loadingDict)
        rcsSection = {"SectionId":sectionId, "LoadingXml":loadingXml}
        json_data = json.dumps(rcsSection)

        response = requests.post(f'{self.restApiUrl}:{self.tcpPort}/Section/{self.projectId}/SetLoadingInSection', json_data,
            headers={
                'Content-Type': 'application/json'
            })
        
        if response.status_code == 200:  
            return "Ok"
        else:
            logger.warning(f"GetLoadingInSection failed '{response.text}'")
            raise Exception('Update failed')

    @property
    def Project(self) -> rcsproject.RcsProject:
        # get the active RCS project
        return self._project
        
    @Project.setter
    def Project(self, value : rcsproject.RcsProject):
        self._project = value