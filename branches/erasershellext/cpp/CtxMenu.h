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
#define foreach(iter, container) \
	for(string_list::iterator iter = container.begin(); \
	iter != container.end(); iter++)

#define GCNEW(object_type,...)  \
	*(CCtxMenu::GCNew(new object_type(__VA_ARGS__)))

#define S(str) L ## str

#ifdef _DEBUG
#define DebugMessageBox(...) MessageBox(__VA_ARGS__)
static HWND DebugHWND =
CreateWindow(0, _T("Eraser Debug Windows"), 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL);
#else
#define DebugMessageBox(...) ;;;;;;;;;;;;;;;;;;;;;;;
#endif

namespace Eraser 
{
	typedef std::wstring							string_type;
	typedef std::list<string_type  >	string_list;
	typedef std::list<void*>					gc;


	enum CEraserSecureMove
	{
		INV_SRC_FILE,
		INV_DST_FILE, 
	};

	static int SecureMove(const std::wstring& dst, const std::wstring& src)
	{
		//CFile file(
		//	CreateFile(
		//	src.c_str(), 
		//	FILE_GENERIC_READ|FILE_GENERIC_WRITE,
		//	FILE_SHARE_READ|FILE_SHARE_WRITE,
		//	NULL,
		//	OPEN_EXISTING,
		//	0,
		//	NULL)
		//	);

		if(!CopyFile(src.c_str(), dst.c_str(), FALSE))
		{
			//file.Close();
			return GetLastError();
		}

		// successfull copy, add for erasure

	}

	// CCtxMenu
	class ATL_NO_VTABLE CCtxMenu :
		public CComObjectRootEx<CComSingleThreadModel>,
		public CComCoClass<CCtxMenu, &CLSID_CtxMenu>,
		public IShellExtInit,
		public IContextMenu
	{
	public:
		CCtxMenu()
		{
		}

		enum CEraserLPVERBS
		{
			CERASER_ERASE = 0,
			CERASER_SCHEDULE,
			CERASER_ERASE_ON_RESTART,
			CERASER_SEPERATOR_1,
			CERASER_SECURE_MOVE,
			CERASER_SEPERATOR_2,
			CERASER_CUSTOMISE
		};

	public:
		// IShellExtInit
		STDMETHOD(Initialize)(LPCITEMIDLIST, LPDATAOBJECT, HKEY);

		// IContextMenu
		STDMETHOD(GetCommandString)(UINT, UINT, UINT*, LPSTR, UINT);
		STDMETHOD(InvokeCommand)(LPCMINVOKECOMMANDINFO);
		STDMETHOD(QueryContextMenu)(HMENU, UINT, UINT, UINT, UINT);

	protected:
		gc          m_GC;
		string_list	m_szSelectedFiles;
		string_list	m_szSelectedDirectories;
		string_list m_szSelectedUnused;
		HBITMAP			m_szEraserIcon;

		
		template<typename T>
		T* GCNew(T* pointer)
		{
			this->m_GC.push_back(reinterpret_cast<void*>(pointer));
			return pointer;
		}


	public:
		DECLARE_REGISTRY_RESOURCEID(IDR_ERASERCTXMENU)
		DECLARE_NOT_AGGREGATABLE(CCtxMenu)
		BEGIN_COM_MAP(CCtxMenu)
			COM_INTERFACE_ENTRY(IShellExtInit)
			COM_INTERFACE_ENTRY(IContextMenu)
		END_COM_MAP()
		DECLARE_PROTECT_FINAL_CONSTRUCT()
		HRESULT FinalConstruct()
		{
			return S_OK;
		}

		void FinalRelease()
		{
			for(gc::iterator i = m_GC.begin(); i != m_GC.end(); i++)
				delete *i;
		}
	};

	OBJECT_ENTRY_AUTO(__uuidof(CtxMenu), CCtxMenu)

} // namespace Eraser

