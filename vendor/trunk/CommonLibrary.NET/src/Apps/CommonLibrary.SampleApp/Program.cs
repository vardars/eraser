using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Data;
using System.Data.Common;
using ComLib;
using ComLib.Application;
using CommonLibrary.Tests;



namespace ComLib.Samples
{
    public class SampleAppRunner
    {        
        /// <summary>
        /// Sample application runner.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Lang_Expression_Tests();
            Lang_Parse_Tests();
        }


        static void RunScannerTests()
        {
            var t = new ScannerTests();
            t.CanParseNumber();
            t.CanParseId();
            t.CanParseString();
            t.CanParseLines();
            t.CanParseUntilChars();
            t.CanConsumeSpace();
        }


        static void Lang_Expression_Tests()
        {
            var t = new Lang_Expression_Tests();
            t.Can_Do_Compare_Expressions_On_Constants();
        }


        static void Lang_Parse_Tests()
        {
            var t = new Lang_Parse_Tests();
            t.Can_Do_Single_Assignment_Constant_Expressions();
            t.Can_Do_Single_Assignment_Constant_Math_Expressions();
            t.Can_Do_Single_Assignment_Constant_Compare_Expressions();
            t.Can_Do_Single_Assignment_Constant_Logical_Expressions();
            t.Can_Do_If_Statements_With_Constants();
            t.Can_Do_While_Statements();
            t.Can_Do_Unary_Expressions();
            t.Can_Do_For_Statements();
            t.Can_Do_Multiple_Assignment_Expressions();
        }


        static void RunAutomationTests()
        {
            var t = new AutomationTestsJs();
            t.CanCallFunc();
            t.CanParseFuncCallsUsingIndexPositions();
            t.CanParseFuncCallsUsingJsonNamedParameters();
        }


        static void Main2(string[] args)
        {
            IApp app = new Example_Logging();
            try
            {       
                if (!app.Accept(args)) return;

                app.Init();
                app.Execute();
                app.ShutDown();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : " + ex.Message);
                Console.WriteLine("Error : " + ex.StackTrace);
            }

            Console.WriteLine("Finished... Press enter to exit.");
            Console.ReadKey();
        }


        /// <summary>
        /// Sample application runner.
        /// Does pretty much the same thing as the above.
        /// But the above is shown just to display the API/Usage of ApplicationTemplate.
        /// </summary>
        /// <param name="args"></param>
        static void RunWithTemplateCall(IApp app, string[] args)
        {
            App.Run(app, args);
        }
    }
}
