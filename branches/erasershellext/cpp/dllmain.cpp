// dllmain.cpp : Implementation of DllMain.

#include "stdafx.h"
#include "resource.h"
#include "EraserCtxMenu_i.h"
#include "dllmain.h"
#include "compreg.h"
#include "dlldatax.h"

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
#ifdef _MERGE_PROXYSTUB
	if (!PrxDllMain(m_hInstance, DLL_PROCESS_ATTACH, NULL))
		return FALSE;
#endif
	return CWinApp::InitInstance();
}

int CEraserCtxMenuApp::ExitInstance()
{
	return CWinApp::ExitInstance();
}
