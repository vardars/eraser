using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;


namespace ComLib.Lang
{
    /// <summary>
    /// Light version of javascript with some "sandbox" features coming up.
    /// Features include:
    /// 1. var declarations
    /// 2. if conditions
    /// 3. while loops
    /// 4. for loops
    /// 5. func calls
    /// </summary>
    public class Interpreter
    {
        //private InterpreterSettings _settings;
        private Scope _scope;


        /// <summary>
        /// Initialize
        /// </summary>
        public Interpreter()
        {
            _scope = new Scope();
        }


        /// <summary>
        /// Scope of the script
        /// </summary>
        public Scope Scope
        {
            get { return _scope; }
        }


        /// <summary>
        /// Parses the script but does not execute it.
        /// </summary>
        /// <param name="scriptPath">Path to the script</param>
        public void ParseFile(string scriptPath)
        {
            var script = ReadFile(scriptPath);
            Parse(script);
        }


        /// <summary>
        /// Parses the script but does not execute it.
        /// </summary>
        /// <param name="script"></param>
        public void Parse(string script)
        {
            var parser = new Parser();
            parser.Parse(script, _scope);
        }


        /// <summary>
        /// Executes the file.
        /// </summary>
        /// <param name="scriptPath">Path to the script</param>
        public void ExecuteFile(string scriptPath)
        {
            var script = ReadFile(scriptPath);
            Execute(script);
        }


        /// <summary>
        /// Executes the script
        /// </summary>
        /// <param name="script">Script text</param>
        public void Execute(string script)
        {
            var parser = new Parser();
            parser.Parse(script, _scope);
            parser.Execute();
        }


        private string ReadFile(string scriptPath)
        {
            if (!File.Exists(scriptPath))
                throw new FileNotFoundException(scriptPath);

            var script = File.ReadAllText(scriptPath);
            return script;
        }
    }


    /// <summary>
    /// Settings for the interpreter
    /// </summary>
    public class InterpreterSettings
    {
        /// <summary>
        /// Whether or not to enable callbacks for function calls.
        /// </summary>
        public bool EnableFunctionCallCallBacks;
    }
}
