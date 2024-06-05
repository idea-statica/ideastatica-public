#include "..\CppFeaApi\NativeFeaApi.h"

// CppFeaAppDlg.h : header file
//

#pragma once


// CCppFeaDlg dialog
class CCppFeaDlg : public CDialogEx
{
// Construction
public:
	CCppFeaDlg(CWnd* pParent = nullptr);	// standard constructor

// Dialog Data
#ifdef AFX_DESIGN_TIME
	enum { IDD = IDD_CPPFEA_DIALOG };
#endif

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support


// Implementation
protected:
	HICON m_hIcon;
	NativeFeaApi* pApi;
	CString m_checkbotPath;
	CString m_feaProjectPath;
	int m_checkbotStatus = 0;

	// Generated message map functions
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()


public:
	afx_msg void OnBnClickedButton1();
	afx_msg void OnClose();
};
