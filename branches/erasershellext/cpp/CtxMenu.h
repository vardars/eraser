// CtxMenu.h : Declaration of the CCtxMenu

#pragma once
#include "resource.h"       // main symbols

#include "EraserCtxMenu_i.h"


#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif

#include <string>
#include <list>


#define CERASER_ENUM_TYPE		unsigned short
#define CERASER_CAST(type,x)	((type)(x))
#define CERASER_ENUM(x)			(CERASER_CAST(CERASER_ENUM_TYPE,x))

#define private()	private:
#define public()	public:
#define protected()	protected:
#define nullpointer(type) ((type)0)

#ifdef __cplusplus0x
#define CERASER_ENUM_CPP0X			: CERASER_ENUM_TYPE
#elif defined(__cplusplus)
#define CERASER_ENUM_CPP0X	
#else
#error This is a template C++ file!
#endif

#ifdef _DEBUG
#define DebugMessageBox(...) MessageBox(__VA_ARGS__)
static HWND DebugHWND =
CreateWindow(0, _T("Eraser Debug Windows", "Eraser Debug Windows"), 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL);
#else
#define DebugMessageBox(...) ;;;;;;;;;;;;;;;;;;;;;;;
#endif

namespace Eraser 
{
	typedef std::list<std::basic_string<WCHAR> > string_list;

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

		public() enum CEraserLPVERB CERASER_ENUM_CPP0X
		{
			LPVERB_FRIST					= 0x8000, // 16-bit enum hack 
			LPVERB_SHOW_FILE_NAME					,
			LPVERB_ERASE_FREE_SPACE					,
			LPVERB_ERASE_FILE						,
			LPVERB_SET_FREE_SPACE_ALGORITHM			,
			LPVERB_SET_FILE_ALGORITHM				,
			LPVERB_SET_DEFAULT_FREE_SPACE_ALGORITHM	,
			LPVERB_SET_DEFAULT_FILE_ALGORITHM		,	
			LPVERB_LAST,
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
		HBITMAP*	m_szEraserIcon;
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