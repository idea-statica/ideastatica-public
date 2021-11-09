#Registration of the plugin for RSTAB

1. register COM: regasm /codebase IdeaRstabPlugin.dll

2. add following lines to RSTAB.ini in RSTAB installation directory:
ModuleName3=IDEA StatiCa New Api
ModuleDescription3=IDEA StatiCa
ModuleClassName3=IdeaRstabPlugin.StartCCMCommand