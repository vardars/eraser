// dllmain.h : Declaration of module class.

class CEraserCtxMenuModule : public CAtlDllModuleT< CEraserCtxMenuModule >
{
public :
	DECLARE_LIBID(LIBID_EraserCtxMenuLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_ERASERCTXMENU, "{92BDCDEA-D98E-49C2-9851-A4AD15B847EA}")
};

extern class CEraserCtxMenuModule _AtlModule;
