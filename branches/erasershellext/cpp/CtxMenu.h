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

namespace Eraser 
{
	typedef std::list<std::wstring> string_list;

	enum CEraserSecureMove
	{
		INV_SRC_FILE,
		INV_DST_FILE,
	};

	static int SecureMove(const std::wstring& dst, const std::wstring& src)
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
