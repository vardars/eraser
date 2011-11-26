unit form1;
interface
uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, menus, extctrls, buttons, clipbrd;
type
  TForm1Cls = class(TForm)
    Erase: TButton;
    FileName: TEdit;
    Label1: TLabel;
    procedure Erase_Click(Sender: TObject);
  private
  public
  end;
var
  Form1Cls: TForm1Cls;
implementation
uses eraserdll;
{$R *.dfm}

procedure TForm1Cls.Erase_Click(Sender: TObject);
var
  Bytes: array[0..2] of Integer;
  Context: Integer;
  xResult: Integer;
  FName:pchar;
begin
     //' variables
      Context := 0;
  try
    try
     //' initialize library
      xResult := eraserInit;
      if (eraserError(xResult)) then begin
        Label1.caption := 'Error: eraserInit returned ' + IntToStr(xResult);
        exit;
      end;
     //' create context
      xResult := eraserCreateContext(Context);
      if (eraserError(xResult)) then begin
        Label1.caption := 'Error: eraserCreateContext returned ' + IntToStr(xResult);
        exit;
      end;
     //' set data type
      xResult := eraserSetDataType(Context, ERASER_DATA_FILES);
      if (eraserError(xResult)) then begin
        label1.caption := 'Error: eraserSetDataType returned ' + IntToStr(xResult);
        exit;
      end;
     //' add files to erase
     fName :=pchar(FileName.text);
      xResult := eraserAddItem(Context,fname,length(FileName.text));
      if (eraserError(xResult)) then begin
        label1.caption := 'Error: eraserAddItem returned ' + IntToStr(xResult);
        exit;
      end;
     //' erase
      xResult := eraserStartSync(Context);
      if (eraserError(xResult)) then begin
        label1.caption := 'Error: eraserStartSync returned ' + IntToStr(xResult);
        exit;
      end;
     //' done erasing, query some statistics
      label1.caption := 'File erased.';
     //' the second parameter is a 64-bit value, thus passing array ByRef
      xResult := eraserStatGetArea(Context, Bytes[1]);
      if (eraserOK(xResult)) then begin
        label1.caption := label1.caption + ' (' + IntToStr(Bytes[1]) + ' bytes)';
      end;
    except
     //' handle error
    end;
  finally
     //' show Erasing Report
    xResult := eraserShowReport(Context, Application.Handle);
    if (eraserError(xResult)) then begin
      label1.caption := label1.caption + ' Oops, eraserShowReport returned ' + IntToStr(xResult);
    end;
     //' destroy context
    xResult := eraserDestroyContext(Context);
     //' clean up library
    xResult := eraserEnd;
  end;
end;
end.

