VERSION 5.00
Begin VB.Form Form1 
   Caption         =   "Eraser Sample in VB"
   ClientHeight    =   1410
   ClientLeft      =   60
   ClientTop       =   345
   ClientWidth     =   4770
   LinkTopic       =   "Form1"
   ScaleHeight     =   1410
   ScaleWidth      =   4770
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton Erase 
      Caption         =   "&Erase"
      Height          =   375
      Left            =   3720
      TabIndex        =   2
      Top             =   960
      Width           =   975
   End
   Begin VB.TextBox FileName 
      Height          =   375
      Left            =   120
      TabIndex        =   1
      Top             =   960
      Width           =   3495
   End
   Begin VB.Label Label1 
      Caption         =   "Enter file to erase"
      Height          =   735
      Left            =   120
      TabIndex        =   0
      Top             =   120
      Width           =   4575
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Private Sub Erase_Click()
    ' variables
    Dim Result As Long
    Dim Bytes(2) As Long
    Dim Context As Long
    
    Context = 0
    
    ' initialize library
    Result = eraserInit()
    If eraserError(Result) Then
        Label.Caption = "Error: eraserInit returned " & Result
        GoTo Error
    End If
    
    ' create context
    Result = eraserCreateContext(Context)
    If eraserError(Result) Then
        Label.Caption = "Error: eraserCreateContext returned " & Result
        GoTo Error
    End If
    
    ' set data type
    Result = eraserSetDataType(Context, ERASER_DATA_FILES)
    If eraserError(Result) Then
        Label.Caption = "Error: eraserSetDataType returned " & Result
        GoTo Error
    End If
    
    ' add files to erase
    Result = eraserAddItem(Context, FileName.Text, Len(FileName.Text))
    If eraserError(Result) Then
        Label.Caption = "Error: eraserAddItem returned " & Result
        GoTo Error
    End If
    
    ' erase
    Result = eraserStartSync(Context)
    If eraserError(Result) Then
        Label.Caption = "Error: eraserStartSync returned " & Result
        GoTo Error
    End If
    
    ' done erasing, query some statistics
    Label.Caption = "File erased."
    
    ' the second parameter is a 64-bit value, thus passing array ByRef
    Result = eraserStatGetArea(Context, Bytes(1))
    If eraserOK(Result) Then
        Label.Caption = Label.Caption & " (" & Bytes(1) & " bytes)"
    End If
             
    ' and clean up
    GoTo CleanUp
    
Error:
    ' handle error
    
CleanUp:
    ' show Erasing Report
    Result = eraserShowReport(Context, Form1.Hwnd)
    If eraserError(Result) Then
        Label.Caption = Label.Caption & " Oops, eraserShowReport returned " & Result
    End If
    
    ' destroy context
    Result = eraserDestroyContext(Context)
    
    ' clean up library
    Result = eraserEnd()
    
End Sub
