// dllmain.h : Declaration of module class.
#pragma once

class CEraserCtxMenuModule : public CAtlDllModuleT< CEraserCtxMenuModule >
{
public :
	DECLARE_LIBID(LIBID_EraserCtxMenuLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_ERASERCTXMENU, "{92BDCDEA-D98E-49C2-9851-A4AD15B847EA}")
};

class CEraserCtxMenuApp : public CWinApp
{
public:
	virtual BOOL InitInstance();
	virtual int ExitInstance();

	DECLARE_MESSAGE_MAP()
};

extern CEraserCtxMenuModule _AtlModule;
extern CEraserCtxMenuApp theApp;
