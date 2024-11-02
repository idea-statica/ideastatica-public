
#ifdef  SAFPROVIDER_EXPORTS 
/*Enabled as "export" while compiling the dll project*/
#define SAFPROVIDERDLL_EXPORTS __declspec(dllexport)  
#else
/*Enabled as "import" in the Client side for using already created dll file*/
#define SAFPROVIDERDLL_EXPORTS __declspec(dllimport)  
#endif