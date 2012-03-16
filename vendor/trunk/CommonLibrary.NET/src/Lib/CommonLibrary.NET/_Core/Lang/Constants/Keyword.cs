using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComLib.Lang
{
    /// <summary>
    /// Token for the language.
    /// </summary>
    public enum Keyword
    {
        /// <summary>
        /// var
        /// </summary>
        Var,


        /// <summary>
        /// if
        /// </summary>
        If,


        /// <summary>
        /// for
        /// </summary>
        For,


        /// <summary>
        /// while
        /// </summary>
        While,


        /// <summary>
        /// function
        /// </summary>
        Function,


        /// <summary>
        /// break
        /// </summary>
        Break,


        /// <summary>
        /// continue
        /// </summary>
        Continue
    }



    /// <summary>
    /// Operator class helper methods
    /// </summary>
    public class Keywords
    {
        
        private static IDictionary<Keyword, bool> _keywords = new Dictionary<Keyword, bool>()
        {
            { Keyword.Var, true },
            { Keyword.If, true },
            { Keyword.For, true },
            { Keyword.While, true },
            { Keyword.Function, true },
            { Keyword.Break, true },
            { Keyword.Continue, true }
        };


        private static IDictionary<string, Keyword> _textToKeywords = new Dictionary<string, Keyword>()
        {
            { "var", Keyword.Var },
            { "if", Keyword.If }, 
            { "for", Keyword.For },
            { "while", Keyword.While },
            { "function", Keyword.Function },
            { "break", Keyword.Break },
            { "continue", Keyword.Continue }
        };


        /// <summary>
        /// Gets whether or not this is a keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static bool IsKeyword(string keyword)
        {
            return _textToKeywords.ContainsKey(keyword);
        }


        /// <summary>
        /// Gets the enum from the text keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static Keyword ToKeyword(string keyword)
        {
            return _textToKeywords[keyword];
        }
    }
}
