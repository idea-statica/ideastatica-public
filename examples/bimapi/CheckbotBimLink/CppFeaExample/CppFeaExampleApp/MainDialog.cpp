#include "MainDialog.h"

#include "BimApiLinkCppFeaExample/CheckbotLink.h"
#include "resource.h"

#include <filesystem>
#include <string>

namespace
{
	/// <summary>
	/// The folder CppFeaExampleCheckbotProject next to the application.
	/// </summary>
	CString GetDefaultProjectPath()
	{
		std::wstring applicationPath(MAX_PATH, L'\0');
		DWORD length = 0;
		// A path longer than the buffer comes back cut off, with the buffer's full size as its length.
		while ((length = GetModuleFileNameW(nullptr, applicationPath.data(), static_cast<DWORD>(applicationPath.size()))) == applicationPath.size())
		{
			applicationPath.resize(applicationPath.size() * 2);
		}
		applicationPath.resize(length);

		std::filesystem::path projectPath = std::filesystem::path(applicationPath).parent_path() / L"CppFeaExampleCheckbotProject";
		return projectPath.c_str();
	}
}

BEGIN_MESSAGE_MAP(CMainDialog, CDialog)
	ON_BN_CLICKED(IDC_START_CHECKBOT, &CMainDialog::OnStartCheckbot)
END_MESSAGE_MAP()

CMainDialog::CMainDialog(const CppFeaExampleApi::IFeaApi& fea)
	: CDialog(IDD_MAIN)
	, m_fea(fea)
	, m_icon(AfxGetApp()->LoadIcon(IDR_MAINFRAME))
	, m_checkbotPath(L"C:\\Program Files\\IDEA StatiCa\\StatiCa 26.1\\IdeaCheckbot.exe")
	, m_projectPath(GetDefaultProjectPath())
{
}

BOOL CMainDialog::OnInitDialog()
{
	CDialog::OnInitDialog();

	// A dialog shows the application icon in its title bar and in the taskbar only when it is set here.
	SetIcon(m_icon, TRUE);
	SetIcon(m_icon, FALSE);

	return TRUE;
}

void CMainDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);

	DDX_Text(pDX, IDC_CHECKBOT_PATH, m_checkbotPath);
	DDX_Text(pDX, IDC_PROJECT_PATH, m_projectPath);
}

void CMainDialog::OnStartCheckbot()
{
	// TRUE copies what the user typed in the text boxes into m_checkbotPath and m_projectPath.
	UpdateData(TRUE);

	std::error_code error;
	if (!std::filesystem::is_regular_file(m_checkbotPath.GetString(), error))
	{
		AfxMessageBox(L"IdeaCheckbot.exe was not found. Enter the full path to IdeaCheckbot.exe of your IDEA StatiCa.", MB_ICONWARNING);
		return;
	}

	std::wstring failure = BimApiLinkCppFeaExample::StartCheckbot(m_checkbotPath.GetString(), m_projectPath.GetString(), m_fea);
	if (!failure.empty())
	{
		AfxMessageBox(failure.c_str(), MB_ICONERROR);
	}
}
