using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;


namespace ComLib.Lang
{
    /// <summary>
    /// For loop Expression data
    /// </summary>
    public class BlockStatement : Statement
    {
        /// <summary>
        /// List of statements
        /// </summary>
        protected List<Statement> _statements = new List<Statement>();


        /// <summary>
        /// Public access to statments.
        /// </summary>
        public List<Statement> Statements
        {
            get { return _statements; }
        }
    }


    /// <summary>
    /// Conditional based block statement used in ifs/elses/while
    /// </summary>
    public class ConditionalBlockStatement : BlockStatement
    {
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="statements"></param>
        public ConditionalBlockStatement(Expression condition, List<Statement> statements)
        {
            this.Condition = condition;
            this._statements = statements == null ? new List<Statement>() : statements;
        }


        /// <summary>
        /// The condition to check.
        /// </summary>
        public Expression Condition;

    }
}
