/* 
 * $Id$
 * Copyright 2008 The Eraser Project
 * Original Author: Kasra Nassiri <cjax@users.sourceforge.net>
 * Modified By: Joel Low <lowjoel@users.sourceforge.net>
 * 
 * This file is part of Eraser.
 * 
 * Eraser is free software: you can redistribute it and/or modify it under the
 * terms of the GNU General Public License as published by the Free Software
 * Foundation, either version 3 of the License, or (at your option) any later
 * version.
 * 
 * Eraser is distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
 * A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * A copy of the GNU General Public License can be found at
 * <http://www.gnu.org/licenses/>.
 */

#pragma once

#include "resource.h"
#include "EraserCtxMenu_i.h"

#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif

#include <list>
#include <vector>
#include <string>

namespace Eraser 
{
	typedef std::wstring				string_type;
	typedef std::list<string_type>		string_list;

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
			CERASER_ERASE				= 1 << 0,
			CERASER_ERASE_ON_RESTART	= 1 << 1,
			CERASER_ERASE_UNUSED_SPACE	= 1 << 2,
			CERASER_SEPERATOR_1,
			CERASER_SECURE_MOVE			= 1 << 3,
		};

	public:
		//IShellExtInit
		STDMETHOD(Initialize)(LPCITEMIDLIST, LPDATAOBJECT, HKEY);

		//IContextMenu3
		STDMETHOD(GetCommandString)(UINT, UINT, UINT*, LPSTR, UINT);
		STDMETHOD(InvokeCommand)(LPCMINVOKECOMMANDINFO);
		STDMETHOD(QueryContextMenu)(HMENU, UINT, UINT, UINT, UINT);
		STDMETHOD(HandleMenuMsg)(UINT, WPARAM, LPARAM);
		STDMETHOD(HandleMenuMsg2)(UINT, WPARAM, LPARAM, LRESULT*);

	protected:
		bool OnMeasureItem(UINT& itemWidth, UINT& itemHeight);
		bool OnDrawItem(HDC hdc, RECT rect, UINT action, UINT state);

		CEraserLPVERBS GetApplicableActions();

		static MENUITEMINFO* GetSeparator();
		static HICON GetMenuIcon();
		static HBITMAP GetMenuBitmap();
		static HBITMAP CreateDIB(LONG width, LONG height, char** bitmapBits);

	protected:
		UINT		m_itemID;
		string_list	m_szSelectedFiles;
		string_type	m_szDestinationDirectory;
		std::vector<CEraserLPVERBS> VerbMenuIndices;

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

}
