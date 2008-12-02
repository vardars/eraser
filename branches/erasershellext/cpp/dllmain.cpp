// dllmain.cpp : Implementation of DllMain.

#include "stdafx.h"
#include "resource.h"
#include "EraserCtxMenu_i.h"
#include "dllmain.h"

CEraserCtxMenuModule _AtlModule;
CEraserCtxMenuApp theApp;

BEGIN_MESSAGE_MAP(CEraserCtxMenuApp, CWinApp)
END_MESSAGE_MAP()

BOOL CEraserCtxMenuApp::InitInstance()
{
	return CWinApp::InitInstance();
}

int CEraserCtxMenuApp::ExitInstance()
{
	return CWinApp::ExitInstance();
}
