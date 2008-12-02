o/////////////////////////////////////////////////////////////////////////////
// CtxMenu.cpp : Implementation of CCtxMenu
// CCtxMenu
/////////////////////////////////////////////////////////////////////////////
#include "stdafx.h"
#include "CtxMenu.h"
#include "dllmain.h"
#pragma comment(lib,"shlwapi")

template<typename handleType> class Handle
{
public:
	Handle(handleType handle)
	{
		Object = handle;
	}

	~Handle()
	{
		DeleteObject(Object);
	}

	operator handleType&()
	{
		return Object;
	}

private:
	handleType Object;
};

namespace Eraser {
	const wchar_t* CCtxMenu::m_szMenuTitle = L"Eraser v6";

HRESULT CCtxMenu::Initialize(LPCITEMIDLIST /*pidlFolder*/, LPDATAOBJECT pDataObj,
                             HKEY /*hProgID*/)
{
	m_itemID      = 0;
	FORMATETC fmt = { CF_HDROP, NULL, DVASPECT_CONTENT, -1, TYMED_HGLOBAL };
	STGMEDIUM stg = { TYMED_HGLOBAL };
	HDROP     hDrop;

	//Look for CF_HDROP data in the data object.
	if (FAILED(pDataObj->GetData (&fmt, &stg)))
		//Nope! Return an "invalid argument" error back to Explorer.
		return E_INVALIDARG;

	//Get a pointer to the actual data.
	hDrop = static_cast<HDROP>(GlobalLock(stg.hGlobal));

	//Make sure it worked.
	if (hDrop == NULL)
		return E_INVALIDARG;

	//Sanity check - make sure there is at least one filename.
	UINT uNumFiles = DragQueryFile(hDrop, 0xFFFFFFFF, NULL, 0);
	if (!uNumFiles)
	{
		GlobalUnlock(stg.hGlobal);
		ReleaseStgMedium(&stg);
		return E_INVALIDARG;
	}

	HRESULT hr = S_OK;
	WCHAR Buffer[MAX_PATH] = {0};	
	for (UINT i = uNumFiles; i < uNumFiles; i++)	
	{
		//TODO: Collect the list of files queried.
		UINT charsWritten = DragQueryFile(hDrop, i, Buffer, sizeof(Buffer) / sizeof(Buffer[0]));
		if (!charsWritten)
			hr = E_INVALIDARG;
		else
			PathQuoteSpaces(Buffer);
	}

	//TODO: load Eraser Icon and store HBITMAP in m_szEraserIcon
	GlobalUnlock (stg.hGlobal);
	ReleaseStgMedium (&stg);
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

HRESULT CCtxMenu::QueryContextMenu(HMENU hmenu, UINT uMenuIndex, UINT uidFirstCmd,
                                   UINT /*uidLastCmd*/, UINT uFlags)
{
	//If the flags include CMF_DEFAULTONLY then we shouldn't do anything.
	if (uFlags & CMF_DEFAULTONLY)
		return MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_NULL, 0);

	//First, create and populate a submenu.
	UINT uID = uidFirstCmd;	
	HMENU hSubmenu = CreatePopupMenu();	

	//Create the submenu, following the order defined in the CEraserLPVERB enum
	InsertMenu    (hSubmenu, CERASER_ERASE, MF_BYPOSITION, uID++,				_T("&Erase"));
	InsertMenu    (hSubmenu, CERASER_SCHEDULE, MF_BYPOSITION, uID++,			_T("&Schedule"));
	InsertMenu    (hSubmenu, CERASER_ERASE_ON_RESTART, MF_BYPOSITION, uID++,	_T("Erase on &Restart"));
	//-------------------------------------------------------------------------
	InsertMenuItem(hSubmenu, CERASER_SEPERATOR_1, TRUE, GetSeperator());
	InsertMenu    (hSubmenu, CERASER_SECURE_MOVE, MF_BYPOSITION, uID++,			_T("Secure &Move"));	
	//-------------------------------------------------------------------------
	InsertMenuItem(hSubmenu, CERASER_SEPERATOR_2, TRUE, GetSeperator());
	InsertMenu    (hSubmenu, CERASER_CUSTOMISE, MF_BYPOSITION, uID++,			_T("&Console"));	

	//Insert the submenu into the Context menu provided by Explorer.
	{
		MENUITEMINFO mii = { sizeof(MENUITEMINFO) };
		mii.wID = uID++;
		mii.fMask = MIIM_SUBMENU | MIIM_STRING | MIIM_ID;
		mii.hSubMenu = hSubmenu;
		mii.dwTypeData = const_cast<wchar_t*>(m_szMenuTitle);

		//Set the bitmap for the registered item. Vista machines will be set using a DIB,
		//older machines will be ownerdrawn.
		OSVERSIONINFO osvi;
		ZeroMemory(&osvi, sizeof(osvi));
		osvi.dwOSVersionInfoSize = sizeof(osvi);

		if (GetVersionEx(&osvi) && osvi.dwPlatformId == VER_PLATFORM_WIN32_NT &&
			osvi.dwMajorVersion >= 6)
		{
			mii.fMask |= MIIM_CHECKMARKS;
			mii.hbmpUnchecked = GetMenuBitmap();
		}
		else
		{
			mii.fMask |= MIIM_FTYPE;
			mii.fType = MFT_OWNERDRAW;
		}

		m_itemID = uMenuIndex++;
		InsertMenuItem(hmenu, m_itemID, TRUE, &mii);
	}

	return MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_NULL, uID - uidFirstCmd);
}

HRESULT CCtxMenu::HandleMenuMsg(UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	return HandleMenuMsg2(uMsg, wParam, lParam, NULL);
}

HRESULT CCtxMenu::HandleMenuMsg2(UINT uMsg, WPARAM wParam, LPARAM lParam, LRESULT* result)
{
	//Skip if we aren't handling our own.
	bool handleResult = false;
	switch (uMsg)
	{
	case WM_MEASUREITEM:
	{
		MEASUREITEMSTRUCT* mis = reinterpret_cast<MEASUREITEMSTRUCT*>(lParam);
		if (mis->CtlID == m_itemID)
			handleResult = OnMeasureItem(mis->itemWidth, mis->itemHeight);
		else
			handleResult = false;
		break;
	}

	case WM_DRAWITEM:
	{
		DRAWITEMSTRUCT* dis = reinterpret_cast<DRAWITEMSTRUCT*>(lParam);
		if (dis->CtlID == m_itemID)
			handleResult = OnDrawItem(dis->hDC, dis->rcItem, dis->itemAction, dis->itemState);
		else
			handleResult = false;
	}
	}

	if (result)
		*result = handleResult;
	return S_OK;
}

bool CCtxMenu::OnMeasureItem(UINT& itemWidth, UINT& itemHeight)
{
	LOGFONT logFont;
	if (!SystemParametersInfo(SPI_GETICONTITLELOGFONT, sizeof(logFont), &logFont, 0))
		return false;

	//Measure the size of the text.
	Handle<HDC> screenDC = GetDC(NULL);
	Handle<HFONT> font = CreateFontIndirect(&logFont);
	SelectObject(screenDC, font);
	SIZE textSize;
	if (!GetTextExtentPoint32(screenDC, m_szMenuTitle, wcslen(m_szMenuTitle), &textSize))
		return false;

	itemWidth = textSize.cx;
	itemHeight = textSize.cy;

	//Account for the size of the bitmap.
	UINT iconWidth = GetSystemMetrics(SM_CXMENUCHECK);
	itemWidth += iconWidth;
	itemHeight = std::max(iconWidth, itemHeight);

	//And remember the minimum size for menu items.
	itemHeight = std::max((int)itemHeight, GetSystemMetrics(SM_CXMENUSIZE));
	return true;
}

bool CCtxMenu::OnDrawItem(HDC hdc, RECT rect, UINT action, UINT state)
{
    bool draw_bitmap_edge = true;

	//Draw the background.
	LOGBRUSH logBrush = { BS_SOLID,
		(state & ODS_SELECTED) ?
			GetSysColor(COLOR_HIGHLIGHT) : GetSysColor(COLOR_MENU),
		0
	};
	Handle<HBRUSH> bgBrush = CreateBrushIndirect(&logBrush);
	FillRect(hdc, &rect, bgBrush);

	//Then the bitmap.
	{
		//Draw the icon with alpha and all first.
		Handle<HICON> icon(GetMenuIcon());
		int iconSize = GetSystemMetrics(SM_CXMENUCHECK);
		int iconMargin = GetSystemMetrics(SM_CXEDGE);
		DrawIconEx(hdc, rect.left + iconMargin, rect.top + (rect.bottom - rect.top - iconSize) / 2,
			icon, 0, 0, 0, bgBrush, DI_NORMAL);

		//Move the rectangle's left bound to the text starting position
		rect.left += iconMargin * 2 + iconSize;
	}
	
	//Draw the text.
	SetBkMode(hdc, TRANSPARENT);
	LOGFONT logFont;
	if (!SystemParametersInfo(SPI_GETICONTITLELOGFONT, sizeof(logFont), &logFont, 0))
		return false;

	/*Handle<HFONT> font = CreateFontIndirect(&logFont);
	SelectObject(hdc, font);*/
	SIZE textSize;
	if (!GetTextExtentPoint32(hdc, m_szMenuTitle, wcslen(m_szMenuTitle), &textSize))
		return false;

	COLORREF oldColour = SetTextColor(hdc, (state & ODS_SELECTED) ?
		GetSysColor(COLOR_HIGHLIGHTTEXT) : GetSysColor(COLOR_MENUTEXT));
	UINT flags = DST_PREFIXTEXT;
	if (state & ODS_NOACCEL)
		flags |= DSS_HIDEPREFIX;
	::DrawState(hdc, NULL, NULL, reinterpret_cast<LPARAM>(m_szMenuTitle), wcslen(m_szMenuTitle),
		rect.left, rect.top + (rect.bottom - rect.top - textSize.cy) / 2, textSize.cx, textSize.cy, flags);
	SetTextColor(hdc, oldColour);
    return true;
}

HRESULT CCtxMenu::GetCommandString(UINT idCmd, UINT uFlags, UINT* /*pwReserved*/,
                                   LPSTR pszName, UINT cchMax)
{
	USES_CONVERSION;

	//Check idCmd, it must be 0 or 1 since we have two menu items.
	if ( idCmd > 2 )
		return E_INVALIDARG;

	//If Explorer is asking for a help string, copy our string into the supplied buffer.
	if (!(uFlags & GCS_HELPTEXT)) 
		return E_INVALIDARG;

	static LPCTSTR szErase        = _T("Erases the currently selected file\r\n");
	static LPCTSTR szEraseUnunsed = _T("Erases the currently selected drive's unused disk space\r\n");
	LPCTSTR pszText = (0 == idCmd) ? szErase : szEraseUnunsed;

	if (uFlags & GCS_UNICODE)
		//We need to cast pszName to a Unicode string, and then use the Unicode string copy API.
		lstrcpynW((LPWSTR)pszName, T2CW(pszText), cchMax);
	else
		//Use the ANSI string copy API to return the help string.
		lstrcpynA(pszName, T2CA(pszText), cchMax);

	return S_OK;
}

/*
usage: Eraser <action> <arguments>
where action is
addtask                 Adds tasks to the current task list.
querymethods            Lists all registered Erasure methods.

global parameters:
--quiet, -q	            Do not create a Console window to display progress.

parameters for addtask:
eraser addtask --method=<methodGUID> (--recycled | --unused=<volume> |  --dir=<directory> | [file1 [file2 [...]]])
--method, -m            The Erasure method to use.
--recycled, -r          Erases files and folders in the recycle bin
--unused, -u            Erases unused space in the volume.
optional arguments: --unused=<drive>[,clusterTips]
clusterTips     If specified, the drive's files will have their cluster tips
erased.
--dir, --directory, -d  Erases files and folders in the directory
optional arguments: --dir=<directory>[,e=excludeMask][,i=includeMask][,delete]
excludeMask     A wildcard expression for files and folders to exclude.
includeMask     A wildcard expression for files and folders to include.
The include mask is applied before the exclude mask.
delete          Deletes the folder at the end of the erasure if specified.
file1 ... fileN         The list of files to erase.

parameters for querymethods:
eraser querymethods

no parameters to set.

All arguments are case sensitive.

*/  

HRESULT CCtxMenu::InvokeCommand ( LPCMINVOKECOMMANDINFO pCmdInfo )
{
	// If lpVerb really points to a string, ignore this function call and bail out.
	if ( HIWORD( pCmdInfo->lpVerb )  != 0)
		return E_INVALIDARG;

	HRESULT result = E_INVALIDARG;
	// final eraser command to call
	string_type command(L"eraser ");
	string_type files, directories, unuseds;

	// compile the eraser command syntax
	for (string_list::const_iterator i = m_szSelectedFiles.begin();
		i != m_szSelectedFiles.end(); ++i)
	{
		files       += L"\"" + *i + L"\" ";
	}
	for (string_list::const_iterator i = m_szSelectedUnused.begin();
		i != m_szSelectedUnused.end(); ++i)
	{
		unuseds     += L"--unused=\"" + *i + L"\" ";
	}
	for (string_list::const_iterator i = m_szSelectedDirectories.begin();
		i != m_szSelectedDirectories.end(); ++i)
	{
		directories += L"--dir=\"" + *i + L"\" ";
	}

	// Get the command index.
	switch(LOWORD(pCmdInfo->lpVerb + 1))
	{
#if 0
	case CERASER_ERASE:
		{
			command += L"addtask " + files + unuseds + directories;
			//result = system(command.c_str());
			break;
		}
	case CERASER_SECURE_MOVE:
		{
			result = S_OK;
			// we need some user interaction, thus we will have a windows form
			// has to be native, so i guess a bit of work
			break;
		}
		// NOT IMPLEMENTED METHODS
		case CERASER_ERASE_ON_RESTART:
		{
			MessageBox (pCmdInfo->hwnd, szMsg, _T("Eraser v6 - Shell Extention Query"), MB_ICONINFORMATION );
			command += S("--restart ") + objects;
			result = system(command.c_str());
			break;
		}
		case CERASER_SCHEDULE:
		{
			command += S("--schedule ") + objects;
			result = system(command.c_str());
			break;
		}
	case CERASER_CONSOLE:
		{
			// interactive eraser console
			break;
		}
#endif
	default:
		{
			TCHAR szMsg [MAX_PATH + 32];
			wsprintf ( szMsg, _T("Invalid Query was submitted, unable to process!\n\nCommand ID = %d\n\n"), LOWORD(pCmdInfo->lpVerb) );
			MessageBox ( pCmdInfo->hwnd, szMsg, _T("Eraser v6 - Shell Extention Query"), MB_ICONINFORMATION );
		}
		return result;
	}
}

static bool AlphaBlt(HDC hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest,
                     int nHeightDest, HDC hdcSrc, int nXOriginSrc, int nYOriginSrc,
                     int nWidthSrc, int nHeightSrc)
{
    BLENDFUNCTION bf;
    bf.BlendOp = AC_SRC_OVER;
    bf.BlendFlags = 0;
    bf.SourceConstantAlpha = 0xff;
    bf.AlphaFormat = AC_SRC_ALPHA;

	return AlphaBlend(hdcDest, nXOriginDest, nYOriginDest, nWidthDest, nHeightDest,
		hdcSrc, nXOriginSrc, nYOriginSrc, nWidthSrc, nHeightSrc, bf) != FALSE;
}

HICON CCtxMenu::GetMenuIcon()
{
	int smIconSize = GetSystemMetrics(SM_CXMENUCHECK);
	return static_cast<HICON>(LoadImage(theApp.m_hInstance, L"Eraser",
		IMAGE_ICON, smIconSize, smIconSize, LR_LOADTRANSPARENT));
}

HBITMAP CCtxMenu::GetMenuBitmap()
{
	BITMAP bitmap;
	ICONINFO iconInfo;
	ZeroMemory(&bitmap, sizeof(bitmap));
	ZeroMemory(&iconInfo, sizeof(iconInfo));
	Handle<HICON> icon(GetMenuIcon());

	//Try to get the icon's size, bitmap and bit depth. We will try to convert
	//the bitmap into a DIB for display on Vista if it contains an alpha channel.
	if (!GetIconInfo(icon, &iconInfo))
		return NULL;

	Handle<HBITMAP> iconMask(iconInfo.hbmMask);
	if (!GetObject(iconInfo.hbmColor, sizeof(BITMAP), &bitmap) ||
		bitmap.bmBitsPixel < 32)
		return iconInfo.hbmColor;

	//If we can't create the DIB section, return the one straight from the icon.
	HBITMAP dibBitmap(CreateDIB(bitmap.bmWidth, bitmap.bmHeight));
	if (!dibBitmap)
		return iconInfo.hbmColor;

	//OK, we've got a DIB. We want to draw the icon onto the DIB to get an alpha
	//channel.
	Handle<HDC> dibDC = CreateCompatibleDC(NULL);
	Handle<HDC> srcDC = CreateCompatibleDC(NULL);
	SelectObject(dibDC, dibBitmap);
	SelectObject(srcDC, iconInfo.hbmColor);
	if (!AlphaBlt(dibDC, 0, 0, bitmap.bmWidth, bitmap.bmHeight, srcDC, 0, 0,
		bitmap.bmWidth, bitmap.bmHeight))
	{
		DeleteObject(dibBitmap);
		return iconInfo.hbmColor;
	}

	DeleteObject(iconInfo.hbmColor);
	return dibBitmap;
}

HBITMAP CCtxMenu::CreateDIB(LONG width, LONG height)
{
	BITMAPINFO info;
    ZeroMemory(&info, sizeof(info));
	info.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
	info.bmiHeader.biWidth = width;
	info.bmiHeader.biHeight = height;
    info.bmiHeader.biPlanes = 1;
	info.bmiHeader.biBitCount = 32;
	info.bmiHeader.biCompression = BI_RGB;
	char* dibBitmapBits = NULL;
	return ::CreateDIBSection(0, &info, DIB_RGB_COLORS,
		reinterpret_cast<void**>(&dibBitmapBits), NULL, 0);
}
}
