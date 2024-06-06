# C++/CLI implementation of Checkbot BIM Link Example for an FEA Application

[CppFeaApp](./CppFeaApp/) - startup project : implementation of a fake FEA aplication (native MFC exe project)

[CppFeaApi](./CppFeaApi/) - sample implementation of a fake FEA API (native c++ dll project)

[CppFeaApiWrapper](./CppFeaApiWrapper/) - implementation of BIM API classes,  BIM API Importers and Checkbot controller (c++/cli project which generates BIM API model from data provided by CppFeaApi, controlling of Checkbot)

[CopyToOutputApp](./CopyToOutputApp/) - .net app which is responsible for copying all needed files to output (MFC project doesn't do it)

[ImporterWrappers](./ImporterWrappers/) - implementation of abstract BIM Api importers in c# (how to move it to CppFeaApiWrapper ?? it is a qustion)


![CppFeaExample](https://github.com/idea-statica/ideastatica-public/blob/main/docs/Images/cpp-fea-example.png?raw=true)

## How it works

After clicking on the button 'Start Checkbot' [CCppFeaDlg::OnRunCheckbotClick](https://github.com/idea-statica/ideastatica-public/blob/1c1f9daf523099e0582672eeafb3fdfb21de2c8c/examples/bimapi/CheckbotBimLink/CppFeaExample/CppFeaApp/CppFeaAppDlg.cpp#L199) the instance of [NativeFeaApi](https://github.com/idea-statica/ideastatica-public/blob/1c1f9daf523099e0582672eeafb3fdfb21de2c8c/examples/bimapi/CheckbotBimLink/CppFeaExample/CppFeaApi/NativeFeaApi.h#L10) is created.

The instance is passed to [CheckbotController](examples/bimapi/CheckbotBimLink/CppFeaExample/CppFeaApiWrapper/CheckbotController.h) by calling exported function [RunCheckbot](https://github.com/idea-statica/ideastatica-public/blob/1c1f9daf523099e0582672eeafb3fdfb21de2c8c/examples/bimapi/CheckbotBimLink/CppFeaExample/CppFeaApiWrapper/CheckBotControlFunctions.h#L13).
BIM API importers are registered in [CheckbotController::BuildContainer](https://github.com/idea-statica/ideastatica-public/blob/1c1f9daf523099e0582672eeafb3fdfb21de2c8c/examples/bimapi/CheckbotBimLink/CppFeaExample/CppFeaApiWrapper/CheckbotController.cpp#L173). Finally Checkbot is started.

After clicking on the button 'Connections'

![Import connections button](https://github.com/idea-statica/ideastatica-public/blob/main/docs/Images/Checkbot-Import-Connections.png?raw=true)


[Model::GetUserSelection()](https://github.com/idea-statica/ideastatica-public/blob/b82b74f7b83f6e8f4649295ac3244131fd07a9c9/examples/bimapi/CheckbotBimLink/CppFeaExample/CppFeaApiWrapper/Model.cpp#L45) is called to prepare and send data to Checkbot.
