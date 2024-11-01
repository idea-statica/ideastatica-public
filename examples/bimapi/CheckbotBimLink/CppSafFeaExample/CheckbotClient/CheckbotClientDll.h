#ifdef  CHECKBOTCLIENT_EXPORTS 
/*Enabled as "export" while compiling the dll project*/
#define CHECKBOTCLIENTDLL_EXPORTS __declspec(dllexport)  
#else
/*Enabled as "import" in the Client side for using already created dll file*/
#define CHECKBOTCLIENTDLL_EXPORTS __declspec(dllimport)  
#endif