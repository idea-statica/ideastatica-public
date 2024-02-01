import xmltodict

class RcsSection:
    def __init__(self, sectionData): 
        self.Id = sectionData['Id']
        self.Description = sectionData['Description']
        self.ReinforcedCrossSectionId = sectionData['RCSId']
        self.CheckMemberId = sectionData['CheckMemberId']

    @property
    def Id(self) -> int:
        return self._id
            
    @Id.setter
    def Id(self, value : int):
        self._id = value

    @property
    def Description(self) -> str:
        return self._description
            
    @Description.setter
    def Description(self, value : str):
        self._description = value

    @property
    def RfCssId(self) -> int:
        return self._rfCssId
            
    @RfCssId.setter
    def RfCssId(self, value : int):
        self._rfCssId = value

    @property
    def CheckMemberId(self) -> int:
        return self._checkMemberId
            
    @CheckMemberId.setter
    def CheckMemberId(self, value : int):
        self._checkMemberId = value        

class ReinforcedCrossSection:
    def __init__(self, rfCssData):  
        self.Id =  rfCssData['Id']
        self.Name = rfCssData['Name']
        self.CssId = rfCssData['CrossSectionId']

    @property
    def Id(self) -> int:
        return self._id
            
    @Id.setter
    def Id(self, value : int):
        self._id = value

    @property
    def Name(self) -> str:
        return self._name
            
    @Name.setter
    def Name(self, value : str):
        self._name = value    
        
    @property
    def CssId(self) -> int:
        return self._cssId
            
    @CssId.setter
    def CssId(self, value : int):
        self._cssId = value                              

class RcsProject:
    def __init__(self, projectData):
        sections = projectData['Sections']
        sect = sections['RcsSection']

        sectDict = {}
        if(type(sect) == list):
            for sect in sect:
                sec = RcsSection(sect)
                sectDict[sec._id] = sec
        else:
            sec = RcsSection(sect)
            sectDict[sec._id] = sec   

        self.Sections = sectDict

        reinfCss = projectData['ReinforcedCrossSections']
        reinfCssModel = reinfCss['RcsReinforcedCrossSection']
        reinfCssDict = {}
        if(type(reinfCssModel) == list):
            for reinfCssData in reinfCssModel:
                newReinfCss = ReinforcedCrossSection(reinfCssData)
                reinfCssDict[newReinfCss.Id] = newReinfCss
        else:
            newReinfCss = ReinforcedCrossSection(reinfCssModel)   
            reinfCssDict[newReinfCss.Id] = newReinfCss  

        self.ReinfCrossSections = reinfCssDict

    @property
    def Sections(self) -> dict[int, RcsSection]:
        return self._sections
        
    @Sections.setter
    def Sections(self, value : dict[int, RcsSection]):
        self._sections = value

    @property
    def ReinfCrossSections(self) -> dict[int, ReinforcedCrossSection]:
        return self._reinfCrossSections
        
    @ReinfCrossSections.setter
    def ReinfCrossSections(self, value : dict[int, ReinforcedCrossSection]):
        self._reinfCrossSections = value

class ReinforcedCrossSectionImportSetting:
    def __init__(self, reinforcedCrossSectionId, partsToImport):
        self.reinforcedCrossSectionId = reinforcedCrossSectionId
        self.partsToImport = partsToImport
