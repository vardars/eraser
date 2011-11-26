object Form1Cls: TForm1Cls
  Left = 377
  Top = 204
  Width = 374
  Height = 224
  Caption = 'Eraser Sample in Delphi'
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 8
    Top = 8
    Width = 82
    Height = 13
    Caption = 'Enter file to erase'
  end
  object Erase: TButton
    Left = 265
    Top = 68
    Width = 69
    Height = 26
    Caption = '&Erase'
    TabOrder = 0
    OnClick = Erase_Click
  end
  object FileName: TEdit
    Left = 8
    Top = 68
    Width = 249
    Height = 21
    TabOrder = 1
    Text = 'h:\\eraser\\eraser-source\\samples\\delphi\\form1.dcu'
  end
end
