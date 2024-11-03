
// CppFeaAppDlg.cpp : implementation file
//

#include "pch.h"
#include "framework.h"
#include "CppFeaApp.h"
#include "CppFeaAppDlg.h"
#include "afxdialogex.h"
#include "..\SafProvider\NativeFeaApi.h"
#include <vector>
#include <string>
#include <windows.h>
#include <filesystem>
#include <iostream>
#include <fstream>

namespace fs = std::filesystem;

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

extern "C" __declspec(dllimport) int RunCheckbot(SafProviderBase * pApi, std::wstring checkBotPath);
extern "C" __declspec(dllimport) int ReleaseCheckbot();

// CAboutDlg dialog used for App About

class CAboutDlg : public CDialogEx
{
public:
	CAboutDlg();

// Dialog Data
#ifdef AFX_DESIGN_TIME
	enum { IDD = IDD_ABOUTBOX };
#endif

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

// Implementation
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialogEx(IDD_ABOUTBOX)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialogEx)
END_MESSAGE_MAP()


// CCppFeaDlg dialog



CCppFeaDlg::CCppFeaDlg(CWnd* pParent /*=nullptr*/)
	: CDialogEx(IDD_CPPFEA_DIALOG, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
	m_checkbotPath = _T("C:\\Program Files\\IDEA StatiCa\\StatiCa 24.1\\IdeaCheckbot.exe");

	DWORD bufferLength = MAX_PATH + 1;
	TCHAR buffer[MAX_PATH + 1];

	if (SUCCEEDED(SHGetFolderPath(NULL, CSIDL_PERSONAL, NULL, 0, buffer)))
	{
		m_feaProjectPath = CString(buffer);
		m_feaProjectName = _T("CppSafFeaExample");
	}
	else
	{
		m_feaProjectPath = _T("");
	}

	char path[MAX_PATH];
	GetModuleFileNameA(NULL, path, MAX_PATH);
	fs::path exePath = path;
	fs::path curDir = exePath.parent_path(); // Get the directory path


	fs::path safFilePath = curDir / fs::path(_T("SAF_steel_truss_first_import.xlsx"));
	m_safFilePath = safFilePath.c_str();
}

void CCppFeaDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);

	DDX_Text(pDX, IDC_CHECKBOT_PATH, m_checkbotPath);
	DDX_Text(pDX, IDC_FEA_PROJECT_PATH, m_feaProjectPath);
	DDX_Text(pDX, IDC_FEA_PROJECT_NAME, m_feaProjectName);
	DDX_Text(pDX, IDC_FEA_PROJECT_FILEPATH, m_safFilePath);
}

BEGIN_MESSAGE_MAP(CCppFeaDlg, CDialogEx)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON1, &CCppFeaDlg::OnRunCheckbotClick)
	ON_WM_CLOSE()
END_MESSAGE_MAP()


// CCppFeaDlg message handlers

BOOL CCppFeaDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// Add "About..." menu item to system menu.

	// IDM_ABOUTBOX must be in the system command range.
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != nullptr)
	{
		BOOL bNameValid;
		CString strAboutMenu;
		bNameValid = strAboutMenu.LoadString(IDS_ABOUTBOX);
		ASSERT(bNameValid);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

	// TODO: Add extra initialization here

	return TRUE;  // return TRUE  unless you set the focus to a control
}

void CCppFeaDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialogEx::OnSysCommand(nID, lParam);
	}
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CCppFeaDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// Center icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}
}

void CCppFeaDlg::OnClose()
{
	if (m_checkbotStatus == 1)
	{
		ReleaseCheckbot();
		delete pApi;
		pApi = nullptr;

		m_checkbotStatus = 0;
	}

	CDialogEx::OnClose();
}


// The system calls this function to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CCppFeaDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

/// <summary>
/// Start IDEA StatiCa Checkbot
/// </summary>
void CCppFeaDlg::OnRunCheckbotClick()
{
	if (m_checkbotStatus == 1)
	{
		// checkbot is already running
		return;
	}

	// create an instance of FEA API (it represents model from native FEA application)
	NativeFeaApi* pApi = new NativeFeaApi();

	// set the path do directory for Checkbot project
	std::wstring feaProject(m_feaProjectPath.GetString());
	std::wstring feaProjName(m_feaProjectName.GetString());
	pApi->SetProjectPath(feaProject, feaProjName);
	pApi->SetSafFilePath(m_safFilePath.GetString());

	// set the path to Checkbot executable
	std::wstring checkBotPath(m_checkbotPath.GetString());

	// run checkbot and pass API of the native FEA application
	m_checkbotStatus = RunCheckbot(pApi, checkBotPath);
}
