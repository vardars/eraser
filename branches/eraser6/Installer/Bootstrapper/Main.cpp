/* 
 * $Id$
 * Copyright 2008 The Eraser Project
 * Original Author: Joel Low <lowjoel@users.sourceforge.net>
 * Modified By:
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
#include "Bootstrapper.h"

#define STATIC_CLASS L"STATIC"
#define BUTTON_CLASS L"BUTTON"

//Common Controls Version 6
#pragma comment(linker, "\"/manifestdependency:type='Win32' " \
    "name='Microsoft.Windows.Common-Controls' version='6.0.0.0' "\
    "processorArchitecture='x86' publicKeyToken='6595b64144ccf1df' "\
    "language='*'\"")

namespace {
	wchar_t* szWindowClass = L"EraserBootstrapper";
	HINSTANCE hInstance = NULL;
	HWND hWndParent = NULL;
}

bool              InitInstance(HINSTANCE hInstance, HWND& hWnd);
void              SetWindowFont(HWND hWnd);
LRESULT __stdcall WndProc(HWND, UINT, WPARAM, LPARAM);

int APIENTRY _tWinMain(HINSTANCE hInstance, HINSTANCE /*hPrevInstance*/,
                       LPTSTR /*lpCmdLine*/, int nCmdShow)
{
	//Create the parent window and the child controls
	::hInstance = hInstance;
	if (!InitInstance(hInstance, hWndParent))
		return false;

	HWND hWndPanel = CreateWindowExW(0, STATIC_CLASS, NULL, WS_CHILD | WS_VISIBLE,
		0, 0, 294, 104, hWndParent, NULL, hInstance, NULL);
	HWND hWndStatusLbl = CreateWindowExW(0, STATIC_CLASS, L"Extracting setup files...",
		WS_CHILD | WS_VISIBLE, 13, 38, 270, 19, hWndPanel, NULL, hInstance, NULL);
	HWND hWndProgressBar = CreateWindowExW(0, PROGRESS_CLASS, NULL,
		WS_CHILD | WS_VISIBLE, 13, 13, 270, 24, hWndPanel, NULL, hInstance, NULL);
	HWND hWndCancelBtn = CreateWindowExW(0, BUTTON_CLASS, L"Cancel", WS_TABSTOP |
		WS_CHILD | WS_VISIBLE | BS_DEFPUSHBUTTON, 193, 65, 90, 23, hWndPanel, NULL,
		hInstance, NULL);
	if (!hWndPanel || !hWndStatusLbl || !hWndProgressBar || !hWndCancelBtn)
		return false;

	SetWindowFont(hWndPanel);
	SetWindowFont(hWndStatusLbl);
	SetWindowFont(hWndProgressBar);
	SetWindowFont(hWndCancelBtn);

	ShowWindow(hWndParent, nCmdShow);
	UpdateWindow(hWndParent);
	
	//Check for .NET Framework
	if (!HasNetFramework())
	{
		//Install the .NET framework.
	}

	ExtractTempFiles();

	// Main message loop:
	MSG msg;
	while (GetMessage(&msg, NULL, 0, 0))
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}

	return (int) msg.wParam;
}

/// Registers the main window class and creates it.
bool InitInstance(HINSTANCE hInstance, HWND& hWnd)
{
	WNDCLASSEX wcex;
	::ZeroMemory(&wcex, sizeof(wcex));

	wcex.cbSize         = sizeof(WNDCLASSEX);
	wcex.style			= CS_HREDRAW | CS_VREDRAW;
	wcex.lpfnWndProc	= WndProc;
	wcex.cbClsExtra		= 0;
	wcex.cbWndExtra		= 0;
	wcex.hInstance		= hInstance;
	wcex.hIcon			= LoadIcon(hInstance, MAKEINTRESOURCE(BOOTSTRAPPER_ICON));
	wcex.hCursor		= LoadCursor(NULL, IDC_ARROW);
	wcex.hbrBackground	= (HBRUSH)(COLOR_WINDOW + 1);
	wcex.lpszClassName	= szWindowClass;
	wcex.hIconSm		= LoadIcon(wcex.hInstance, MAKEINTRESOURCE(BOOTSTRAPPER_ICON));
	RegisterClassExW(&wcex);
	InitCommonControls();

	//Create the window
	hWnd = CreateWindowW(szWindowClass, L"Eraser Setup", WS_CAPTION | WS_SYSMENU,
		CW_USEDEFAULT, 0, 300, 130, NULL, NULL, hInstance, NULL);

	if (!hWnd)
		return false;

	//Set default settings (font)
	SetWindowFont(hWnd);
	return true;
}

/// Helper function to set the window font for created windows to the system default.
void SetWindowFont(HWND hWnd)
{
	HFONT hWndFont = NULL;
	if (!hWndFont)
	{
		NONCLIENTMETRICS ncm;
		::ZeroMemory(&ncm, sizeof(ncm));
		ncm.cbSize = sizeof(ncm);

		if ( !::SystemParametersInfo(SPI_GETNONCLIENTMETRICS, 0, &ncm, 0) )
		{
#if WINVER >= 0x0600
			// a new field has been added to NONCLIENTMETRICS under Vista, so
			// the call to SystemParametersInfo() fails if we use the struct
			// size incorporating this new value on an older system -- retry
			// without it
			ncm.cbSize -= sizeof(int);
			if ( !::SystemParametersInfo(SPI_GETNONCLIENTMETRICS, 0, &ncm, 0) )
#endif
				return;
		}

		hWndFont = CreateFontIndirectW(&ncm.lfMessageFont);
	}

	SendMessage(hWnd, WM_SETFONT, (WPARAM)hWndFont, MAKELPARAM(TRUE, 0));
}

/// Processes messages for the main window.
/// 
/// Current messages processed:
/// WM_COMMAND	- process the application menu
/// WM_PAINT	- Paint the main window
/// WM_DESTROY	- post a quit message and return
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	switch (message)
	{
	case WM_COMMAND:
	{
		/*int wmId    = LOWORD(wParam);
		switch (wmId)
		{
		default:
			
		}*/
		return DefWindowProc(hWnd, message, wParam, lParam);
	}

	case WM_PAINT:
	{
		PAINTSTRUCT ps;
		HDC hdc = BeginPaint(hWnd, &ps);

		// TODO: Add any drawing code here...
		EndPaint(hWnd, &ps);
		break;
	}

	case WM_DESTROY:
		PostQuitMessage(0);
		break;

	default:
		return DefWindowProc(hWnd, message, wParam, lParam);
	}

	return 0;
}

HWND GetTopWindow()
{
	return hWndParent;
}

void Yield()
{
	MSG msg;
	while (PeekMessage(&msg, (HWND)0, 0, 0, PM_NOREMOVE) && msg.message != WM_QUIT)
    {
		TranslateMessage(&msg);
		DispatchMessage(&msg);
    }
}

std::wstring GetApplicationPath()
{
	wchar_t filePath[MAX_PATH];
	DWORD result = GetModuleFileNameW(hInstance, filePath,
		sizeof(filePath) / sizeof(filePath[0]));

	if (result == 0)
		throw GetErrorMessage(GetLastError());
	return std::wstring(filePath, result);
}

std::wstring GetErrorMessage(DWORD lastError)
{
	unsigned lastBufferSize = 128;
	wchar_t* buffer = NULL;
	DWORD result = 0;

	while (result == 0 || result == lastBufferSize - 1)
	{
		delete[] buffer;
		buffer = new wchar_t[lastBufferSize *= 2];
		result = FormatMessageW(FORMAT_MESSAGE_FROM_SYSTEM, NULL, lastError, 0, buffer,
			lastBufferSize, NULL);
	}

	std::wstring message(buffer);
	delete[] buffer;
	return message;
}
