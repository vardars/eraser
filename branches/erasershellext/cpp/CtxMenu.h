// CtxMenu.h : Declaration of the CCtxMenu
#pragma once

#include "resource.h"
#include "EraserCtxMenu_i.h"

#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif

#include <string>
#include <list>
#include <fstream>

namespace Eraser 
{
	typedef std::wstring				string_type;
	typedef std::list<string_type>		string_list;

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

		if (!CopyFile(src.c_str(), dst.c_str(), FALSE))
		{
			//file.Close();
			return GetLastError();
		}

		// successfull copy, add for erasure
	}

	class ATL_NO_VTABLE CCtxMenu :
		public CComObjectRootEx<CComSingleThreadModel>,
		public CComCoClass<CCtxMenu, &CLSID_CtxMenu>,
		public IShellExtInit,
		public IContextMenu3
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

		// IContextMenu3
		STDMETHOD(GetCommandString)(UINT, UINT, UINT*, LPSTR, UINT);
		STDMETHOD(InvokeCommand)(LPCMINVOKECOMMANDINFO);
		STDMETHOD(QueryContextMenu)(HMENU, UINT, UINT, UINT, UINT);
		STDMETHOD(HandleMenuMsg)(UINT, WPARAM, LPARAM);
		STDMETHOD(HandleMenuMsg2)(UINT, WPARAM, LPARAM, LRESULT*);

	protected:
		bool OnMeasureItem(UINT& itemWidth, UINT& itemHeight);
		bool OnDrawItem(HDC hdc, RECT rect, UINT action, UINT state);

		static MENUITEMINFO* GetSeparator();
		static HICON GetMenuIcon();
		static HBITMAP GetMenuBitmap();
		static HBITMAP CreateDIB(LONG width, LONG height, char** bitmapBits);

	protected:
		UINT		m_itemID;
		string_list	m_szSelectedFiles;
		string_list	m_szSelectedDirectories;
		string_list	m_szSelectedUnused;

		static const wchar_t* m_szMenuTitle;

	public:
		DECLARE_REGISTRY_RESOURCEID(IDR_ERASERCTXMENU)
		DECLARE_NOT_AGGREGATABLE(CCtxMenu)
		BEGIN_COM_MAP(CCtxMenu)
			COM_INTERFACE_ENTRY(IShellExtInit)
			COM_INTERFACE_ENTRY(IContextMenu)
			COM_INTERFACE_ENTRY(IContextMenu2)
			COM_INTERFACE_ENTRY(IContextMenu3)
		END_COM_MAP()

		DECLARE_PROTECT_FINAL_CONSTRUCT()
		HRESULT FinalConstruct()
		{
			return S_OK;
		}
	};

	OBJECT_ENTRY_AUTO(__uuidof(CtxMenu), CCtxMenu)

} // namespace Eraser

