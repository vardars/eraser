using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ComLib.Lang
{
    /// <summary>
    /// Parses script in terms of tokens.
    /// Main method is NextToken();
    /// A script can be broken down into a sequence of tokens.
    /// e.g.
    /// 
    /// 1. var name = "kishore";
    /// Tokens:
    /// 
    /// VALUE:         TYPE:
    /// 1. var         keyword
    /// 2. name        id
    /// 3. =           operator
    /// 4. "kishore"   literal
    /// 5. ;           operator
    /// </summary>
    public class Lexer
    {
        private Scanner _scanner;
        private Token _lastToken;
        private int _lineNumber = 1;
        private int _lineCharPosition = 0;
        private static IDictionary<char, bool> _opChars = new Dictionary<char, bool>()
        {
            { '*', true},
            { '/', true},
            { '+', true},
            { '-', true},
            { '%', true},
            { '<', true},
            { '>', true},
            { '=', true},
            { '!', true},
            { '&', true},
            { '|', true}
             
        };


        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="scanner"></param>
        public Lexer(Scanner scanner)
        {
            _scanner = scanner;
        }


        /// <summary>
        /// The current line number.
        /// </summary>
        public int LineNumber { get { return _lineNumber; } }


        /// <summary>
        /// The char position on the current line.
        /// </summary>
        public int LineCharPos { get { return _lineCharPosition; } }


        /// <summary>
        /// The current token.
        /// </summary>
        internal Token LastToken { get { return _lastToken; } }
        
       
        /// <summary>
        /// Expect the token type supplied.
        /// </summary>
        public void Expect()
        {
        }

        
        /// <summary>
        /// Reads the next token from the reader.
        /// </summary>
        /// <returns> A token, or <c>null</c> if there are no more tokens. </returns>
        internal Token NextToken()
        {
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // LEXER ALWAYS READS NEXT CHAR
            char c = _scanner.ReadChar();
            char n = _scanner.PeekChar();
            
            if (_scanner.IsEnd())
            {
                _lastToken = Token.EndToken;
            }
            // Empty space.
            else if ( c == ' ' || c == '\t' )
            {
                _scanner.ConsumeWhiteSpace(false, false);
                _lastToken = LiteralToken.WhiteSpace;
            }
            // Variable
            else if (IsStartChar(c))
            {
                _lastToken = ReadWord();
            }
            // Operator ( Math, Compare, Increment ) * / + -, < < > >= ! =
            else if ( IsOp(c) == true)
            {
                _lastToken = ReadOperator();
            }
            else if (c == '(')
            {
                _lastToken = OperatorToken.LeftParenthesis;
            }
            else if (c == ')')
            {
                _lastToken = OperatorToken.RightParenthesis;
            }
            else if (c == '{')
            {
                _lastToken = OperatorToken.LeftBrace;
            }
            else if (c == '}')
            {
                _lastToken = OperatorToken.RightBrace;
            }
            else if (c == ',')
            {
                _lastToken = OperatorToken.Comma;
            }
            else if (c == ';')
            {
                _lastToken = OperatorToken.Semicolon;
            }
            // String literal
            else if (c == '"' || c == '\'')
            {
                _lastToken = ReadString();
            }
            else if (IsNumeric(c))
            {
                _lastToken = ReadNumber();
            }
            // Single line
            else if (c == '/' && n == '/')
            {
                var result = _scanner.ReadLine(false);
                if (result.Success) _lineNumber++;
            }
            // Multi-line
            else if (c == '/' && n == '*')
            {
                var result = _scanner.ReadUntilChars(false, '*', '/');
                if (result.Success)
                {
                    _lineNumber += result.Lines;
                }
            }
            return _lastToken;
        }


        /*
        /// <summary>
        /// Reads the next token from the reader.
        /// </summary>
        /// <returns> A token, or <c>null</c> if there are no more tokens. </returns>
        internal Token PeekToken()
        {
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // LEXER ALWAYS READS NEXT CHAR
            char c = _scanner.PeekChar();
            char n = _scanner.PeekChar(2);

            Token peekToken = null;

            if (_scanner.IsEnd())
            {
                peekToken = Token.EndToken;
            }
            // Empty space.
            else if (c == ' ' || c == '\t')
            {
                peekToken = LiteralToken.WhiteSpace;
            }
            // Variable
            else if (IsStartChar(c))
            {
                peekToken = ReadWord();
            }
            // Operator ( Math, Compare, Increment ) * / + -, < < > >= ! =
            else if (IsOp(c) == true)
            {
                peekToken = ReadOperator();
            }
            // String literal
            else if (c == '"' || c == '\'')
            {
                peekToken = ReadString();
            }
            else if (IsNumeric(c))
            {
                peekToken = ReadNumber();
            }
            // Single line
            else if (c == '/' && n == '/')
            {
                var result = _scanner.ReadLine(false);
                if (result.Success) _lineNumber++;
            }
            // Multi-line
            else if (c == '/' && n == '*')
            {
                var result = _scanner.ReadUntilChars(false, '*', '/');
                if (result.Success)
                {
                    _lineNumber += result.Lines;
                }
            }
            else if (c == ';')
            {
                peekToken = OperatorToken.Semicolon;
            }
            return peekToken;
        }
        */


        /// <summary>
        /// Read word
        /// </summary>
        /// <returns></returns>
        private Token ReadWord()
        {
            var result = _scanner.ReadId(false, false);

            // true / false / null
            if (LiteralToken.IsLiteral(result.Text))
                return LiteralToken.ToLiteral(result.Text);

            // var / for / while
            if (Keywords.IsKeyword(result.Text))
                return KeywordToken.ToKeyWord(result.Text);

            return new IdToken(result.Text);
        }


        /// <summary>
        /// Read number
        /// </summary>
        /// <returns></returns>
        private Token ReadNumber()
        {
            var result = _scanner.ReadNumber(false, false);
            return new LiteralToken(result.Text, Convert.ToDouble(result.Text), false);
        }


        /// <summary>
        /// Read an operator
        /// </summary>
        /// <returns></returns>
        private Token ReadOperator()
        {
            var result = _scanner.ReadChars(_opChars, false, false);
            return OperatorToken.ToToken(result.Text);
        }


        /// <summary>
        /// Reads a string either in quote or double quote format.
        /// </summary>
        /// <returns></returns>
        private Token ReadString()
        {
            // Either ' or "
            char quote = _scanner.CurrentChar;
            var result = _scanner.ReadString(quote, setPosAfterToken: false);
            return new LiteralToken(result.Text, result.Text, false);
        }


        private static bool IsStartChar(int c)
        {
            return c == '$' || c == '_' || ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z');
        }


        private static bool IsOp(char c)
        {
            return IsMathOp(c) || IsCompareOp(c) || IsLogicOp(c);
        }


        private static bool IsMathOp(char c)
        {
            return  c == '*' || c == '/' || c == '+' || c == '-' || c == '%';
        }


        private static bool IsLogicOp(char c)
        {
            return c == '&' || c == '|';
        }


        private static bool IsCompareOp(char c)
        {
            return c == '<' || c == '>' || c == '!' || c == '=';
        }

        
        private bool IsNumeric(char c)
        {
            return c == '.' || (c >= '0' && c <= '9');
        }

        
        private bool IsStringStart(char c)
        {
            return c == '"' || c == '\'';
        }


        private bool IsWhiteSpace(char c)
        {
            return c == ' ' || c == '\t';
        }
    }
}
