#include "CppFeaExampleApp.h"

#include "MainDialog.h"

// Gives the buttons and text boxes the look of the current Windows version.
#pragma comment(linker, "/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='*' publicKeyToken='6595b64144ccf1df' language='*'\"")

CCppFeaExampleApp theApp;

BOOL CCppFeaExampleApp::InitInstance()
{
	CWinApp::InitInstance();

	m_fea = CppFeaExampleApi::CreateFeaApi();

	CMainDialog dialog(*m_fea);
	m_pMainWnd = &dialog;
	dialog.DoModal();

	// FALSE ends the application now that its window is closed, instead of starting a message loop.
	return FALSE;
}
