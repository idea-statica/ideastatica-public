import os
import requests
import subprocess
import time
import xmltodict
import rcsproject

class ideastatica_rcs_client:
    def __init__(self, ideaStatiCaSetupDir, tcpPort):
        self.tcpPort = tcpPort
        self.ideaStatiCaSetupDir = ideaStatiCaSetupDir
        self.rcsApiServicePath =  full_path = os.path.join(ideaStatiCaSetupDir, r'net6.0-windows', r'IdeaStatiCa.RcsRestApi.exe')
        self.rcsApiProcess = subprocess.Popen([self.rcsApiServicePath, f'-port={tcpPort}'])
        # TODO synchronization - we need to wait till server is fully running
        time.sleep(5)

    def __del__(self):
        if not self.rcsApiProcess is None:
            self.rcsApiProcess.kill()

        print("Object destroyed")

    def printServiceDetails(self):
        print(f"TcpPort : {self.tcpPort}")
        print(f"IdeaStatiCaSetupDir : {self.ideaStatiCaSetupDir}")
        print(f"RcsApiServicePath : {self.rcsApiServicePath}")

    def OpenProject(self, ideaPath):
        binaryData = None
        with open(ideaPath, 'rb') as f:
            binaryData = f.read()

        if binaryData is None:
            raise Exception(f'Can not open the file {ideaPath}')
        
        headers = {"Content-Type": "application/octet-stream"}   
        
        response = requests.post(f'http://localhost:{self.tcpPort}/Project/OpenProject', data=binaryData, headers=headers)
        if response.status_code == 200:
            self.projectId = response.text.replace('"', '')
            print(f'Project opened with id: {self.projectId}')
            self.SetProjectSummary()
            return self.projectId
        else:
            raise ValueError(f'file {ideaPath} can not be open {response.content}')
        
    def SetProjectSummary(self):
        if self.projectId is None:
            raise Exception(r'Any project is not open')
        
        self.projectSummaryData = None
        self.Project = None
        
        response = requests.get(f'http://localhost:{self.tcpPort}/Project/{self.projectId}/ProjectSummary', headers={"Content-Type": "application/json"})
        if response.status_code == 200:
            parsed_data = xmltodict.parse(response.text)

            self.projectSummaryData = parsed_data[r'RcsProjectSummaryModel']

            self.Project = rcsproject.RcsProject(parsed_data[r'RcsProjectSummaryModel'])
        
    @property
    def Project(self) -> rcsproject.RcsProject:
        return self._project
        
    @Project.setter
    def Project(self, value : rcsproject.RcsProject):
        self._project = value