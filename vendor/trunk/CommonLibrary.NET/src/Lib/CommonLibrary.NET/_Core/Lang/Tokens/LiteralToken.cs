using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComLib.Lang
{
    /// <summary>
    /// String, number, bool(true/false), null
    /// </summary>
    internal class LiteralToken : Token
    {
        private object _value;


        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="text">The raw text value</param>
        /// <param name="value">The actual value of the literal</param>
        /// <param name="isKeyword">Whether this is a keyword</param>
        public LiteralToken(string text, object value, bool isKeyword)
        {
            this._text = text;
            this._value = value;
            this._isKeyword = isKeyword;

        }


        /// <summary>
        /// Value of the literal
        /// </summary>
        public override object Value
        {
            get { return _value; }
        }


        // Reserved keywords.
        internal readonly static LiteralToken True =  new LiteralToken("true", true, true);
        internal readonly static LiteralToken False = new LiteralToken("false", false, true);
        internal readonly static LiteralToken Null =  new LiteralToken("null", null, true);
        internal readonly static LiteralToken WhiteSpace = new LiteralToken(string.Empty, string.Empty, true);


        private static IDictionary<string, LiteralToken> _textToLiterals = new Dictionary<string, LiteralToken>()
        {
            { "true", LiteralToken.True },
            { "false", LiteralToken.False },
            { "null", LiteralToken.Null },
            { " ", LiteralToken.WhiteSpace },
        };


        /// <summary>
        /// Determines if the text supplied is a literal token
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsLiteral(string key)
        {
            return _textToLiterals.ContainsKey(key);
        }


        /// <summary>
        /// Get the literal token from the string
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static LiteralToken ToLiteral(string key)
        {
            return _textToLiterals[key];
        }
    }
}
