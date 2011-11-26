Attribute VB_Name = "Module1"
' EraserDll.bas
' Header file for the Eraser Library.
'
' Eraser. Secure data removal. For Windows.
' Copyright © 1997-2001  Sami Tolvanen (sami@tolvanen.com).
'
' This program is free software; you can redistribute it and/or
' modify it under the terms of the GNU General Public License
' as published by the Free Software Foundation; either version 2
' of the License, or (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA
' 02111-1307, USA.


' wParam values
Public Const ERASER_WIPE_BEGIN = 0
Public Const ERASER_WIPE_UPDATE = 1
Public Const ERASER_WIPE_DONE = 2
Public Const ERASER_TEST_PAUSED = 3

' Library type definitions

Public Enum ERASER_METHOD
    ERASER_METHOD_LIBRARY
    ERASER_METHOD_GUTMANN
    ERASER_METHOD_DOD
    ERASER_METHOD_PSEUDORANDOM
End Enum

Public Enum ERASER_DATA_TYPE
    ERASER_DATA_DRIVES
    ERASER_DATA_FILES
End Enum

Public Enum ERASER_OPTIONS_PAGE
    ERASER_PAGE_DRIVE
    ERASER_PAGE_FILES
End Enum

' eraserRemoveFolder options
Public Const ERASER_REMOVE_FOLDERONLY = 0
Public Const ERASER_REMOVE_RECURSIVELY = 1

' display flags
Public Const eraserDispPass = 1         ' Show pass information
Public Const eraserDispTime = 2         ' Show estimated time
Public Const eraserDispMessage = 4      ' [UNUSED] Show message
Public Const eraserDispProgress = 8     ' [UNUSED] Show progress bar
Public Const eraserDispStop = 16        ' [UNUSED] Allow termination
Public Const eraserDispItem = 32        ' [UNUSED] Show item name
Public Const eraserDispInit = 64        ' Set progress to 0 on ERASER_WIPE_BEGIN
Public Const eraserDispReserved = 128   ' [UNUSED]

' bit masks for items to erase
' files
Public Const fileClusterTips = 1
Public Const fileNames = 2
Public Const fileAlternateStreams = 4
' unused disk space
Public Const diskFreeSpace = 32
Public Const diskClusterTips = 64
Public Const diskDirEntries = 128


' Error messages
Public Const ERASER_OK = 0                         ' No error
Public Const ERASER_ERROR = -1                     ' Unspecified error
Public Const ERASER_ERROR_PARAM1 = -2              ' Parameter 1 invalid
Public Const ERASER_ERROR_PARAM2 = -3              ' Parameter 2 invalid
Public Const ERASER_ERROR_PARAM3 = -4              ' Parameter 3 invalid
Public Const ERASER_ERROR_PARAM4 = -5              ' Parameter 4 invalid
Public Const ERASER_ERROR_PARAM5 = -6              ' Parameter 5 invalid
Public Const ERASER_ERROR_PARAM6 = -7              ' Parameter 6 invalid
Public Const ERASER_ERROR_MEMORY = -8              ' Out of memory
Public Const ERASER_ERROR_THREAD = -9              ' Failed to start thread
Public Const ERASER_ERROR_EXCEPTION = -10          ' BUG!
Public Const ERASER_ERROR_CONTEXT = -11            ' Context array full (ERASER_MAX_CONTEXT)
Public Const ERASER_ERROR_INIT = -12               ' Library not initialized (eraserInit())
Public Const ERASER_ERROR_RUNNING = -13            ' Failed because the thread is running
Public Const ERASER_ERROR_NOTRUNNING = -14         ' Failed because the thread not running
Public Const ERASER_ERROR_DENIED = -15             ' Operation not permitted
Public Const ERASER_ERROR_NOTIMPLEMENTED = -32     ' Function not implemented

' Library initialization

' initializes the library, must be called before using
Public Declare Function eraserInit Lib "eraser.dll" () As Long
' cleans up after use
Public Declare Function eraserEnd Lib "eraser.dll" () As Long


' Context creation and destruction

' creates context with predefined settings
Public Declare Function eraserCreateContext Lib "eraser.dll" (ByRef Context As Long) As Long
' creates context and sets an alternative method, pass count and items to erase
Public Declare Function eraserCreateContextEx Lib "eraser.dll" (ByRef Context As Long, ByVal Method As ERASER_METHOD, ByVal Passes As Integer, ByVal Items As Byte) As Long
' destroys a context
Public Declare Function eraserDestroyContext Lib "eraser.dll" (ByVal Context As Long) As Long
' checks the validity of a context, return ERASER_OK if valid
Public Declare Function eraserIsValidContext Lib "eraser.dll" (ByVal Context As Long) As Long


' Data type

' sets context data type
Public Declare Function eraserSetDataType Lib "eraser.dll" (ByVal Context As Long, ByVal DataType As ERASER_DATA_TYPE) As Long
' returns context data type
Public Declare Function eraserGetDataType Lib "eraser.dll" (ByVal Context As Long, ByRef DataType As ERASER_DATA_TYPE) As Long


' Data

' adds item to the context data array
Public Declare Function eraserAddItem Lib "eraser.dll" (ByVal Context As Long, ByVal FileName As String, ByVal NameLength As Integer) As Long
' clears the context data array
Public Declare Function eraserClearItems Lib "eraser.dll" (ByVal Context As Long) As Long


' Notification

' sets the window to notify
Public Declare Function eraserSetWindow Lib "eraser.dll" (ByVal Context As Long, ByVal Hwnd As Long) As Long
' returns the window
Public Declare Function eraserGetWindow Lib "eraser.dll" (ByVal Context As Long, ByRef Hwnd As Long) As Long
' sets the window message
Public Declare Function eraserSetWindowMessage Lib "eraser.dll" (ByVal Context As Long, ByVal Message As Long) As Long
' returns the window message
Public Declare Function eraserGetWindowMessage Lib "eraser.dll" (ByVal Context As Long, ByRef Message As Long) As Long


' Statistics

' returns the erased area
Public Declare Function eraserStatGetArea Lib "eraser.dll" (ByVal Context As Long, ByRef Bytes As Long) As Long
' returns the erased cluster tip area
Public Declare Function eraserStatGetTips Lib "eraser.dll" (ByVal Context As Long, ByRef Bytes As Long) As Long
' returns the amount of data written
Public Declare Function eraserStatGetWiped Lib "eraser.dll" (ByVal Context As Long, ByRef Bytes As Long) As Long
' returns the time used (ms)
Public Declare Function eraserStatGetTime Lib "eraser.dll" (ByVal Context As Long, ByRef MilliSeconds As Long) As Long


' Display

' returns what the UI should show (see above for flag descriptions)
Public Declare Function eraserDispFlags Lib "eraser.dll" (ByVal Context As Long, ByRef Flags As Byte) As Long


' Progress information

' returns an estimate of how long the operation takes to complete
Public Declare Function eraserProgGetTimeLeft Lib "eraser.dll" (ByVal Context As Long, ByRef Seconds As Long) As Long
' returns the completion percent of current item
Public Declare Function eraserProgGetPercent Lib "eraser.dll" (ByVal Context As Long, ByRef Percent As Byte) As Long
' returns the completion percent of the operation
Public Declare Function eraserProgGetTotalPercent Lib "eraser.dll" (ByVal Context As Long, ByRef Percent As Byte) As Long
' returns the index of the current overwriting pass
Public Declare Function eraserProgGetCurrentPass Lib "eraser.dll" (ByVal Context As Long, ByRef Pass As Integer) As Long
' returns the amount of passes
Public Declare Function eraserProgGetPasses Lib "eraser.dll" (ByVal Context As Long, ByRef Passes As Integer) As Long
' returns a message UI can to show to the user telling what is going on
Public Declare Function eraserProgGetMessage Lib "eraser.dll" (ByVal Context As Long, ByRef Message As String, ByRef Length As Integer) As Long
' returns the name of the item that is being processed
Public Declare Function eraserProgGetCurrentDataString Lib "eraser.dll" (ByVal Context As Long, ByRef Data As String, ByRef Length As Integer) As Long


' Control

' starts overwriting in a new thread
Public Declare Function eraserStart Lib "eraser.dll" (ByVal Context As Long) As Long
' starts overwriting
Public Declare Function eraserStartSync Lib "eraser.dll" (ByVal Context As Long) As Long
' stops running task
Public Declare Function eraserStop Lib "eraser.dll" (ByVal Context As Long) As Long
' checks whether task is being processed
Public Declare Function eraserIsRunning Lib "eraser.dll" (ByVal Context As Long, ByRef Running As Byte) As Long


' Result

' checks whether the task was completed successfully
Public Declare Function eraserCompleted Lib "eraser.dll" (ByVal Context As Long, ByRef Completed As Byte) As Long
' checks whether the task failed
Public Declare Function eraserFailed Lib "eraser.dll" (ByVal Context As Long, ByRef Failed As Byte) As Long
' checks whether the task was terminated
Public Declare Function eraserTerminated Lib "eraser.dll" (ByVal Context As Long, ByRef Terminated As Byte) As Long
' returns the amount of error messages in the context array
Public Declare Function eraserErrorStringCount Lib "eraser.dll" (ByVal Context As Long, ByRef Count As Integer) As Long
' retrieves the given error message from the array
Public Declare Function eraserErrorString Lib "eraser.dll" (ByVal Context As Long, ByVal Index As Integer, ByRef Error As String, ByRef Length As Integer) As Long
' returns the amount of failed items in the context array
Public Declare Function eraserFailedCount Lib "eraser.dll" (ByVal Context As Long, ByRef Count As Long) As Long
' retrieves the given failed item from the array
Public Declare Function eraserFailedString Lib "eraser.dll" (ByVal Context As Long, ByVal Index As Long, ByRef Error As String, ByRef Length As Integer) As Long


' Display report

' displays erasing report
Public Declare Function eraserShowReport Lib "eraser.dll" (ByVal Context As Long, ByVal Hwnd As Long) As Long


' Display library options

' displays the options window
Public Declare Function eraserShowOptions Lib "eraser.dll" (ByVal Hwnd As Long, ByVal OptionsPage As ERASER_OPTIONS_PAGE) As Long


' File / directory deletion

' removes a file
Public Declare Function eraserRemoveFile Lib "eraser.dll" (ByVal FileName As String, ByVal NameLength As Integer) As Long
' removes a folder
Public Declare Function eraserRemoveFolder Lib "eraser.dll" (ByVal FolderName As String, ByVal NameLength As Integer, ByVal RemoveType As Byte) As Long


' Helpers

' returns the amount of free disk space on a drive
Public Declare Function eraserGetFreeDiskSpace Lib "eraser.dll" (ByVal Drive As String, ByVal NameLength As Integer, ByRef FreeBytes As Long) As Long

' returns the cluster size of a partition
Public Declare Function eraserGetClusterSize Lib "eraser.dll" (ByVal Drive As String, ByVal NameLength As Integer, ByRef ClusterSize As Long) As Long


' Test mode

' enables test mode --> files will be opened with sharing enabled
' and erasing process will be paused after each overwriting pass
' until eraserTestContinueProcess(...) is called for the handle
Public Declare Function eraserTestEnable Lib "eraser.dll" (ByVal Context As Long) As Long

' continues paused erasing process in test mode
Public Declare Function eraserTestContinueProcess Lib "eraser.dll" (ByVal Context As Long) As Long

Public Function eraserOK(ByVal ReturnValue As Long) As Boolean
    eraserOK = (ReturnValue >= ERASER_OK)
End Function

Public Function eraserError(ByVal ReturnValue As Long) As Boolean
    eraserError = (ReturnValue < ERASER_OK)
End Function
