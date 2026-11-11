#pragma once

#include <afxwin.h>

#include "CppFeaExampleApi/FeaApi.h"

/// <summary>
/// The window of the example FEA application. It starts Checkbot linked to the FEA model.
/// </summary>
class CMainDialog : public CDialog
{
public:
	explicit CMainDialog(const CppFeaExampleApi::IFeaApi& fea);

protected:
	BOOL OnInitDialog() override;
	void DoDataExchange(CDataExchange* pDX) override;

	afx_msg void OnStartCheckbot();

	DECLARE_MESSAGE_MAP()

private:
	const CppFeaExampleApi::IFeaApi& m_fea;
	HICON m_icon;
	CString m_checkbotPath;
	CString m_projectPath;
};
