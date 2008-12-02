// dllmain.cpp : Implementation of DllMain.

#include "stdafx.h"
#include "resource.h"
#include "EraserCtxMenu_i.h"
#include "dllmain.h"

CEraserCtxMenuModule _AtlModule;

class CEraserCtxMenuApp : public CWinApp
{
public:

// Overrides
	virtual BOOL InitInstance();
	virtual int ExitInstance();

	DECLARE_MESSAGE_MAP()
};

BEGIN_MESSAGE_MAP(CEraserCtxMenuApp, CWinApp)
END_MESSAGE_MAP()

CEraserCtxMenuApp theApp;

BOOL CEraserCtxMenuApp::InitInstance()
{
	return CWinApp::InitInstance();
}

int CEraserCtxMenuApp::ExitInstance()
{
	return CWinApp::ExitInstance();
}
