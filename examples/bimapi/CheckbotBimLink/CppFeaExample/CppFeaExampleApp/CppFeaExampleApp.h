#pragma once

#include <afxwin.h>

#include "CppFeaExampleApi/FeaApi.h"

#include <memory>

/// <summary>
/// The example FEA application. It keeps the FEA model for as long as it runs, and shows the window that starts
/// Checkbot.
/// </summary>
class CCppFeaExampleApp : public CWinApp
{
public:
	BOOL InitInstance() override;

private:
	std::unique_ptr<CppFeaExampleApi::IFeaApi> m_fea;
};
