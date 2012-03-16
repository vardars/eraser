using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComLib.Lang
{
    /// <summary>
    /// Exception used in script parsing
    /// </summary>
    public class LangException : Exception
    {
        /// <summary>
        /// Line number of exception
        /// </summary>
        public int LineNumber;


        /// <summary>
        /// The path to the script that caused the exception
        /// </summary>
        public string ScriptPath;


        /// <summary>
        /// THe type of error.
        /// </summary>
        public string ErrorType;


        /// <summary>
        /// Error message.
        /// </summary>
        public string Error;


        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="errorType"></param>
        /// <param name="error"></param>
        /// <param name="scriptpath"></param>
        /// <param name="lineNumber"></param>
        public LangException(string errorType, string error, string scriptpath, int lineNumber)
        {
            LineNumber = lineNumber;
            ErrorType = error;
            Error = error;
            ScriptPath = scriptpath;
        }
    }
}
