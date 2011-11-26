unit eraserdll;
interface
uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,Variants,
  StdCtrls, menus, extctrls, buttons, clipbrd;
const
  diskClusterTips = 64;
  diskDirEntries = 128;
  diskFreeSpace = 32;
  ERASER_ERROR = -1;
  ERASER_ERROR_CONTEXT = -11;
  ERASER_ERROR_DENIED = -15;
  ERASER_ERROR_EXCEPTION = -10;
  ERASER_ERROR_INIT = -12;
  ERASER_ERROR_MEMORY = -8;
  ERASER_ERROR_NOTIMPLEMENTED = -32;
  ERASER_ERROR_NOTRUNNING = -14;
  ERASER_ERROR_PARAM1 = -2;
  ERASER_ERROR_PARAM2 = -3;
  ERASER_ERROR_PARAM3 = -4;
  ERASER_ERROR_PARAM4 = -5;
  ERASER_ERROR_PARAM5 = -6;
  ERASER_ERROR_PARAM6 = -7;
  ERASER_ERROR_RUNNING = -13;
  ERASER_ERROR_THREAD = -9;
  ERASER_OK = 0;
  ERASER_REMOVE_FOLDERONLY = 0;
  ERASER_REMOVE_RECURSIVELY = 1;
  ERASER_TEST_PAUSED = 3;
  ERASER_WIPE_BEGIN = 0;
  ERASER_WIPE_DONE = 2;
  ERASER_WIPE_UPDATE = 1;
  eraserDispInit = 64;
  eraserDispItem = 32;
  eraserDispMessage = 4;
  eraserDispPass = 1;
  eraserDispProgress = 8;
  eraserDispReserved = 128;
  eraserDispStop = 16;
  eraserDispTime = 2;
  fileAlternateStreams = 4;
  fileClusterTips = 1;
  fileNames = 2;
type
  ERASER_DATA_TYPE = (ERASER_DATA_DRIVES, ERASER_DATA_FILES);
  ERASER_METHOD = (ERASER_METHOD_LIBRARY, ERASER_METHOD_GUTMANN, ERASER_METHOD_DOD,
    ERASER_METHOD_PSEUDORANDOM);
  ERASER_OPTIONS_PAGE = (ERASER_PAGE_DRIVE, ERASER_PAGE_FILES);

// Library initialization

// initializes the library, must be called before using
function eraserInit: Integer; stdcall; external 'eraser.dll';
//cleans up after use
function eraserEnd: Integer; stdcall; external 'eraser.dll';


//Context creation and destruction

//creates context with predefined settings
function eraserCreateContext(var Context: Integer): Integer; stdcall; external 'eraser.dll';
//creates context and sets an alternative method, pass count and items to erase
function eraserCreateContextEx(var Context: Integer; Method: Integer; Passes: Integer; Items: Byte): Integer; stdcall; external 'eraser.dll';
//destroys a context
function eraserDestroyContext(Context: Integer): Integer; stdcall; external 'eraser.dll';
//checks the validity of a context, return ERASER_OK if valid
function eraserIsValidContext(Context: Integer): Integer; stdcall; external 'eraser.dll';


//Data type

//sets context data type
function eraserSetDataType(Context: Integer;  DataType: ERASER_DATA_TYPE): Integer; stdcall; external 'eraser.dll';
//returns context data type
function eraserGetDataType(Context: Integer; var DataType: ERASER_DATA_TYPE): Integer; stdcall; external 'eraser.dll';


//Data

//adds item to the context data array
function eraserAddItem( Context: Integer; const FileName:pchar; const NameLength: Integer ): Integer; stdcall; external 'eraser.dll';
//clears the context data array
function eraserClearItems(Context: Integer): Integer; stdcall; external 'eraser.dll';


//Notification

//sets the window to notify
function eraserSetWindow(var Context: Integer; xHwnd: Integer): Integer; stdcall; external 'eraser.dll';
//returns the window
function eraserGetWindow(var Context: Integer; xHwnd: Integer): Integer; stdcall; external 'eraser.dll';
//sets the window message
function eraserSetWindowMessage(var Context: Integer; Message: Integer): Integer; stdcall; external 'eraser.dll';
//returns the window message
function eraserGetWindowMessage(var Context: Integer; Message: Integer): Integer; stdcall; external 'eraser.dll';


//Statistics

//returns the erased area
function eraserStatGetArea(Context: Integer; var Bytes: Longint): Integer; stdcall; external 'eraser.dll';
//returns the erased cluster tip area
function eraserStatGetTips(Context: Integer; var Bytes: Longint): Integer; stdcall; external 'eraser.dll';
//returns the amount of data written
function eraserStatGetWiped(Context: Integer; var Bytes: Longint): Integer; stdcall; external 'eraser.dll';
//returns the time used (ms)
function eraserStatGetTime(Context: Integer; MilliSeconds: Integer): Integer; stdcall; external 'eraser.dll';


//Display

//returns what the UI should show (see above for flag descriptions)
function eraserDispFlags(var Context: Integer; Flags: Byte): Integer; stdcall; external 'eraser.dll';


//Progress information

//returns an estimate of how Integer the operation takes to complete
function eraserProgGetTimeLeft(var Context: Integer; Seconds: Integer): Integer; stdcall; external 'eraser.dll';
//returns the completion percent of current item
function eraserProgGetPercent(var Context: Integer; Percent: Byte): Integer; stdcall; external 'eraser.dll';
//returns the completion percent of the operation
function eraserProgGetTotalPercent(var Context: Integer; Percent: Byte): Integer; stdcall; external 'eraser.dll';
//returns the index of the current overwriting pass
function eraserProgGetCurrentPass(var Context: Integer; Pass: Integer): Integer; stdcall; external 'eraser.dll';
//returns the amount of passes
function eraserProgGetPasses(var Context: Integer; Passes: Integer): Integer; stdcall; external 'eraser.dll';
//returns a message UI can to show to the user telling what is going on
function eraserProgGetMessage(var Context: Integer; Message: Pchar; Length: Integer): Integer; stdcall; external 'eraser.dll';
//returns the name of the item that is being processed
function eraserProgGetCurrentDataPchar(var Context: Integer; Data: Pchar; Length: Integer): Integer; stdcall; external 'eraser.dll';

//Control

//starts overwriting in a new thread
function eraserStart(Context: Integer): Integer; stdcall; external 'eraser.dll';
//starts overwriting
function eraserStartSync(Context: Integer): Integer; stdcall; external 'eraser.dll';
//stops running task
function eraserStop(Context: Integer): Integer; stdcall; external 'eraser.dll';
//checks whether task is being processed
function eraserIsRunning(var Context: Integer; Running: Byte): Integer; stdcall; external 'eraser.dll';


//Result

//checks whether the task was completed successfully
function eraserCompleted(var Context: Integer; Completed: Byte): Integer; stdcall; external 'eraser.dll';
//checks whether the task failed
function eraserFailed(var Context: Integer; Failed: Byte): Integer; stdcall; external 'eraser.dll';
//checks whether the task was terminated
function eraserTerminated(var Context: Integer; Terminated: Byte): Integer; stdcall; external 'eraser.dll';
//returns the amount of error messages in the context array
function eraserErrorPcharCount(var Context: Integer; Count: Integer): Integer; stdcall; external 'eraser.dll';
//retrieves the given error message from the array
function eraserErrorPchar(var Context: Integer; Index: Integer; Error: Pchar; Length: Integer): Integer; stdcall; external 'eraser.dll';
//returns the amount of failed items in the context array
function eraserFailedCount(var Context: Integer; Count: Integer): Integer; stdcall; external 'eraser.dll';
//retrieves the given failed item from the array
function eraserFailedPchar(var Context: Integer; Index: Integer; Error: Pchar; Length: Integer): Integer; stdcall; external 'eraser.dll';


//Display report

//displays erasing report
function eraserShowReport(Context: Integer; const xHwnd: Hwnd): Integer; stdcall; external 'eraser.dll';


//Display library options

//displays the options window
function eraserShowOptions(xHwnd: Integer; OptionsPage: Integer): Integer; stdcall; external 'eraser.dll';


//File / directory deletion

//removes a file
function eraserRemoveFile(FileName: Pchar; NameLength: Integer): Integer; stdcall; external 'eraser.dll';
//removes a folder
function eraserRemoveFolder(FolderName: Pchar; NameLength: Integer; RemoveType: Byte): Integer; stdcall; external 'eraser.dll';


//Helpers

//returns the amount of free disk space on a drive
function eraserGetFreeDiskSpace(Drive: Pchar; NameLength: Integer; FreeBytes: Integer): Integer; stdcall; external 'eraser.dll';

//returns the cluster size of a partition
function eraserGetClusterSize(Drive: Pchar; NameLength: Integer; ClusterSize: Integer): Integer; stdcall; external 'eraser.dll';


//Test mode

//enables test mode --> files will be opened with sharing enabled
//and erasing process will be paused after each overwriting pass
//until eraserTestContinueProcess(...) is called for the handle
function eraserTestEnable(var Context: Integer): Integer; stdcall; external 'eraser.dll';

//continues paused erasing process in test mode
function eraserTestContinueProcess(var Context: Integer): Integer; stdcall; external 'eraser.dll';

   // Global or Public procs
function eraserOK(ReturnValue: Integer): boolean;
function eraserError(ReturnValue: Integer): boolean;
implementation

function eraserOK(ReturnValue: Integer): boolean;
begin
  eraserOK := (ReturnValue >= ERASER_OK);
end;

function eraserError(ReturnValue: Integer): boolean;
begin
  eraserError := (ReturnValue < ERASER_OK);
end;

end.

