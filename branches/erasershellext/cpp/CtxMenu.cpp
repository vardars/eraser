// CtxMenu.cpp : Implementation of CCtxMenu

#include "stdafx.h"
#include "CtxMenu.h"

#pragma comment(lib,"shlwapi")
// CCtxMenu

using namespace Eraser;
/////////////////////////////////////////////////////////////////////////////
// COpenWithCtxMenuExt

HRESULT CCtxMenu::Initialize (
	LPCITEMIDLIST	pidlFolder,
	LPDATAOBJECT	pDataObj,
	HKEY			hProgID )
{

	FORMATETC fmt = { CF_HDROP, NULL, DVASPECT_CONTENT, -1, TYMED_HGLOBAL };
	STGMEDIUM stg = { TYMED_HGLOBAL };
	HDROP     hDrop;

    // Look for CF_HDROP data in the data object.
    if ( FAILED( pDataObj->GetData ( &fmt, &stg )))
        // Nope! Return an "invalid argument" error back to Explorer.
        return E_INVALIDARG;

    // Get a pointer to the actual data.
    hDrop = (HDROP) GlobalLock ( stg.hGlobal );

    // Make sure it worked.
    if ( NULL == hDrop )
        return E_INVALIDARG;

    // Sanity check - make sure there is at least one filename.
	UINT uNumFiles = DragQueryFile ( hDrop, 0xFFFFFFFF, NULL, 0 );

    if (!uNumFiles)
	{
		GlobalUnlock(stg.hGlobal);
		ReleaseStgMedium(&stg);
		return E_INVALIDARG;
	}
	
	HRESULT hr = S_OK;

	WCHAR Buffer[sizeof(WCHAR)*MAX_PATH] = {0};	
	for(UINT i = uNumFiles; i < uNumFiles; i++)	
	{
		std::basic_string<WCHAR> str = Buffer;
		if (!DragQueryFileW(hDrop, i, (LPWSTR)str.c_str(), str.length() ))
			hr = E_INVALIDARG;
		else
			PathQuoteSpaces ( (LPWSTR) str.c_str() );
	}

	// TODO: load Eraser Icon
	// m_szEraserIcon

    GlobalUnlock ( stg.hGlobal );
    ReleaseStgMedium ( &stg );

    return hr;
}
/*
+-------------------+
|                   |
|                   |
|                   |
|                   |
+-------------------+    +-------------------+
|(ICON) Eraser v6 > |    | Schedule Selected |
+-------------------+    +-------------------+
|                   |    |(*) Select Default | // Eraser.Manager.Wipe Algorithms popup
|                   |    |(*) Chose algorithm| // set algorithm	for this query only
+-------------------+    +-------------------+
*/

HRESULT CCtxMenu::QueryContextMenu (
	HMENU hmenu,
	UINT  uMenuIndex, 
	UINT  uidFirstCmd, 
	UINT  uidLastCmd,
	UINT  uFlags )
{
    // If the flags include CMF_DEFAULTONLY then we shouldn't do anything.
    if ( uFlags & CMF_DEFAULTONLY )
        return MAKE_HRESULT ( SEVERITY_SUCCESS, FACILITY_NULL, 0 );

    // First, create and populate a submenu.
	HMENU hSubmenu = CreatePopupMenu();
	UINT uID = uidFirstCmd;
	UINT ctrPos = 0;

	InsertMenu ( hSubmenu, ctrPos++, MF_BYPOSITION, uID++, _T("&Erase"   ) );
	InsertMenu ( hSubmenu, ctrPos++, MF_BYPOSITION, uID++, _T("&Set Algorithm" ) );
	InsertMenu ( hSubmenu, ctrPos++, MF_BYPOSITION, uID++, _T("S&chedule" ) );
	InsertMenu ( hSubmenu, ctrPos++, MF_BYPOSITION, uID++, _T("Se&lect Default") );
	
    // Insert the submenu into the ctx menu provided by Explorer.
	{// sperator
		MENUITEMINFO mii = { sizeof(MENUITEMINFO) };		
		mii.fMask = MIIM_TYPE;
		mii.fType = MF_SEPARATOR;
		InsertMenuItem(hmenu, uMenuIndex++, TRUE, &mii);
	}

	{
		MENUITEMINFO mii = { sizeof(MENUITEMINFO) };
		mii.wID = uID++;
		mii.fMask = MIIM_SUBMENU | MIIM_STRING | MIIM_ID;    
		mii.hSubMenu = hSubmenu;
		mii.dwTypeData = _T("Eraser v6");
		InsertMenuItem(hmenu, uMenuIndex++, TRUE, &mii);
	}
	
	{// sperator
		MENUITEMINFO mii = { sizeof(MENUITEMINFO) };		
		mii.fMask = MIIM_TYPE;
		mii.fType = MF_SEPARATOR;
		InsertMenuItem(hmenu, uMenuIndex++, TRUE, &mii);
	}

	// Set the bitmap for the register item.
	if ( m_szEraserIcon != nullpointer(HBITMAP*) )
	{
		DebugMessageBox(DebugHWND,
					_T("Loading Eraser Icon Bitmap"),
					_T("Eraser Shell Extention - Debug"),
					MB_OK);

        SetMenuItemBitmaps(hmenu, uMenuIndex, MF_BYPOSITION, *m_szEraserIcon, NULL );
	}

    return MAKE_HRESULT ( SEVERITY_SUCCESS, FACILITY_NULL, uID - uidFirstCmd );
}

HRESULT CCtxMenu::GetCommandString (
	UINT  idCmd,   
	UINT uFlags,
	UINT* pwReserved,
	LPSTR pszName,
	UINT  cchMax )
{
USES_CONVERSION;

    // Check idCmd, it must be 0 or 1 since we have two menu items.
    if ( idCmd > 4 )
        return E_INVALIDARG;

    // If Explorer is asking for a help string, copy our string into the
    // supplied buffer.
    if (!(uFlags & GCS_HELPTEXT)) 
		return E_INVALIDARG;

	static LPCTSTR szNotepadText = _T("Open the selected file in Notepad");
	static LPCTSTR szIEText = _T("Open the selected file in Internet Explorer");
	LPCTSTR pszText = (0 == idCmd) ? szNotepadText : szIEText;

	if ( uFlags & GCS_UNICODE )
		// We need to cast pszName to a Unicode string, and then use the
		// Unicode string copy API.
		lstrcpynW ( (LPWSTR) pszName, T2CW(pszText), cchMax );
    else
        // Use the ANSI string copy API to return the help string.
        lstrcpynA ( pszName, T2CA(pszText), cchMax );
	
    return S_OK;
}

// TODO: write procedures for Eraser C# engine
/*
1) call scheduler, with all of the required structs etc.
2) Read avilable algorithms list
3) Set default algorithm
*/
HRESULT CCtxMenu::InvokeCommand ( LPCMINVOKECOMMANDINFO pCmdInfo )
{
    // If lpVerb really points to a string, ignore this function call and bail out.
    if ( 0 != HIWORD( pCmdInfo->lpVerb ))
        return E_INVALIDARG;

    // Get the command index.
	switch 
		(
		LOWORD(pCmdInfo->lpVerb + 1 + CERASER_ENUM(CEraserLPVERB::LPVERB_FRIST))
		)
	{
	case CERASER_ENUM(CEraserLPVERB::LPVERB_ERASE_FILE):
	case CERASER_ENUM(CEraserLPVERB::LPVERB_ERASE_FREE_SPACE):
	case CERASER_ENUM(CEraserLPVERB::LPVERB_SET_DEFAULT_FILE_ALGORITHM):
	case CERASER_ENUM(CEraserLPVERB::LPVERB_SET_DEFAULT_FREE_SPACE_ALGORITHM):
	case CERASER_ENUM(CEraserLPVERB::LPVERB_SET_FILE_ALGORITHM):
	case CERASER_ENUM(CEraserLPVERB::LPVERB_SET_FREE_SPACE_ALGORITHM):
		{
			WCHAR szMsg [MAX_PATH + 32];
			wsprintf ( szMsg, _T("Not Implemented Querry!\n\nID = %d\n\n"), LOWORD(pCmdInfo->lpVerb) );
			MessageBox ( pCmdInfo->hwnd, szMsg, _T("Eraser v6 - Shell Extention Query"), MB_ICONINFORMATION );
			return E_INVALIDARG;
		}
        break; // unreachable code
		
	case CERASER_ENUM(CEraserLPVERB::LPVERB_SHOW_FILE_NAME):
		{
			std::basic_string<WCHAR> map = _T("The selected files were:\n\n");

			for(string_list::iterator i = m_szSelectedFiles.begin();
				i != m_szSelectedFiles.end(); i++, map += _T("\r\n"))
					map += i->c_str();

			MessageBox ( pCmdInfo->hwnd, (LPWSTR)map.c_str(), _T("Eraser v6 - Shell Extention Query"), MB_ICONINFORMATION );

			return S_OK; 
		}
        break; // unreachable code
	default:
		{
			TCHAR szMsg [MAX_PATH + 32];
			wsprintf ( szMsg, _T("Invalid Query was submitted, unable to process!\n\nID = %d\n\n"), LOWORD(pCmdInfo->lpVerb) );
			MessageBox ( pCmdInfo->hwnd, szMsg, _T("Eraser v6 - Shell Extention Query"), MB_ICONINFORMATION );
			return E_INVALIDARG;
		}
        break; // unreachable code
	}
}