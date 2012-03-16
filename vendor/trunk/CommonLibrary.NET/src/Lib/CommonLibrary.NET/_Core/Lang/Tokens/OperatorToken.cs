using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComLib.Lang
{
    /// <summary>
    /// Operator as token
    /// </summary>
    internal class OperatorToken : Token
    {
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="op"></param>
        public OperatorToken(string op)
        {
            this._text = op;
            this._isKeyword = false;
        }


        /// <summary>
        /// Get the operator as an enum
        /// </summary>
        /// <returns></returns>
        public static Operator ToOp(string op)
        {
            return Ops.ToOp(op);
        }


        // Binary Math ( + - * / % )
        internal readonly static OperatorToken Plus = new OperatorToken("+");
        internal readonly static OperatorToken Minus = new OperatorToken("-");
        internal readonly static OperatorToken Multiply = new OperatorToken("*");
        internal readonly static OperatorToken Divide = new OperatorToken("/");
        internal readonly static OperatorToken Modulo = new OperatorToken("%");

        // Comparision operators ( < <= > >= == != )
        internal readonly static OperatorToken LessThan = new OperatorToken("<");
        internal readonly static OperatorToken LessThanOrEqual = new OperatorToken("<=");
        internal readonly static OperatorToken MoreThan = new OperatorToken(">");
        internal readonly static OperatorToken MoreThanOrEqual = new OperatorToken(">=");
        internal readonly static OperatorToken EqualEqual = new OperatorToken("==");
        internal readonly static OperatorToken NotEqual = new OperatorToken("!=");

        // Unary ( ++ -- *= /= )
        internal readonly static OperatorToken Increment = new OperatorToken("++");
        internal readonly static OperatorToken Decrement = new OperatorToken("--");
        internal readonly static OperatorToken IncrementAdd = new OperatorToken("+=");
        internal readonly static OperatorToken IncrementSubtract = new OperatorToken("-=");
        internal readonly static OperatorToken IncrementMultiply = new OperatorToken("*=");
        internal readonly static OperatorToken IncrementDivide = new OperatorToken("/=");

        // Various chars
        internal readonly static OperatorToken LeftBrace = new OperatorToken("{");
        internal readonly static OperatorToken RightBrace = new OperatorToken("}");
        internal readonly static OperatorToken LeftParenthesis = new OperatorToken("(");
        internal readonly static OperatorToken RightParenthesis = new OperatorToken(")");
        internal readonly static OperatorToken LeftBracket = new OperatorToken("[");
        internal readonly static OperatorToken RightBracket = new OperatorToken("]");
        internal readonly static OperatorToken Semicolon = new OperatorToken(";");
        internal readonly static OperatorToken Comma = new OperatorToken(",");
        internal readonly static OperatorToken Dot = new OperatorToken(".");
        internal readonly static OperatorToken Colon = new OperatorToken(":");
        internal readonly static OperatorToken Assignment = new OperatorToken("=");

        // Conditional / Logical
        internal readonly static OperatorToken LogicalAnd = new OperatorToken("&&");
        internal readonly static OperatorToken LogicalOr = new OperatorToken("||");
        internal readonly static OperatorToken Not = new OperatorToken("!");
        internal readonly static OperatorToken Conditional = new OperatorToken("?");


        private static IDictionary<string, OperatorToken> _textToOpTokens = new Dictionary<string, OperatorToken>()
        {
            { "*", OperatorToken.Multiply },
            { "/", OperatorToken.Divide },
            { "+", OperatorToken.Plus },
            { "-", OperatorToken.Minus },
            { "%", OperatorToken.Modulo },

            { "<", OperatorToken.LessThan },
            { "<=", OperatorToken.LessThanOrEqual },
            { ">", OperatorToken.MoreThan },
            { ">=", OperatorToken.MoreThanOrEqual },
            { "!=", OperatorToken.NotEqual },
            { "==", OperatorToken.EqualEqual },

            { "=", OperatorToken.Assignment },
            { "!", OperatorToken.Not },
            { ",", OperatorToken.Comma },
            { ";", OperatorToken.Semicolon },

            { "&&", OperatorToken.LogicalAnd },
            { "||", OperatorToken.LogicalOr },

            { "(", OperatorToken.LeftParenthesis },
            { ")", OperatorToken.RightParenthesis },
            { "{", OperatorToken.LeftBrace },
            { "}", OperatorToken.RightBrace },

            { "++", OperatorToken.Increment },
            { "--", OperatorToken.Decrement },
            { "*=", OperatorToken.IncrementMultiply },
            { "/=", OperatorToken.IncrementDivide },
            { "+=", OperatorToken.IncrementAdd },
            { "-=", OperatorToken.IncrementSubtract },
        };


        /// <summary>
        /// Get the keyword token from the string
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static OperatorToken ToToken(string key)
        {
            return _textToOpTokens[key];
        }
    }
}
