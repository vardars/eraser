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

#include "stdafx.h"
#include "CtxMenu.h"
#include "DllMain.h"
#include <sstream>

extern "C"
{
	typedef LONG NTSTATUS;
	enum KEY_INFORMATION_CLASS
	{
		KeyBasicInformation,
		KeyNodeInformation,
		KeyFullInformation,
		KeyNameInformation,
		KeyCachedInformation,
		KeyVirtualizationInformation
	};

	struct KEY_BASIC_INFORMATION
	{
		LARGE_INTEGER LastWriteTime;
		ULONG TitleIndex;
		ULONG NameLength;
		WCHAR Name[1];
	};

	struct KEY_NODE_INFORMATION
	{
		LARGE_INTEGER LastWriteTime;
		ULONG TitleIndex;
		ULONG ClassOffset;
		ULONG ClassLength;
		ULONG NameLength;
		WCHAR Name[1];
	};

	typedef NTSTATUS (*pZwQueryKey)(HANDLE KeyHandle, KEY_INFORMATION_CLASS KeyInformationClass,
		PVOID KeyInformation, ULONG Length, PULONG ResultLength);
	pZwQueryKey ZwQueryKey;
}

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
	const wchar_t* CCtxMenu::MenuTitle = L"Eraser";

	HRESULT CCtxMenu::Initialize(LPCITEMIDLIST pidlFolder, LPDATAOBJECT pDataObj,
	                             HKEY hProgID)
	{
		//Initialise member variables.
		MenuID = 0;
		
		//Determine where the shell extension was invoked from.
		if (GetHKeyPath(hProgID) == L"{645FF040-5081-101B-9F08-00AA002F954E}")
		{
			InvokeReason = INVOKEREASON_RECYCLEBIN;

			//We can't do much other processing: the LPDATAOBJECT parameter contains
			//data that is a private type so we don't know how to query for it.
			return S_OK;
		}

		//Check pidlFolder for the drop path, if it exists. This is for drag-and-drop
		//context menus.
		else if (pidlFolder != NULL)
		{
			InvokeReason = INVOKEREASON_DRAGDROP;

			//Translate the drop path to a location on the filesystem.
			wchar_t dropTargetPath[MAX_PATH];
			if (!SHGetPathFromIDList(pidlFolder, dropTargetPath))
				return E_FAIL;

			DragDropDestinationDirectory = dropTargetPath;
		}

		//Okay, everything else is a simple context menu for a set of selected files/
		//folders/drives.
		else
			InvokeReason = INVOKEREASON_FILEFOLDER;

		//Look for CF_HDROP data in the data object.
		FORMATETC fmt = { CF_HDROP, NULL, DVASPECT_CONTENT, -1, TYMED_HGLOBAL };
		STGMEDIUM stg = { TYMED_HGLOBAL };
		if (FAILED(pDataObj->GetData(&fmt, &stg)))
			//Nope! Return an "invalid argument" error back to Explorer.
			return E_INVALIDARG;

		//Get a pointer to the actual data.
		HDROP hDrop = static_cast<HDROP>(GlobalLock(stg.hGlobal));
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

		//Collect all the files which have been selected.
		HRESULT hr = S_OK;
		WCHAR buffer[MAX_PATH] = {0};
		for (UINT i = 0; i < uNumFiles; i++)
		{
			UINT charsWritten = DragQueryFile(hDrop, i, buffer, sizeof(buffer) / sizeof(buffer[0]));
			if (!charsWritten)
			{
				hr = E_INVALIDARG;
				continue;
			}

			SelectedFiles.push_back(std::wstring(buffer, charsWritten));
		}

		//Clean up.
		GlobalUnlock(stg.hGlobal);
		ReleaseStgMedium(&stg);
		return hr;
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

		//Create the submenu, following the order defined in the CEraserLPVERB enum, creating
		//only items which are applicable.
		Actions applicableActions = GetApplicableActions();
		VerbMenuIndices.clear();
		if (applicableActions & ACTION_ERASE)
		{
			InsertMenu    (hSubmenu, ACTION_ERASE, MF_BYPOSITION, uID++,				_T("&Erase"));
			VerbMenuIndices.push_back(ACTION_ERASE);
		}
		if (applicableActions & ACTION_ERASE_ON_RESTART)
		{
			InsertMenu    (hSubmenu, ACTION_ERASE_ON_RESTART, MF_BYPOSITION, uID++,		_T("Erase on &Restart"));
			VerbMenuIndices.push_back(ACTION_ERASE_ON_RESTART);
		}
		if (applicableActions & ACTION_ERASE_UNUSED_SPACE)
		{
			InsertMenu    (hSubmenu, ACTION_ERASE_UNUSED_SPACE, MF_BYPOSITION, uID++,	_T("Erase &Unused Space"));
			VerbMenuIndices.push_back(ACTION_ERASE_UNUSED_SPACE);
		}
		//-------------------------------------------------------------------------
		if (applicableActions & ACTION_SECURE_MOVE)
		{
			if (uID - uidFirstCmd > 0)
			{
				std::auto_ptr<MENUITEMINFO> separator(GetSeparator());
				InsertMenuItem(hSubmenu, 0, FALSE, separator.get());
			}

			InsertMenu    (hSubmenu, ACTION_SECURE_MOVE, MF_BYPOSITION, uID++,			_T("Secure &Move"));
			VerbMenuIndices.push_back(ACTION_SECURE_MOVE);
		}

		//Insert the submenu into the Context menu provided by Explorer.
		{
			MENUITEMINFO mii = { sizeof(MENUITEMINFO) };
			mii.wID = uID++;
			mii.fMask = MIIM_SUBMENU | MIIM_STRING | MIIM_ID;
			mii.hSubMenu = hSubmenu;
			mii.dwTypeData = const_cast<wchar_t*>(MenuTitle);

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
			else if (InvokeReason != INVOKEREASON_DRAGDROP)
			{
				mii.fMask |= MIIM_FTYPE;
				mii.fType = MFT_OWNERDRAW;
			}

			MenuID = uMenuIndex++;
			InsertMenuItem(hmenu, MenuID, TRUE, &mii);
		}

		return MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_NULL, uID - uidFirstCmd);
	}

	HRESULT CCtxMenu::HandleMenuMsg(UINT uMsg, WPARAM wParam, LPARAM lParam)
	{
		return HandleMenuMsg2(uMsg, wParam, lParam, NULL);
	}

	HRESULT CCtxMenu::HandleMenuMsg2(UINT uMsg, WPARAM /*wParam*/, LPARAM lParam,
	                                 LRESULT* result)
	{
		//Skip if we aren't handling our own.
		bool handleResult = false;
		switch (uMsg)
		{
		case WM_MEASUREITEM:
			{
				MEASUREITEMSTRUCT* mis = reinterpret_cast<MEASUREITEMSTRUCT*>(lParam);
				if (mis->CtlID == MenuID)
					handleResult = OnMeasureItem(mis->itemWidth, mis->itemHeight);
				else
					handleResult = false;
				break;
			}

		case WM_DRAWITEM:
			{
				DRAWITEMSTRUCT* dis = reinterpret_cast<DRAWITEMSTRUCT*>(lParam);
				if (dis->CtlID == MenuID)
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
		if (!GetTextExtentPoint32(screenDC, MenuTitle, static_cast<DWORD>(wcslen(MenuTitle)), &textSize))
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

	bool CCtxMenu::OnDrawItem(HDC hdc, RECT rect, UINT /*action*/, UINT state)
	{
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

		SIZE textSize;
		if (!GetTextExtentPoint32(hdc, MenuTitle, static_cast<DWORD>(wcslen(MenuTitle)), &textSize))
			return false;

		COLORREF oldColour = SetTextColor(hdc, (state & ODS_SELECTED) ?
			GetSysColor(COLOR_HIGHLIGHTTEXT) : GetSysColor(COLOR_MENUTEXT));
		UINT flags = DST_PREFIXTEXT;
		if (state & ODS_NOACCEL)
			flags |= DSS_HIDEPREFIX;
		::DrawState(hdc, NULL, NULL, reinterpret_cast<LPARAM>(MenuTitle), wcslen(MenuTitle),
			rect.left, rect.top + (rect.bottom - rect.top - textSize.cy) / 2, textSize.cx, textSize.cy, flags);
		SetTextColor(hdc, oldColour);
		return true;
	}

	HRESULT CCtxMenu::GetCommandString(UINT_PTR idCmd, UINT uFlags, UINT* /*pwReserved*/,
	                                   LPSTR pszName, UINT cchMax)
	{
		USES_CONVERSION;

		//Check idCmd, it must be 0 or 1 since we have two menu items.
		if (idCmd > 2)
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

	HRESULT CCtxMenu::InvokeCommand(LPCMINVOKECOMMANDINFO pCmdInfo)
	{
		//If lpVerb really points to a string, ignore this function call and bail out.
		if (HIWORD(pCmdInfo->lpVerb) != 0)
			return E_INVALIDARG;

		//If the verb index refers to an item outside the bounds of our VerbMenuIndices
		//vector, exit.
		if (LOWORD(pCmdInfo->lpVerb) > VerbMenuIndices.size())
			return E_INVALIDARG;

		//Build the command line
		string_type commandLine;
		HRESULT result = E_INVALIDARG;
		switch (VerbMenuIndices[LOWORD(pCmdInfo->lpVerb)])
		{
		case ACTION_ERASE:
			{
				//Add Task command.
				commandLine = L"addtask ";

				//Add every item selected onto the command line.
				for (string_list::const_iterator i = SelectedFiles.begin();
					i != SelectedFiles.end(); ++i)
				{
					//Check if the current item is a file or folder.
					std::wstring item(*i);
					if (item.length() > 3 && item[item.length() - 1] == '\\')
						item.erase(item.end() - 1);
					DWORD attributes = GetFileAttributes(item.c_str());

					//Add the correct command line for the file type.
					if (attributes & FILE_ATTRIBUTE_DIRECTORY)
						commandLine += L"\"-d=" + item + L"\" ";
					else
						commandLine += L"\"" + item + L"\" ";
				}

				break;
			}
#if 0
		case CERASER_SECURE_MOVE:
			{
				result = S_OK;
				// we need some user interaction, thus we will have a windows form
				// has to be native, so i guess a bit of work
				break;
			}
			// NOT IMPLEMENTED METHODS
			case ACTION_ERASE_ON_RESTART:
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
			if (!(pCmdInfo->fMask & CMIC_MASK_FLAG_NO_UI))
			{
				std::wstringstream strm;
				strm << L"An invalid command with the ID "
					<< VerbMenuIndices[LOWORD(pCmdInfo->lpVerb)] << L" was requested.\n\n"
					<< L"Eraser was unable to process the request.";
				MessageBox(pCmdInfo->hwnd, strm.str().c_str(), L"Eraser Shell Extension", MB_OK | MB_ICONERROR);
			}
		}

		//Execute the command.
		if (!commandLine.empty())
		{
			//Get the path to this DLL so we can look for Eraser.exe
			wchar_t fileName[MAX_PATH];
			DWORD fileNameLength = GetModuleFileName(theApp.m_hInstance, fileName,
				sizeof(fileName) / sizeof(fileName[0]));
			if (!fileNameLength || fileNameLength >= sizeof(fileName) / sizeof(fileName[0]))
				return E_UNEXPECTED;
			
			//Trim to the last \, then append Eraser.exe
			std::wstring eraserPath(fileName, fileNameLength);
			std::wstring::size_type lastBackslash = eraserPath.rfind('\\');
			if (lastBackslash == std::wstring::npos)
				return E_INVALIDARG;

			eraserPath.erase(eraserPath.begin() + lastBackslash + 1, eraserPath.end());
			if (eraserPath.empty())
				return E_UNEXPECTED;

			eraserPath += L"Eraser.exe";

			//Create the process.
			STARTUPINFO startupInfo;
			ZeroMemory(&startupInfo, sizeof(startupInfo));
			startupInfo.cb = sizeof(startupInfo);
			startupInfo.dwFlags = STARTF_USESHOWWINDOW;
			startupInfo.wShowWindow = static_cast<WORD>(pCmdInfo->nShow);
			PROCESS_INFORMATION processInfo;
			ZeroMemory(&processInfo, sizeof(processInfo));
			
			std::wstring finalCommandLine(L"\"" + eraserPath + L"\" " + commandLine);
			wchar_t* buffer = new wchar_t[finalCommandLine.length() + 1];
			wcscpy_s(buffer, finalCommandLine.length() + 1, finalCommandLine.c_str());

			if (!CreateProcess(NULL, buffer, NULL, NULL, false, CREATE_NO_WINDOW,
				NULL, NULL, &startupInfo, &processInfo))
			{
				if (!(pCmdInfo->fMask & CMIC_MASK_FLAG_NO_UI))
				{
					MessageBox(pCmdInfo->hwnd, L"The Eraser application could not be started. "
						L"Ensure that your installation of Eraser is not corrupted: "
						L"try running the Eraser Setup again to Repair the install.",
						L"Eraser Shell Extension", MB_OK | MB_ICONERROR);
				}

				delete[] buffer;
				return E_UNEXPECTED;
			}

			delete[] buffer;
			CloseHandle(processInfo.hThread);
			CloseHandle(processInfo.hProcess);
		}

		return result;
	}

	CCtxMenu::Actions CCtxMenu::GetApplicableActions()
	{
		unsigned result = 0;
		
		//First decide the actions which are applicable to the current invocation
		//reason.
		switch (InvokeReason)
		{
		case INVOKEREASON_RECYCLEBIN:
			result |= ACTION_ERASE | ACTION_ERASE_ON_RESTART;
			break;
		case INVOKEREASON_FILEFOLDER:
			result |= ACTION_ERASE | ACTION_ERASE_ON_RESTART | ACTION_ERASE_UNUSED_SPACE;
		case INVOKEREASON_DRAGDROP:
			result |= ACTION_SECURE_MOVE;
		}

		//Remove actions that don't apply to the current invocation reason.
		for (std::list<std::wstring>::const_iterator i = SelectedFiles.begin();
			i != SelectedFiles.end(); ++i)
		{
			//Remove trailing slashes if they are directories.
			std::wstring item(*i);

			//Check if the path is a path to a volume, if it is not, remove the
			//erase unused space verb.
			wchar_t volumeName[MAX_PATH];
			if (!GetVolumeNameForVolumeMountPoint(item.c_str(), volumeName,
				sizeof(volumeName) / sizeof(volumeName[0])))
			{
				result &= ~ACTION_ERASE_UNUSED_SPACE;
			}
		}

		return static_cast<Actions>(result);
	}

	std::wstring CCtxMenu::GetHKeyPath(HKEY handle)
	{
		ZwQueryKey = reinterpret_cast<pZwQueryKey>(GetProcAddress(
			LoadLibrary(L"Ntdll.dll"), "ZwQueryKey"));
		ULONG keyInfoSize = sizeof(KEY_NODE_INFORMATION);
		KEY_NODE_INFORMATION* keyInfo = NULL;
		NTSTATUS queryResult = ERROR_MORE_DATA;

		while (queryResult == ERROR_MORE_DATA)
		{
			delete[] keyInfo;
			keyInfo = reinterpret_cast<KEY_NODE_INFORMATION*>(
				new char[keyInfoSize += 512]);
			::ZeroMemory(keyInfo, keyInfoSize);
			queryResult = ZwQueryKey(handle, KeyNodeInformation, keyInfo,
				keyInfoSize, &keyInfoSize);
		}

		std::wstring result(keyInfo->Name);
		delete[] keyInfo;
		return result;
	}

	MENUITEMINFO* CCtxMenu::GetSeparator()
	{
		MENUITEMINFO* mii = new MENUITEMINFO();
		mii->cbSize = sizeof(MENUITEMINFO);
		mii->fMask = MIIM_TYPE;
		mii->fType = MF_SEPARATOR;
		return mii;
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

		//Try converting the DDB into a DIB.
		DIBSECTION dibSection;
		HBITMAP dib = CreateDIB(bitmap.bmWidth, bitmap.bmHeight, NULL);
		if (!GetObject(dib, sizeof(dibSection), &dibSection) ||
			!GetDIBits(CreateCompatibleDC(NULL), iconInfo.hbmColor, 0, bitmap.bmHeight,
				dibSection.dsBm.bmBits, reinterpret_cast<BITMAPINFO*>(&dibSection.dsBmih),
				DIB_RGB_COLORS))
		{
			return iconInfo.hbmColor;
		}

		return dib;
	}

	HBITMAP CCtxMenu::CreateDIB(LONG width, LONG height, char** bitmapBits)
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
			reinterpret_cast<void**>(bitmapBits ? bitmapBits : &dibBitmapBits), NULL, 0);
	}
}
