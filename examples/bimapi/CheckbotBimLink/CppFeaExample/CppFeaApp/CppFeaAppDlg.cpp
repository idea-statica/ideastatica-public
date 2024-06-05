
// CppFeaAppDlg.cpp : implementation file
//

#include "pch.h"
#include "framework.h"
#include "CppFeaApp.h"
#include "CppFeaAppDlg.h"
#include "afxdialogex.h"
#include "..\CppFeaApi\NativeFeaApi.h"
#include <vector>
#include <string>

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

extern "C" __declspec(dllimport) int RunCheckbot(NativeFeaApi * pApi, std::wstring checkBotPath);
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
	m_checkbotPath = _T("C:\\Program Files\\IDEA StatiCa\\StatiCa 24.0\\IdeaCheckbot.exe");

	DWORD bufferLength = MAX_PATH + 1;
	TCHAR buffer[MAX_PATH + 1];

	if (GetCurrentDirectory(bufferLength, buffer))
	{
		m_feaProjectPath = CString(buffer);
		m_feaProjectPath += _T("\\CppFeaProject");
	}
	else
	{
		m_feaProjectPath = _T("");
	}

}

void CCppFeaDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);

	DDX_Text(pDX, IDC_CHECKBOT_PATH, m_checkbotPath);
	DDX_Text(pDX, IDC_FEA_PROJECT_PATH, m_feaProjectPath);
}

BEGIN_MESSAGE_MAP(CCppFeaDlg, CDialogEx)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON1, &CCppFeaDlg::OnBnClickedButton1)
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

void CCppFeaDlg::OnBnClickedButton1()
{
	if (m_checkbotStatus == 1)
	{
		// checkbot is already running
		return;
	}

	// create an instance of FEA API (it represents model from native FEA application)
	NativeFeaApi* pApi = new NativeFeaApi();

	std::wstring feaProject(m_feaProjectPath.GetString());
	pApi->SetProjectPath(feaProject);

	std::wstring checkBotPath(m_checkbotPath.GetString());

	//NativeFeaGeometry* geom = pApi->GetGeometry();
	//std::vector<int> memberIds = geom->GetMembersIdentifiers();
	//std::vector<int> nodesIds = geom->GetNodesIdentifiers();

	//NativeFeaNode* node = geom->GetNode(3);

	// run checkbot and pass API of the native FEA application
	m_checkbotStatus = RunCheckbot(pApi, checkBotPath);
}
