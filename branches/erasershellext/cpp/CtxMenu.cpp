/////////////////////////////////////////////////////////////////////////////
// CtxMenu.cpp : Implementation of CCtxMenu
// CCtxMenu
/////////////////////////////////////////////////////////////////////////////
#include "stdafx.h"
#include "CtxMenu.h"
#pragma comment(lib,"shlwapi")

namespace Eraser {
HRESULT CCtxMenu::Initialize(
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

	// TODO: load Eraser Icon and store HBITMAP in m_szEraserIcon

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
|(ICON) Eraser v6 > |    | Erase selected    | //--> erase the files immediately using defaults
+-------------------+    | Schedule Selected | //--> open the scheduler menu, with files/folders filled in
|                   |    +-------------------+
|                   |    | Secure move       | //--> secure move the files
|                   |    +-------------------+ // Eraser.Manager Algorithms popup
|                   |    |(*) Customise      | // set algorithm	for this query only
+-------------------+    +-------------------+
*/

/*
we have eraser shell scripts (.eshlls) where we use them
to generate shell extention menus dynamically.
the syntax is simple.
This project should be aimed at the 6.1 release.

BEGIN @MENU : POSITION($INDEX)
	[ICON="PATH"]
	[TEXT="DISPLAY_TEXT"]

	[FLAGS=MIIM_*]			
		//MIIM_BITMAP
		//MIIM_CHECKMARKS
		//MIIM_DATA
		//MIIM_FTYPE
		//MIIM_ID
		//MIIM_STATE
		//MIIM_STRING
		//MIIM_SUBMENU
		//MIIM_TYPE

	[TYPE=MF_*]
		//MF_APPEND,
		//MF_BITMAP,
		//MF_BYCOMMAND,
		//MF_BYPOSITION
		//MF_CHECKED
		//MF_DEFAULT
		//MF_DELETE
		//MF_DISABLED
		//MF_ENABLED
		//MF_END
		//MF_GRAYED
		//MF_HELP
		//MF_HILITE
		//MF_INSERT
		//MF_MENUBARBREAK
		//MF_MENUBREAK
		//MF_MOUSESELECT
		//MF_OWNERDRAW
		//MF_POPUP
		//MF_POPUP
		//MF_REMOVE,
		//MF_RIGHTJUSTIFY,
		//MF_SEPARATOR,
		//MF_STRING,
		//MF_SYSMENU,
		//MF_UNCHECKED,
		//MF_UNHILITE
		//MF_USECHECKBITMAPS
END

// Desirable to have bitmaps cached
@MENU 
{
	BITMAP = "FILE";
	TEXT="Eraser";
	BITMAP CHECKED="FILE";
	BITMAP UNCHECKED="FILE";

	// submenu creation
	@MENU 
	{
		@CHECKBOX 
		{
		}
		@SEPERATOR
	}
}

UINT     cbSize;
UINT     fMask;
UINT     fType;         // used if MIIM_TYPE (4.0) or MIIM_FTYPE (>4.0)
UINT     fState;        // used if MIIM_STATE
UINT     wID;           // used if MIIM_ID
HMENU    hSubMenu;      // used if MIIM_SUBMENU
HBITMAP  hbmpChecked;   // used if MIIM_CHECKMARKS
HBITMAP  hbmpUnchecked; // used if MIIM_CHECKMARKS
ULONG_PTR dwItemData;   // used if MIIM_DATA
__field_ecount_opt(cch) LPSTR    dwTypeData;    // used if MIIM_TYPE (4.0) or MIIM_STRING (>4.0)
UINT     cch;           // used if MIIM_TYPE (4.0) or MIIM_STRING (>4.0)
HBITMAP  hbmpItem;      // used if MIIM_BITMAP

*/

/**
 * return a seperate MENUITEMINFO structure */
static MENUITEMINFO* GetSeperator()
{
	MENUITEMINFO *mii = new MENUITEMINFO();		
	mii->cbSize = sizeof(MENUITEMINFO);
	mii->fMask = MIIM_TYPE;
	mii->fType = MF_SEPARATOR;
	return mii;
}

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
	UINT ctrPos = 0;
	UINT uID = uidFirstCmd;	
	HMENU hSubmenu = CreatePopupMenu();	


	/* Remember this order is defined _Statically_ on the CEraserLPVERB enum */
	/* -------------------------------- Submenu -------------------------------- */
	/* [0] */ InsertMenu     ( hSubmenu, ctrPos++, MF_BYPOSITION, uID++, _T("&Erase"    ) );
	/* [1] */ InsertMenu     ( hSubmenu, ctrPos++, MF_BYPOSITION, uID++, _T("&Schedule" ) );
	/* [2] */ InsertMenu     ( hSubmenu, ctrPos++, MF_BYPOSITION, uID++, _T("Erase on &Restart" ) );
	/* ------------------------------------------------------------------------- */
	/* [3] */ InsertMenuItem ( hSubmenu, ctrPos++, TRUE, GetSeperator() );
	/* [4] */ InsertMenu     ( hSubmenu, ctrPos++, MF_BYPOSITION, uID++, _T("Secure &Move" ) );	
	/* ------------------------------------------------------------------------- */
	/* [5] */ InsertMenuItem ( hSubmenu, ctrPos++, TRUE, GetSeperator() );
	/* [6] */ InsertMenu     ( hSubmenu, ctrPos++, MF_BYPOSITION, uID++, _T("&Customise") );	
	/* ------------------------------ Submenu end ------------------------------ */

	// Insert the submenu into the ctx menu provided by Explorer.
	/* ------------------------------------------------------------------------- */
	InsertMenuItem ( hSubmenu, uMenuIndex++, TRUE, GetSeperator() );
	{
		MENUITEMINFO mii = { sizeof(MENUITEMINFO) };
		mii.wID = uID++;
		mii.fMask = MIIM_SUBMENU | MIIM_STRING | MIIM_ID;    
		mii.hSubMenu = hSubmenu;
		mii.dwTypeData = _T("Eraser v6");
		InsertMenuItem(hmenu, uMenuIndex++, TRUE, &mii);
	}
	/* ------------------------------------------------------------------------- */
	InsertMenuItem ( hSubmenu, uMenuIndex++, TRUE, GetSeperator() );

	// Set the bitmap for the register item.
	if ( m_szEraserIcon != ((HBITMAP)0) )
		SetMenuItemBitmaps(hmenu, uMenuIndex++, MF_BYPOSITION, m_szEraserIcon, NULL );

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

#define foreach(iter, container) \
	for(string_list::iterator iter = container.begin(); \
	iter != container.end(); iter++)

HRESULT CCtxMenu::InvokeCommand ( LPCMINVOKECOMMANDINFO pCmdInfo )
{
	// If lpVerb really points to a string, ignore this function call and bail out.
	if ( HIWORD( pCmdInfo->lpVerb )  != 0)
		return E_INVALIDARG;

	// Get the command index.
	switch ((CERASER_ENUM_TYPE) LOWORD(pCmdInfo->lpVerb + 1))
	{
		case CERASER_ERASE:
			EraseFiles(m_szSelectedFiles);
			break;
		case CERASER_SECURE_MOVE:
			EraseFiles(m_szSelectedFiles);
			break;
		// NOT IMPLEMENTED METHODS
#if 0
		case CERASER_ERASE_ON_RESTART:
		{
			List<String ^> ^files = gcnew List<String ^>(m_szSelectedFiles.size());

			foreach(file, m_szSelectedFiles)
			{
				files->Add( UnmanagedToManagedString(*file) );
			}
			EraseFiles(files);
		}
		case CERASER_SCHEDULE:
		{
			foreach(file, m_szSelectedFiles)
			{
			}
		}
		case CERASER_CUSTOMISE:
		{
		}
#endif
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
}