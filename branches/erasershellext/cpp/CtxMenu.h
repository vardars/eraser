// CtxMenu.h : Declaration of the CCtxMenu

#pragma once
#pragma unmanaged

#include "resource.h"       // main symbols

#include "EraserCtxMenu_i.h"

#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif

#include <string>
#include <list>
#include <fstream>

#define CERASER_ENUM_TYPE		unsigned int
#define CERASER_ENUM(x)			((CERASER_ENUM_TYPE)x)

#ifdef _DEBUG
#define DebugMessageBox(...) MessageBox(__VA_ARGS__)
static HWND DebugHWND =
CreateWindow(0, _T("Eraser Debug Windows", "Eraser Debug Windows"), 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL);
#else
#define DebugMessageBox(...) ;;;;;;;;;;;;;;;;;;;;;;;
#endif

namespace Eraser 
{
	typedef std::basic_string<WCHAR> string_type;
	typedef std::list<string_type  > string_list;

	enum CEraserSecureMove
	{
		INV_SRC_FILE,
		INV_DST_FILE,
	};

	static int SecureMove(const string_type& dst, const string_type& src)
	{
	}

	// CCtxMenu
	class ATL_NO_VTABLE CCtxMenu :
		public CComObjectRootEx<CComSingleThreadModel>,
		public CComCoClass<CCtxMenu, &CLSID_CtxMenu>,
		public ICtxMenu,
		public IShellExtInit,
		public IContextMenu
	{
	public:
		CCtxMenu()
		{
		}
				
	public: enum CEraserLPVERBS
	{
		CERASER_ERASE						= 0,
		CERASER_SCHEDULE				= 1,
		CERASER_ERASE_ON_RESTART= 2,			
		CERASER_SEPERATOR_1			= 3,
		CERASER_SECURE_MOVE			= 4,			
		CERASER_SEPERATOR_2			= 5,
		CERASER_CUSTOMISE				= 6,
	};

	public:
		// IShellExtInit
		STDMETHOD(Initialize)(LPCITEMIDLIST, LPDATAOBJECT, HKEY);

		// IContextMenu
		STDMETHOD(GetCommandString)(UINT, UINT, UINT*, LPSTR, UINT);
		STDMETHOD(InvokeCommand)(LPCMINVOKECOMMANDINFO);
		STDMETHOD(QueryContextMenu)(HMENU, UINT, UINT, UINT, UINT);

	protected:
		string_list	m_szSelectedFiles;
		HBITMAP     m_szEraserIcon;
	public:

	DECLARE_REGISTRY_RESOURCEID(IDR_CTXMENU)

	DECLARE_NOT_AGGREGATABLE(CCtxMenu)

	BEGIN_COM_MAP(CCtxMenu)
		COM_INTERFACE_ENTRY(ICtxMenu)
	END_COM_MAP()

		DECLARE_PROTECT_FINAL_CONSTRUCT()

		HRESULT FinalConstruct()
		{
			return S_OK;
		}

		void FinalRelease()
		{
		}
	};

	OBJECT_ENTRY_AUTO(__uuidof(CtxMenu), CCtxMenu)

} // namespace Eraser

#pragma managed

using namespace System;
using namespace System::Collections::Generic;
using namespace System::ComponentModel;
using namespace System::Data;
using namespace System::Drawing;
using namespace System::Text;

using namespace Eraser;
using namespace Eraser::Manager;

namespace Eraser
{	
	static String ^UnmanagedToManagedString(const string_type& string)
	{
		return gcnew String(string.c_str());
	}

	static void EraseFiles(List<String ^> ^paths)
	{
		Task ^task = gcnew Task();
	
		for(int i = 0; i < paths->Count; i++)
		{
			Task::File ^fileTask = gcnew Task::File();
			fileTask->Path = paths[i];
			fileTask->Method = reinterpret_cast<ErasureMethod ^>
				( ErasureMethodManager::Default );

			task->Targets->Add(fileTask);
		}

		// DirectExecutor::AddTask(task);
	}

	static void EraseFolder(List<String ^> ^paths)
	{
		Task ^task = gcnew Task();

		for(int i = 0; i < paths->Count; i++)
		{
			Task::Folder ^folderTask = gcnew Task::Folder();
			folderTask->Path = paths[i];
			folderTask->IncludeMask = L"";
			folderTask->ExcludeMask = L"";
			folderTask->DeleteIfEmpty = 
				Windows::Forms::MessageBox::Show(L"Would you like to delete empty folders?", L"Eraser Empty Folders Tips?",
				Windows::Forms::MessageBoxButtons::YesNo, 
				Windows::Forms::MessageBoxIcon::Question) ==
				Windows::Forms::DialogResult::Yes;
			task->Targets->Add(folderTask);
		}

		// DirectExecutor::AddTask(task);
	}

	static void EraseUnusedSpace(List<String ^> ^paths)
	{
		Task ^task = gcnew Task();

		for(int i = 0; i < paths->Count; i++)
		{
			Task::UnusedSpace ^unusedSpaceTask = gcnew Task::UnusedSpace();
			unusedSpaceTask->Drive = paths[i];
			unusedSpaceTask->EraseClusterTips = 
				Windows::Forms::MessageBox::Show(L"Would you like to erase the cluster tips as well?", L"Eraser Cluster Tips?",
				Windows::Forms::MessageBoxButtons::YesNo, 
				Windows::Forms::MessageBoxIcon::Question) ==
				Windows::Forms::DialogResult::Yes;			
			task->Targets->Add(unusedSpaceTask);
		}

		// DirectExecutor::AddTask(task);
	}

} // namespace Eraser