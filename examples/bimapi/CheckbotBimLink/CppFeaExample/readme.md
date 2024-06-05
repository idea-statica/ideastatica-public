# C++/CLI implementation of Checkbot BIM Link Example for an FEA Application

[CppFeaApp](./CppFeaApp/) - startup project : implementation of a fake FEA aplication (native MFC exe project)

[CppFeaApi](./CppFeaApi/) - sample implementation of a fake FEA API (native c++ dll project)

[CppFeaApiWrapper](./CppFeaApiWrapper/) - implementation of BIM API, Importers anf Checkbot controller (c++/cli project which generates BIM API model from data provided by CppFeaApi, controlling of Checkbot)

[CopyToOutputApp](./CopyToOutputApp/) - .net app which is responsible for copying all needed files to output (MFC project doesn't do it)

[ImporterWrappers](./ImporterWrappers/) - implementation of abstract BIM Api importers in c# (how to move it to CppFeaApiWrapper ?? it is a qustion)

