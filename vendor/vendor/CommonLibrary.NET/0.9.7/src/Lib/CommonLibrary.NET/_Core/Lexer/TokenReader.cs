/*
 * Author: Kishore Reddy
 * Url: http://commonlibrarynet.codeplex.com/
 * Title: CommonLibrary.NET
 * Copyright: ? 2009 Kishore Reddy
 * License: LGPL License
 * LicenseUrl: http://commonlibrarynet.codeplex.com/license
 * Description: A C# based .NET 3.5 Open-Source collection of reusable components.
 * Usage: Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;


namespace ComLib
{
    /// <summary>
    /// Settings for the token reader class.
    /// </summary>
    public class TokenReaderSettings
    {
        /// <summary>
        /// Char used to escape.
        /// </summary>
        public string EscapeChar;


        /// <summary>
        /// Tokens
        /// </summary>
        public string[] Tokens;


        /// <summary>
        /// White space tokens.
        /// </summary>
        public string[] WhiteSpaceTokens;


        /// <summary>
        /// End of line chars.
        /// </summary>
        public string[] EolTokens;
    }



    /// <summary>
    /// Interface for a Token reader.
    /// </summary>
    public interface IScanner
    {
        /// <summary>
        /// Register the eol characters.
        /// </summary>
        /// <param name="eolchars">Dictionary with eol characters.</param>
        void RegisterEol(IDictionary<string, string> eolchars);


        /// <summary>
        /// Register whitespace characters.
        /// </summary>
        /// <param name="whitespaceChars">Dictionary with whitespace characters.</param>
        void RegisterWhiteSpace(IDictionary<string, string> whitespaceChars);


        /// <summary>
        /// Initializes the token reader.
        /// </summary>
        /// <param name="text">Text to use.</param>
        /// <param name="escapeChar">Escape character.</param>
        /// <param name="tokens">Array with tokens.</param>
        /// <param name="whiteSpaceTokens">Array with whitespace tokens.</param>
        /// <param name="eolTokens">Array with eol tokens.</param>
        void Init(string text, string escapeChar, string[] tokens, string[] whiteSpaceTokens, string[] eolTokens);


        /// <summary>
        /// Initializes the token reader.
        /// </summary>
        /// <param name="text">Text to use.</param>
        /// <param name="settings">Instance with token reader settings.</param>
        void Init(string text, TokenReaderSettings settings);


        /// <summary>
        /// Resets the token reader.
        /// </summary>
        void Reset();

        /// <summary>
        /// Get the current position.
        /// </summary>
        int Position { get; }


        /// <summary>
        /// Peek at the current character.
        /// </summary>
        /// <returns>Current character.</returns>
        string PeekChar();

        
        /// <summary>
        /// Gets the char at the nth position from the current char index.
        /// </summary>
        /// <param name="countFromCurrentCharIndex"></param>
        /// <returns></returns>
        string PeekChar(int countFromCurrentCharIndex);


        /// <summary>
        /// Peek at a series of characters.
        /// </summary>
        /// <param name="count">Number of characters.</param>
        /// <returns>Current characters.</returns>
        string PeekChars(int count);
        

        /// <summary>
        /// Peek at the current line.
        /// </summary>
        /// <returns>Current line.</returns>
        string PeekLine();

        /// <summary>
        /// Consume a character.
        /// </summary>
        void ConsumeChar();


        /// <summary>
        /// Consume a number of characters.
        /// </summary>
        /// <param name="count">Number of characters to consume.</param>
        void ConsumeChars(int count);


        /// <summary>
        /// Consume the whitespace without reading anything.
        /// </summary>
        void ConsumeWhiteSpace();


        /// <summary>
        /// Consume the whitespace after optionally
        /// reading a character.
        /// </summary>
        /// <param name="readFirst">True to read a character
        /// before consuming the whitespace.</param>
        void ConsumeWhiteSpace(bool readFirst);


        /// <summary>
        /// Consumes the whitespace.
        /// </summary>
        /// <param name="readFirst">True to read a character
        /// before consuming the whitepsace.</param>
        /// <param name="consumeLastWhiteSpace"></param>
        void ConsumeWhiteSpace(bool readFirst, bool consumeLastWhiteSpace);


        /// <summary>
        /// Consume a new line.
        /// </summary>
        void ConsumeNewLine();

        /// <summary>
        /// Consumes characters up to new line
        /// </summary>
        void ConsumeToNewLine(bool includeNewLine);


        /// <summary>
        /// Consume new lines.
        /// </summary>
        void ConsumeNewLines();


        /// <summary>
        /// Consume until the chars found.
        /// </summary>
        /// <param name="pattern">The pattern to consume chars to.</param>
        /// <param name="includePatternInConsumption">Wether or not to consume the pattern as well.</param>
        /// <param name="movePastEndOfPattern">Whether or not to move to the ending position of the pattern</param>
        /// <param name="moveToStartOfPattern">Whether or not to move to the starting position of the pattern</param>
        bool ConsumeUntil(string pattern, bool includePatternInConsumption, bool moveToStartOfPattern, bool movePastEndOfPattern);
        

        /// <summary>
        /// Read the previous character.
        /// </summary>
        void ReadBackChar();


        /// <summary>
        /// Read previous characters.
        /// </summary>
        /// <param name="count">Number of characters to read.</param>
        void ReadBackChar(int count);
        

        /// <summary>
        /// Read the current character.
        /// </summary>
        /// <returns>Character read.</returns>
        string ReadChar();


        /// <summary>
        /// Read a series of characters.
        /// </summary>
        /// <param name="count">Number of characters to read.</param>
        /// <returns>Characters read.</returns>
        string ReadChars(int count);


        /// <summary>
        /// ReadToken until one of the endchars is found
        /// </summary>
        /// <param name="endChars">List of possible end chars which halts reading further.</param>
        /// <param name="includeEndChar">True to include end character.</param>
        /// <param name="advanceFirst">True to advance before reading.</param>
        /// <param name="readPastEndChar">True to read past the end character.</param>
        /// <returns></returns>
        string ReadTokenUntil(char[] endChars, bool includeEndChar = false, bool advanceFirst = false, bool readPastEndChar = false);


        /// <summary>
        /// Read everything up to an eol.
        /// </summary>
        /// <returns>String read.</returns>
        string ReadToEol();


        /// <summary>
        /// Read a token.
        /// </summary>
        /// <param name="endChar">Ending character.</param>
        /// <param name="escapeChar">Escape character.</param>
        /// <param name="includeEndChar">True to include end character.</param>
        /// <param name="advanceFirst">True to advance before reading.</param>
        /// <param name="expectEndChar">True to expect an end charachter.</param>
        /// <param name="readPastEndChar">True to read past the end character.</param>
        /// <returns>Contens of token read.</returns>
        string ReadToken(string endChar, string escapeChar, bool includeEndChar, bool advanceFirst, bool expectEndChar, bool readPastEndChar);


        /// <summary>
        /// Read a token.
        /// </summary>
        /// <param name="endChar">Ending character.</param>
        /// <param name="escapeChar">Escape character.</param>
        /// <param name="advanceFirst">True to advance before reading.</param>
        /// <param name="expectEndChar">True to expect an end charachter.</param>
        /// <param name="includeEndChar">True to include an end character.</param>
        /// <param name="moveToEndChar">True to move to the end character.</param>
        /// <param name="readPastEndChar">True to read past the end character.</param>
        /// <returns>Contents of token read.</returns>
        string ReadTokenUntil(string endChar, string escapeChar, bool advanceFirst, bool expectEndChar, bool includeEndChar, bool moveToEndChar, bool readPastEndChar);


        /// <summary>
        /// Get the current character.
        /// </summary>
        string CurrentChar { get; }


        /// <summary>
        /// Get the previous character.
        /// </summary>
        string PreviousChar { get; }


        /// <summary>
        /// Get the previous character that was not escaped.
        /// </summary>
        string PreviousCharAny { get; }


        /// <summary>
        /// Get the current index.
        /// </summary>
        /// <returns>Current character index.</returns>
        int CurrentCharIndex();


        /// <summary>
        /// Check for an escape char.
        /// </summary>
        /// <returns>True if the current char is an escape char.</returns>
        bool IsEscape();


        /// <summary>
        /// Check for a token.
        /// </summary>
        /// <returns>True if the current char is a token.</returns>
        bool IsToken();


        /// <summary>
        /// Whether or not the current sequence of chars matches the token supplied.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ignoreCase">Whether or not to ignore the case</param>
        /// <returns></returns>
        bool IsToken(string token, bool ignoreCase = false);


        /// <summary>
        /// Check for an end of stream.
        /// </summary>
        /// <returns>True if the end of the stream has been reached.</returns>
        bool IsEnd();


        /// <summary>
        /// Check for being at the last character.
        /// </summary>
        /// <returns>True if the last character is the current character.</returns>
        bool IsAtEnd();


        /// <summary>
        /// Check for an eol.
        /// </summary>
        /// <returns>True if the current character is an eol.</returns>
        bool IsEol();


        /// <summary>
        /// Check for an eol.
        /// </summary>
        /// <param name="eolChars">Dictionary with eol chars.</param>
        /// <returns>True if the current character is an eol.</returns>
        bool IsEol(IDictionary eolChars);


        /// <summary>
        /// Check for whitespace.
        /// </summary>
        /// <param name="whitespaceChars">Dictionary with whitespace chars.</param>
        /// <returns>True if the current character is a whitespace.</returns>
        bool IsWhiteSpace(IDictionary whitespaceChars);


        /// <summary>
        /// Check for whitespace.
        /// </summary>
        /// <returns>True if the current character is a whitespace.</returns>
        bool IsWhiteSpace();


        /// <summary>
        /// Get a dictionary with the eol characters.
        /// </summary>
        IDictionary<string, string> EolChars { get; }
    }


    /// <summary>
    /// This class implements a token reader.
    /// </summary>
    public class Scanner : IScanner
    {
        private int _pos;
        private string _text; 
        private IDictionary<string, string> _whiteSpaceChars;
        private IDictionary<string, string> _eolChars;
        private IDictionary<string, string> _tokens;
        private Dictionary<string, string> _readonlyWhiteSpaceMap;
        private Dictionary<string, string> _readonlyEolMap;
        private string _currentChar;
        private StringBuilder _backBuffer;
        private string _escapeChar;
        //private int _currentCharInt;
        private int LAST_POSITION;
        private List<string> _errors = new List<string>();
        private string END_CHAR = "";



        /// <summary>
        /// Initialize this instance with defaults.
        /// </summary>
        public Scanner() : this(string.Empty)
        {
        }


        /// <summary>
        /// Initialize with text to parse.
        /// </summary>
        /// <param name="text"></param>
        public Scanner(string text)
        {
            Init(text, "\\", new string[] { "\"", "'" }, new string[] { " ", "\t" }, new string[] {"\n", "\r\n"} );
        }


        /// <summary>
        /// Initialize this instance with supplied parameters.
        /// </summary>
        /// <param name="text">Text to use.</param>
        /// <param name="escapeChar">Escape character.</param>
        /// <param name="tokens">Array with tokens.</param>
        /// <param name="whiteSpaceTokens">Array with whitespace tokens.</param>
        /// <param name="eolChars">Array with eol tokens.</param>
        public Scanner(string text, string escapeChar, string[] tokens, string[] whiteSpaceTokens, string[] eolChars)
        {
            Init(text, escapeChar, tokens, whiteSpaceTokens, eolChars);
        }


        /// <summary>
        /// Initialize using settings.
        /// </summary>
        /// <param name="text">Text to use.</param>
        /// <param name="settings">Instance with token reader settings.</param>
        public void Init(string text, TokenReaderSettings settings)
        {            
            Init(text, settings.EscapeChar, settings.Tokens, settings.WhiteSpaceTokens, settings.EolTokens);
        }
        
        
        /// <summary>
        /// Initialize using the supplied parameters.
        /// </summary>
        /// <param name="text">Text to read.</param>
        /// <param name="escapeChar">Escape character.</param>
        /// <param name="tokens">Array with tokens.</param>
        /// <param name="whiteSpaceTokens">Array with whitespace tokens.</param>
        /// <param name="eolTokens">Array with eol tokens.</param>
        public void Init(string text, string escapeChar, string[] tokens, string[] whiteSpaceTokens, string[] eolTokens)
        {
            Reset();
            _text = text;
            LAST_POSITION = _text.Length - 1;
            _tokens = ToDictionary(tokens);
            _whiteSpaceChars = ToDictionary(whiteSpaceTokens);
            _eolChars = ToDictionary(eolTokens);

            _readonlyEolMap = new Dictionary<string, string>(_eolChars);
            _readonlyWhiteSpaceMap = new Dictionary<string, string>(_whiteSpaceChars);
        }

        
        #region IScanner Members
        /// <summary>
        /// The current position.
        /// </summary>
        public int Position
        {
            get { return _pos; }
        }


        /// <summary>
        /// Store the end of line chars.
        /// </summary>
        /// /// <param name="eolchars">Dictionary with eol characters.</param>
        public void RegisterEol(IDictionary<string, string> eolchars)
        {
            _eolChars = eolchars;
        }

        
        /// <summary>
        /// Store the white space chars.
        /// </summary>
        /// <param name="whitespaceChars">Dictionary with whitespace characters.</param>
        public void RegisterWhiteSpace(IDictionary<string, string> whitespaceChars)
        {
            _whiteSpaceChars = whitespaceChars;
        }


        /// <summary>
        /// Reset reader for parsing again.
        /// </summary>
        public void Reset()
        {
            _pos = -1;
            _text = string.Empty ;
            _backBuffer = new StringBuilder();
            _whiteSpaceChars = new Dictionary<string, string>();
            _tokens = new Dictionary<string, string>();
            _eolChars = new Dictionary<string, string>();
            _escapeChar = "\\";            
        }


        /// <summary>
        /// Returns the char at current position + 1.
        /// </summary>
        /// <returns>Next char or string.empty if end of text.</returns>
        public string PeekChar()
        {
            // Validate.
            // a b c d e
            // 0 1 2 3 4
            // Lenght = 5
            // 4 >= 5 - 1
            if(_pos >= _text.Length - 1 )
                return string.Empty;

            string next = _text[_pos + 1].ToString();
            return next;
        }


        /// <summary>
        /// Returns the chars starting at current position + 1 and
        /// including the <paramref name="count"/> number of characters.
        /// </summary>
        /// <param name="count">Number of characters.</param>
        /// <returns>Range of chars as string or string.empty if end of text.</returns>
        public string PeekChars(int count)
        {
            // Validate.
            if (_pos + count > _text.Length)
                return string.Empty;

            return _text.Substring(_pos + 1, count);
        }


        /// <summary>
        /// Returns the nth char from the current char index
        /// </summary>
        /// <param name="countFromCurrentCharIndex">Number of characters from the current char index</param>
        /// <returns>Single char as string</returns>
        public string PeekChar(int countFromCurrentCharIndex)
        {
            // Validate.
            if (_pos + countFromCurrentCharIndex > _text.Length)
                return string.Empty;

            return _text[_pos + countFromCurrentCharIndex].ToString();
        }


        /// <summary>
        /// Peeks at the string all the way until the end of line.
        /// </summary>
        /// <returns>Current line.</returns>
        public string PeekLine()
        {
            int ndxEol = _text.IndexOf(Environment.NewLine, _pos + 1);
            if (ndxEol == -1)
                return _text.Substring(_pos + 1);

            return _text.Substring(_pos + 1, (ndxEol - _pos));
        }


        /// <summary>
        /// Advance and consume the current current char without storing 
        /// the char in the additional buffer for undo.
        /// </summary>
        public void ConsumeChar()
        {
            _pos++;
        }


        /// <summary>
        /// Consume the next <paramref name="count"/> chars without
        /// storing them in the additional buffer for undo.
        /// </summary>
        /// <param name="count">Number of characters to consume.</param>
        public void ConsumeChars(int count)
        {
            _pos += count;
        }


        /// <summary>
        /// Consume the whitespace without reading anything.
        /// </summary>
        public void ConsumeWhiteSpace()
        {
            ConsumeWhiteSpace(false);
        }


        /// <summary>
        /// Consume all white space.
        /// This works by checking the next char against
        /// the chars in the dictionary of chars supplied during initialization.
        /// </summary>
        /// <param name="readFirst">True to read a character
        /// before consuming the whitespace.</param>
        public void ConsumeWhiteSpace(bool readFirst)
        {
            string currentChar = _currentChar;
            
            if (readFirst)
                currentChar = ReadChar();

            while ( !IsEnd() && _whiteSpaceChars.ContainsKey(currentChar))
            {
                // Advance reader and next char.
                ReadChar();
                currentChar = _currentChar;
            }            
        }


        /// <summary>
        /// Consume new line.
        /// </summary>
        public void ConsumeNewLine()
        {
            // Check 
            string combinedNewLine = _currentChar + PeekChar();
            if (_eolChars.ContainsKey(combinedNewLine))
            {
                // Move to \n in \r\n
                ReadChar();
                ReadChar();
                return;
            }

            // Just \n
            if (_eolChars.ContainsKey(_currentChar))
                ReadChar();
        }


        /// <summary>
        /// Read text up to the eol.
        /// </summary>
        /// <returns>String read.</returns>
        public void ConsumeToNewLine(bool includeNewLine = false)
        {
            // Read until ":" colon and while not end of string.
            while (!IsEol(_eolChars as IDictionary) && _pos <= LAST_POSITION)
            {
                MoveChars(1);
            }
            if(includeNewLine) ConsumeNewLine();
        }


        /// <summary>
        /// Consume until the chars found.
        /// </summary>
        /// <param name="pattern">The pattern to consume chars to.</param>
        /// <param name="includePatternInConsumption">Wether or not to consume the pattern as well.</param>
        /// <param name="movePastEndOfPattern">Whether or not to move to the ending position of the pattern</param>
        /// <param name="moveToStartOfPattern">Whether or not to move to the starting position of the pattern</param>
        public bool ConsumeUntil(string pattern, bool includePatternInConsumption, bool moveToStartOfPattern, bool movePastEndOfPattern)
        {
            int ndx = _text.IndexOf(pattern, _pos);
            if (ndx == -1 ) return false;
            int newCharPos = 0;

            if(!includePatternInConsumption)
                newCharPos = moveToStartOfPattern ? ndx : ndx - 1;
            else
                newCharPos = movePastEndOfPattern ? ndx + pattern.Length : (ndx + pattern.Length) - 1;

            MoveChars(newCharPos - _pos);                        
            return true;                
        }


        /// <summary>
        /// Consume New Lines.
        /// </summary>
        public void ConsumeNewLines()
        {
            string combinedNewLine = _currentChar + PeekChar();
            while (_eolChars.ContainsKey(_currentChar) || _eolChars.ContainsKey(combinedNewLine) && _pos != LAST_POSITION)
            {
                ConsumeNewLine();
                combinedNewLine = _currentChar + PeekChar();            
            }
        }


        /// <summary>
        /// Moves forward by count chars.
        /// </summary>
        /// <param name="count"></param>
        public void MoveChars(int count)
        {
            _pos += count;
            if (_pos >= LAST_POSITION)
                _currentChar = _text[LAST_POSITION].ToString();
            else
                _currentChar = _text[_pos].ToString();
        }


        /// <summary>
        /// Read back the last char and reset
        /// </summary>
        public void ReadBackChar()
        {
            _pos--;
            _backBuffer.Remove(_backBuffer.Length - 1, 1);
            _currentChar = _text[_pos].ToString();
        }


        /// <summary>
        /// Unwinds the reader by <paramref name="count"/> chars.
        /// </summary>
        /// <param name="count">Number of characters to read.</param>
        public void ReadBackChar(int count)
        {
            // Unwind            
            _pos -= count;
            _backBuffer.Remove(_backBuffer.Length - count, count);
            _currentChar = _text[_pos].ToString();
        }


        /// <summary>
        /// Read the next char.
        /// </summary>
        /// <returns>Character read.</returns>
        public string ReadChar()
        {
            _pos++;

            // If the last char, can't read any more.
            if (_pos >= _text.Length)
            {
                _currentChar = END_CHAR;
                return _currentChar;
            }
            
            _currentChar = _text[_pos].ToString();
            _backBuffer.Append(_currentChar);
            //TO_DO: Convert to char.
            return _currentChar;
        }


        /// <summary>
        /// Read the next <paramref name="count"/> number of chars.
        /// </summary>
        /// <param name="count">Number of characters to read.</param>
        /// <returns>Characters read.</returns>
        public string ReadChars(int count)
        {
            string text = _text.Substring(_pos + 1, count);
            _pos += count;
            _currentChar = _text[_pos].ToString();
            _backBuffer.Append(text);
            return text;
        }


        /// <summary>
        /// Read text up to the eol.
        /// </summary>
        /// <returns>String read.</returns>
        public string ReadToEol()
        {
            StringBuilder buffer = new StringBuilder();

            // Read until ":" colon and while not end of string.
            while (!IsEol(_eolChars as IDictionary) && _pos <= LAST_POSITION)
            {
                buffer.Append(_currentChar);
                ReadChar();
            }
            return buffer.ToString();
        }


        /// <summary>
        /// ReadToken until one of the endchars is found
        /// </summary>
        /// <param name="endChars">List of possible end chars which halts reading further.</param>
        /// <param name="includeEndChar">True to include end character.</param>
        /// <param name="advanceFirst">True to advance before reading.</param>
        /// <param name="readPastEndChar">True to read past the end character.</param>
        /// <returns></returns>
        public string ReadTokenUntil(char[] endChars, bool includeEndChar = false, bool advanceFirst = false, bool readPastEndChar = false )
        {
            var buffer = new StringBuilder();
            bool found = false;
            if (advanceFirst) ReadChar();

            while (_pos < LAST_POSITION && !found)
            {
                for (int ndx = 0; ndx < endChars.Length; ndx++)
                {
                    if (_currentChar == endChars[ndx].ToString())
                    {
                        found = true;
                        break;
                    }
                }
                if(!found || (found && includeEndChar))
                    buffer.Append(_currentChar);

                if(!found || (found && readPastEndChar))
                    ReadChar();
            }
            string token = buffer.ToString();
            return token;
        }


        /// <summary>
        /// Read a token.
        /// </summary>
        /// <param name="endChar">Ending character.</param>
        /// <param name="escapeChar">Escape character.</param>
        /// <param name="includeEndChar">True to include end character.</param>
        /// <param name="advanceFirst">True to advance before reading.</param>
        /// <param name="expectEndChar">True to expect an end charachter.</param>
        /// <param name="readPastEndChar">True to read past the end character.</param>
        /// <returns>Contens of token read.</returns>
        public string ReadToken(string endChar, string escapeChar, bool includeEndChar, bool advanceFirst, bool expectEndChar, bool readPastEndChar)
        {
            StringBuilder buffer = new StringBuilder();
            string currentChar = advanceFirst ? ReadChar() : _currentChar;

            // Read until ":" colon and while not end of string.
            while (currentChar != endChar && _pos <= LAST_POSITION)
            {
                // Escape char
                if (currentChar == escapeChar)
                {
                    currentChar = ReadChar();
                    buffer.Append(currentChar);
                }
                else
                    buffer.Append(currentChar);

                currentChar = ReadChar();
            }
            bool matchedEndChar = true;

            // Error out if current char is not ":".
            if (expectEndChar && _currentChar != endChar)
            {
                _errors.Add("Expected " + endChar + " at : " + _pos);
                matchedEndChar = false;
            }

            // Read past char.
            if (matchedEndChar && readPastEndChar)
                ReadChar();

            return buffer.ToString();
        }


        /// <summary>
        /// Consume all white space.
        /// This works by checking the next char against
        /// the chars in the dictionary of chars supplied during initialization.
        /// </summary>
        /// <param name="readFirst">True to read a character
        /// before consuming the whitepsace.</param>
        /// <param name="consumeLastWhiteSpace"></param>
        public void ConsumeWhiteSpace(bool readFirst, bool consumeLastWhiteSpace)
        {
            string currentChar = readFirst ? ReadChar() : _currentChar;
            string nextChar = PeekChar();


            while (!IsEnd() && _whiteSpaceChars.ContainsKey(nextChar))
            {
                // Advance reader and next char.
                ReadChar();
                nextChar = PeekChar();
            } 
        }   


        /// <summary>
        /// Read text up to the eol.
        /// </summary>
        /// <param name="endChar">Ending character.</param>
        /// <param name="escapeChar">Escape character.</param>
        /// <param name="advanceFirst">True to advance before reading.</param>
        /// <param name="expectEndChar">True to expect an end charachter.</param>
        /// <param name="includeEndChar">True to include an end character.</param>
        /// <param name="moveToEndChar">True to move to the end character.</param>
        /// <param name="readPastEndChar">True to read past the end character.</param>
        /// <returns>Contents of token read.</returns>
        public string ReadTokenUntil(string endChar, string escapeChar, bool advanceFirst, bool expectEndChar, bool includeEndChar, bool moveToEndChar, bool readPastEndChar)
        {
            // abcd <div>
            var buffer = new StringBuilder();
            var currentChar = advanceFirst ? ReadChar() : _currentChar;
            var nextChar = PeekChar();
            while (nextChar != endChar && _pos <= LAST_POSITION)
            {
                // Escape char
                if (currentChar == escapeChar)
                {
                    currentChar = ReadChar();
                    buffer.Append(currentChar);
                }
                else
                    buffer.Append(currentChar);

                currentChar = ReadChar();
                nextChar = PeekChar();
            }
            bool matchedEndChar = nextChar == endChar;
            if( expectEndChar && !matchedEndChar)
                _errors.Add("Expected " + endChar + " at : " + _pos);

            if (matchedEndChar)
            {
                buffer.Append(currentChar);
                if (includeEndChar)
                    buffer.Append(nextChar);

                if (moveToEndChar)
                    ReadChar();

                else if (readPastEndChar && !IsAtEnd())
                    ReadChars(2);
            }
            
            return buffer.ToString();
        }


        /// <summary>
        /// Determines whether the current character is the expected one.
        /// </summary>
        /// <param name="charToExpect">Character to expect.</param>
        /// <returns>True if the current character is the expected one.</returns>
        public bool Expect(string charToExpect)
        {
            bool isMatch = _currentChar == charToExpect;
            if(!isMatch)
                _errors.Add("Expected " + charToExpect + " at : " + _pos);
            return isMatch;
        }


        /// <summary>
        /// Current char.
        /// </summary>
        /// <returns>Current character.</returns>
        public string CurrentChar
        {
            get { return _currentChar; }
        }


        /// <summary>
        /// Get the previous char that was read in.
        /// </summary>
        public string PreviousChar
        {
            get
            {
                // Check.
                if (_pos <= 0 || _backBuffer.Length <= 0)
                    return string.Empty;

                // Get the last char from the back buffer.
                // This is the last valid char that is not escaped.
                return _backBuffer[_backBuffer.Length - 2].ToString();
            }
        }


        /// <summary>
        /// Get the end of line chars.
        /// </summary>
        public IDictionary<string, string> EolChars
        {
            get { return _readonlyEolMap; }
        }


        /// <summary>
        /// Get the previous char that is part of the input and which may be an escape char.
        /// </summary>
        public string PreviousCharAny
        {
            get
            {
                // Check.
                if (_pos <= 0 )
                    return string.Empty;

                // Get the last char from the back buffer.
                // This is the last valid char that is not escaped.
                return _text[_pos - 1].ToString();
            }
        }

        
        /// <summary>
        /// Current position in text.
        /// </summary>
        /// <returns>Current character index.</returns>
        public int CurrentCharIndex()
        {
            return _pos;
        }


        /// <summary>
        /// Determine if current char is token.
        /// </summary>
        /// <returns>True if the current char is a token.</returns>
        public bool IsToken()
        {
            return _tokens.ContainsKey(_currentChar);
        }


        /// <summary>
        /// Whether or not the current sequence of chars matches the token supplied.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ignoreCase">Whether or not to check against case.</param>
        /// <returns></returns>
        public bool IsToken(string token, bool ignoreCase = false)
        {
            return string.Compare(_text, _pos, token, 0, token.Length, ignoreCase) == 0;
        }


        /// <summary>
        /// Determine if current char is escape char.
        /// </summary>
        /// <returns>True if the current char is an escape char.</returns>
        public bool IsEscape()
        {
            return string.Compare(_currentChar, _escapeChar, false) == 0;
        }


        /// <summary>
        /// Determine if the end of the text input has been reached.
        /// </summary>
        /// <returns>True if the end of the stream has been reached.</returns>
        public bool IsEnd()
        {
            return _pos >= _text.Length;
        }


        /// <summary>
        /// Determine if at last char.
        /// </summary>
        /// <returns>True if the last character is the current character.</returns>
        public bool IsAtEnd()
        {
            return _pos == _text.Length - 1;
        }


        /// <summary>
        /// Is End of line.
        /// </summary>
        /// <returns>True if the current character is an eol.</returns>
        public bool IsEol()
        {
            return IsEol(_eolChars as IDictionary);
        }


        /// <summary>
        /// Determine if current char is EOL.
        /// </summary>
        /// <param name="eolChars">Dictionary with eol chars.</param>
        /// <returns>True if the current character is an eol.</returns>
        public bool IsEol(IDictionary eolChars)
        {
            // Check for "\r\n"
            string combined = _currentChar + PeekChar();

            if (eolChars.Contains(combined) || eolChars.Contains(_currentChar) )
                return true;

            return false;
        }


        /// <summary>
        /// Determine if current char is whitespace.
        /// </summary>
        /// <param name="whitespaceChars">Dictionary with whitespace chars.</param>
        /// <returns>True if the current character is a whitespace.</returns>
        public bool IsWhiteSpace(IDictionary whitespaceChars)
        {
            return whitespaceChars.Contains(_currentChar);
        }


        /// <summary>
        /// Determine if current char is whitespace.
        /// </summary>
        /// <returns>True if the current character is a whitespace.</returns>
        public bool IsWhiteSpace()
        {
            return this._whiteSpaceChars.ContainsKey(_currentChar);
        }
        #endregion


        #region Private methods
        private void Init(IDictionary<string, bool> tokens, string[] tokenList)
        {
            foreach (string token in tokenList)
            {
                tokens[token] = true;
            }
        }


        /// <summary>
        /// Check if all of the items in the collection satisfied by the condition.
        /// </summary>
        /// <typeparam name="T">Type of items.</typeparam>
        /// <param name="items">List of items.</param>
        /// <returns>Dictionary of items.</returns>
        public static IDictionary<T, T> ToDictionary<T>(IList<T> items)
        {
            IDictionary<T, T> dict = new Dictionary<T, T>();
            foreach (T item in items)
            {
                dict[item] = item;
            }
            return dict;
        }  
        #endregion
    }
}
