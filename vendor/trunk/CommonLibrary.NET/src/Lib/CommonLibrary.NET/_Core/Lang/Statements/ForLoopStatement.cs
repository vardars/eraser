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
    public class ForLoopStatement : ConditionalBlockStatement
    {
        /// <summary>
        /// Initialize
        /// </summary>
        public ForLoopStatement() : base(null, null)
        {
        }


        /// <summary>
        /// Initialize
        /// </summary>
        public ForLoopStatement(Statement start, Expression condition, Statement inc) : base(condition, null)
        {
            Start = start;
            Increment = inc;
        }


        /// <summary>
        /// Start statement.
        /// </summary>
        public Statement Start;


        /// <summary>
        /// Increment statement.
        /// </summary>
        public Statement Increment;



        /// <summary>
        /// Execute each expression.
        /// </summary>
        /// <returns></returns>
        public override void Execute()
        {
            Start.Execute();
            bool canContinue = Condition.EvaluateAs<bool>();
            while (canContinue)
            {
                foreach (var stmt in _statements)
                {
                    stmt.Execute();
                }
                Increment.Execute();
                canContinue = Condition.EvaluateAs<bool>();
            }
        }
    }    
}
