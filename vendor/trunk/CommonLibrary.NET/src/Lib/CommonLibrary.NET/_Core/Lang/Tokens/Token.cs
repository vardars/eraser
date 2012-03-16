using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComLib.Lang
{
    /// <summary>
    /// Token class
    /// </summary>
    internal class Token
    {
        protected string _text;
        protected bool _isKeyword;
        private int _line;
        private int _charPos;


        internal readonly static Token EndToken = new Token();


        /// <summary>
        /// Line number of the token
        /// </summary>
        public int LineNumber
        {
            get { return _line; }
            set { _line = value; }
        }


        /// <summary>
        /// Char position of the token
        /// </summary>
        public int CharPosition
        {
            get { return _charPos; }
            set { _charPos = value; }
        }


        /// <summary>
        /// Text of the token
        /// </summary>
        public virtual string Text
        {
            get { return _text; }
        }


        /// <summary>
        /// Value of the token.
        /// </summary>
        public virtual object Value
        {
            get { return _text; }
        }


        /// <summary>
        /// Whether or not this ia keyword in the lang
        /// </summary>
        public virtual bool IsKeyWord
        {
            get { return _isKeyword; }
        }
    }
}
