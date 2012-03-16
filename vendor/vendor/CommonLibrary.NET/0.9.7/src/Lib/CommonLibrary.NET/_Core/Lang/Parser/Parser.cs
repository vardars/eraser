using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComLib.Lang
{
    /// <summary>
    /// Parses script in terms of sequences of Statements and Expressions;
    /// Each statement and expression is a sequence of Tokens( see Lexer )
    /// Main method is Parse(script) and ParseStatement();
    /// 
    /// 1. var name = "kishore";
    /// 2. if ( name == "kishore" ) print("true");
    /// 
    /// Statements:
    /// 
    /// VALUE:         TYPE:
    /// 1. AssignmentStatement ( "var name = "kishore"; )
    /// 2. IfStatement ( "if (name == "kishore" ) { print ("true"); }
    /// </summary>
    public class Parser
    {
        private Token _lastToken;
        private Token _nextToken;
        private Scope _scope = null;
        private Scanner _scanner = null;
        private Lexer _lexer = null;
        private string _script;
        private string _scriptPath;
        private List<Statement> _statements;

        // Used for end of expression;
        private IDictionary<Token, bool> _expSemicolonEnd = new Dictionary<Token, bool>()
        {
            { OperatorToken.Semicolon, true }
        };
        // Used for end of ( ) in if
        private IDictionary<Token, bool> _expParenthesisEnd = new Dictionary<Token, bool>()
        {
            { OperatorToken.RightParenthesis, true }
        };
        // Use in separating ( <exp> != <exp> && <exp> > <exp> )
        private IDictionary<Token, bool> _expPartEnd = new Dictionary<Token, bool>()
        {
            { OperatorToken.LogicalAnd, true },
            { OperatorToken.LogicalOr, true },
            { OperatorToken.Semicolon, true },
            { OperatorToken.RightParenthesis, true }
        };        


        /// <summary>
        /// Get the scope
        /// </summary>
        public Scope Scope { get { return _scope; } }


        /// <summary>
        /// Processes the script
        /// </summary>
        /// <param name="script">Script text</param>
        /// <param name="scope">Scope object</param>
        public List<Statement> Parse(string script, Scope scope = null)
        {
            Init(script, scope);
            
            while (true)
            {
                if (_nextToken == Token.EndToken)
                    break;
                
                // Get next statement.
                var stmt = ParseStatement();

                // Add to list of statements.
                _statements.Add(stmt);

                // Go to next token.
                Advance();
            }
            return _statements;
        }


        /// <summary>
        /// Executes all the statements
        /// </summary>
        public void Execute()
        {
            // Check number of statements.
            if (_statements == null || _statements.Count == 0) return;

            foreach (var stmt in _statements)
            {
                stmt.Execute();
            }
        }


        private void Init(string script, Scope scope)
        {
            _script = script;
            _scriptPath = string.Empty;
            _statements = new List<Statement>();
            _scope = scope == null ? new Scope() : scope;
            _scanner = new Scanner(_script);
            _lexer = new Lexer(_scanner);
            Advance();
        }


        private LangException BuildSyntaxException(string expected)
        {
            return new LangException("Syntax Error", string.Format("Expected {0} but found {1}", expected, _nextToken.Text), _scriptPath, _lexer.LineNumber);
        }


        #region Parse Statments
        private Statement ParseStatement()
        {
            // var
            if (_nextToken == KeywordToken.Var)
            {
                return ParseVar();
            }
            else if (_nextToken == KeywordToken.If)
            {
                return ParseIf();
            }
            else if (_nextToken == KeywordToken.While)
            {
                return ParseWhile();
            }
            else if (_nextToken == KeywordToken.For)
            {
                return ParseFor();
            }
            else if (_nextToken is IdToken)
            {
                return ParseIdBasedStatement();
            }

            throw BuildSyntaxException("token");
        }


        /// <summary>
        /// // 1. var name;
        /// 2. var age = 21;
        /// 3. var canDrink = age >= 21;
        /// 4. var canVote = CanVote(age);
        /// </summary>
        /// <returns></returns>
        private Statement ParseVar()
        {
            return ParseAssignment(true);
        }


        /// <summary>
        /// // 1. var name;
        /// 2. var age = 21;
        /// 3. var canDrink = age >= 21;
        /// 4. var canVote = CanVote(age);
        /// </summary>
        /// <returns></returns>
        private Statement ParseAssignment(bool expectVar, bool expectId = true, string name = "")
        {
            if (expectVar) Expect(KeywordToken.Var);
            if (expectId)  name = ExpectId();

            // var name;
            if (_nextToken == OperatorToken.Semicolon)
                return new AssignmentStatement(name, null, _scope);

            // var name = <expression>;
            Expect(OperatorToken.Assignment);
            var expression = ParseExpression(_expSemicolonEnd);
            Expect(OperatorToken.Semicolon, false);
            var stmt = new AssignmentStatement(name, expression, _scope);
            return stmt;
        }


        private Statement ParseIf()
        {
            IfStatement stmt = null;
            var statements = new List<Statement>();

            // While ( condition expression )
            Expect(KeywordToken.If);
            Expect(OperatorToken.LeftParenthesis);

            // Parse while condition
            var condition = ParseExpression(_expParenthesisEnd);
            Expect(OperatorToken.RightParenthesis);
            stmt = new IfStatement(condition);

            // Parse the block of statements.
            ParseBlock(stmt);
            return stmt;
        }

        
        /// <summary>
        /// Parses a while statment.
        /// </summary>
        /// <returns></returns>
        private Statement ParseWhile()
        {
            LoopStatement stmt = null;
            var statements = new List<Statement>();

            // While ( condition expression )
            Expect(KeywordToken.While);
            Expect(OperatorToken.LeftParenthesis);
            
            // Parse while condition
            var condition = ParseExpression(_expParenthesisEnd);
            Expect(OperatorToken.RightParenthesis);
            stmt = new LoopStatement(condition);

            // Parse the block of statements.
            ParseBlock(stmt);
            return stmt;
        }


        private Statement ParseFor()
        {
            ForLoopStatement stmt = null;
            var statements = new List<Statement>();

            // While ( condition expression )
            Expect(KeywordToken.For);
            Expect(OperatorToken.LeftParenthesis);

            // Parse while condition
            var start = ParseStatement();
            Advance();
            var condition = ParseExpression(_expSemicolonEnd);
            Advance();
            string name = ExpectId();
            var increment = ParseUnary(name, false);
            Expect(OperatorToken.RightParenthesis);
            stmt = new ForLoopStatement(start, condition, increment);

            // Parse the block of statements.
            ParseBlock(stmt);
            return stmt;
        }


        private Statement ParseIdBasedStatement()
        {
            string name = ExpectId();            
            if (_nextToken == OperatorToken.Assignment)
            {
                return ParseAssignment(false, false, name);
            }
            else if (_nextToken == OperatorToken.RightParenthesis)
            {
                return ParseFuncCall(name);
            }
            else //if (_nextToken == OperatorToken.Increment)
            {
                return ParseUnary(name);
            }
            throw BuildSyntaxException("identifier");
        }


        private Statement ParseFuncCall(string name)
        {
            var exp = ParseFuncExpression(name);
            return new FunctionCallStatement(exp);   
        }


        private BlockStatement ParseBlock(BlockStatement block)
        {
            // { statemnt1; statement2; }
            this.Expect(OperatorToken.LeftBrace);

            if(block == null) block = new BlockStatement();

            while (true)
            {
                // Check for end of statment or invalid end of script.
                if (IsEndOfStatementOrEndOfScript(OperatorToken.RightBrace))
                    break;

                // Parse statement by statement.
                block.Statements.Add(ParseStatement());

                // Go to next token.
                Advance();
            }
            this.Expect(OperatorToken.RightBrace, false);
            return block;
        }


        private Statement ParseUnary(string name, bool useSemicolonAsTerminator = true)
        {
            Operator op = Ops.ToOp(_nextToken.Text);
            double incrementValue = 1;
            AssignmentStatement stmt = null;
            Advance();

            // ++ -- 
            if (_nextToken == OperatorToken.Semicolon)
            {
                stmt = new AssignmentStatement(name, new UnaryExpression(name, incrementValue, op, _scope), _scope);
                Expect(OperatorToken.Semicolon, false);
            }
            else // += -= *= -=
            {
                var endTokens = useSemicolonAsTerminator ? _expSemicolonEnd : _expParenthesisEnd;
                var exp = ParseExpression(endTokens);
                stmt = new AssignmentStatement(name, new UnaryExpression(name, exp, op, _scope), _scope);
            }
            
            return stmt;
        }



        private Expression ParseExpression(IDictionary<Token, bool> endTokens)
        {
            Expression exp = null;

            // Build up a list of tokens
            // e.g. <const> <op> <var> <op> <var> <op> <const> ;
            //      21        <  age   &&   role  ==   "admin";
            while (true)
            {
                // Break for loop
                if (endTokens.ContainsKey(_nextToken) || _nextToken == Token.EndToken)
                    break;

                // && || ? read next expression part.
                if (_nextToken == OperatorToken.LogicalOr || _nextToken == OperatorToken.LogicalAnd)
                {
                    var op = OperatorToken.ToOp(_nextToken.Text);
                    Advance();
                    var right = ParseExpressionPart(_expPartEnd);
                    exp = new ConditionExpression(exp, op, right);
                }
                else
                {
                    exp = ParseExpressionPart(_expPartEnd);
                }

                // Break for loop
                if (endTokens.ContainsKey(_nextToken) || _nextToken == Token.EndToken)
                    break;

                if (_nextToken != OperatorToken.LogicalOr && _nextToken != OperatorToken.LogicalAnd)
                    this.Advance();
            }
            return exp;
        }


        private Expression ParseExpressionPart(IDictionary<Token, bool> endTokens)
        {
            Expression exp = null;

            // Build up a list of tokens
            // e.g. <const> <op> <var> <op> <var> <op> <const> ;
            //      21        <  age   &&   role  ==   "admin";
            while (true)
            {
                // Break for loop
                if (endTokens.ContainsKey(_nextToken) || _nextToken == Token.EndToken)
                    break;

                // 1. true / false / "name" / 123 / null;
                if (_nextToken is LiteralToken)
                {
                    exp = new ConstantExpression(_nextToken.Value);
                }
                // 2. name / age / isActive
                else if (_nextToken is IdToken)
                {
                    exp = new VariableExpression(_nextToken.Text, _scope);
                }
                // 3. Operator ( * / + - < <= > >= == != )
                else if (_nextToken is OperatorToken)
                {
                    var op = OperatorToken.ToOp(_nextToken.Text);
                    Advance();
                    if (Ops.IsCompare(op))
                    {
                        var right = ParseExpressionPart(endTokens);
                        exp = new CompareExpression(exp, op, right);
                    }
                    else if (Ops.IsBinary(op))
                    {
                        var right = ParseExpressionPart(endTokens);
                        exp = new BinaryExpression(exp, op, right);
                    }
                }
                // Break for loop
                if (endTokens.ContainsKey(_nextToken) || _nextToken == Token.EndToken)
                    break;

                // Next token
                this.Advance();
            }
            return exp;
        }


        private Expression ParseFuncExpression(string name)
        {
            Expect(OperatorToken.LeftParenthesis);
            var paramMap = new Dictionary<string, object>();
            var paramList = new List<Expression>();
            var funcExp = new FunctionCallExpression();
            funcExp.Name = name;

            while (true)
            {
                // Check for end of statment or invalid end of script.
                if (IsEndOfStatementOrEndOfScript(OperatorToken.RightParenthesis))
                    break;

                if (_nextToken == OperatorToken.Comma)
                {
                    Advance();
                }

                var exp = ParseExpression(_expParenthesisEnd);
                funcExp.ParamList.Add(exp);

                // Check for end of statment or invalid end of script.
                if (IsEndOfStatementOrEndOfScript(OperatorToken.RightParenthesis))
                    break;

                // Go to next token.
                Advance();
            }
            Expect(OperatorToken.RightParenthesis);
            Expect(OperatorToken.Semicolon, false);
            return funcExp;
        }
        #endregion


        #region Expect
        private void AdvanceAndExpect(Token expectedToken)
        {
            Advance();
            Expect(expectedToken);
        }


        private void Advance()
        {
            while (true)
            {
                _lastToken = _nextToken;
                _nextToken = _lexer.NextToken();
                if (_nextToken != LiteralToken.WhiteSpace)
                    break;
            }
        }


        /// <summary>
        /// Expect the token supplied and advance to next token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="advance">Whether or not to advance to the next token after expecting token</param>
        private void Expect(Token token, bool advance = true)
        {
            if (_nextToken == Token.EndToken)
                return;

            if (_nextToken != token)
                throw BuildSyntaxException(token.Text);

            if ( advance )
                Advance();
        }

        
        /// <summary>
        /// Expect identifier
        /// </summary>
        /// <returns></returns>
        private string ExpectId(bool advance = true)
        {
            if(!(_nextToken is IdToken))
                throw BuildSyntaxException("identifier");

            string id = ((IdToken)_nextToken).Text;

            if (advance)
                Advance();

            return id;
        }
        #endregion


        #region Helpers
        /// <summary>
        /// End of statement script.
        /// </summary>
        /// <param name="endOfStatementToken"></param>
        /// <returns></returns>
        private bool IsEndOfStatementOrEndOfScript(Token endOfStatementToken)
        {
            return IsEndOfStatement(endOfStatementToken) || IsEndOfScript();
        }


        /// <summary>
        /// Whether at end of statement.
        /// </summary>
        /// <returns></returns>
        private bool IsEndOfStatement(Token endOfStatementToken)
        {
            return (_nextToken == endOfStatementToken);
        }


        /// <summary>
        /// Whether at end of script
        /// </summary>
        /// <returns></returns>
        private bool IsEndOfScript()
        {
            return _nextToken == Token.EndToken;
        }

        #endregion
    }
}
