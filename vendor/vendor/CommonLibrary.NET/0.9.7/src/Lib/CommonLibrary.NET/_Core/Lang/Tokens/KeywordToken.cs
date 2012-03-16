using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComLib.Lang
{
    /// <summary>
    /// Operator as token
    /// </summary>
    internal class KeywordToken : Token
    {
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="op"></param>
        public KeywordToken(string op)
        {
            this._text = op;
            this._isKeyword = true;
        }


        // Reserved keywords.
        internal readonly static KeywordToken Var = new KeywordToken("var");
        internal readonly static KeywordToken If = new KeywordToken("if");
        internal readonly static KeywordToken Else = new KeywordToken("else");
        internal readonly static KeywordToken Break = new KeywordToken("break");
        internal readonly static KeywordToken Continue = new KeywordToken("continue");
        internal readonly static KeywordToken For = new KeywordToken("for");
        internal readonly static KeywordToken While = new KeywordToken("while");
        internal readonly static KeywordToken Function = new KeywordToken("function");
        internal readonly static KeywordToken Return = new KeywordToken("return");


        private static IDictionary<string, KeywordToken> _textToKeywords = new Dictionary<string, KeywordToken>()
        {
            { "var", KeywordToken.Var },
            { "if", KeywordToken.If }, 
            { "else", KeywordToken.Else }, 
            { "for", KeywordToken.For },
            { "while", KeywordToken.While },
            { "function", KeywordToken.Function },
            { "break", KeywordToken.Break },
            { "continue", KeywordToken.Continue }
        };


        /// <summary>
        /// Get the keyword token from the string
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static KeywordToken ToKeyWord(string key)
        {
            return _textToKeywords[key];
        }
    }
}
