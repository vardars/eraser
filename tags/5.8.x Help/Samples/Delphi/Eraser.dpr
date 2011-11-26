Program Project1;
uses
  Forms,
  eraserdll in 'eraserdll.pas',
  form1 in 'form1.pas'  {Form1Cls};
 
 
begin
  Application.Initialize;
  Application.CreateForm( TForm1Cls, Form1Cls);
  Application.Run;
end.
