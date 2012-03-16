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
    public class IfStatement : ConditionalBlockStatement
    {
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="condition"></param>
        public IfStatement(Expression condition)
            : base(condition, null)
        {
        }


        /// <summary>
        /// Execute
        /// </summary>
        public override void Execute()
        {
            if (Condition.EvaluateAs<bool>())
            {
                if (_statements != null && _statements.Count > 0)
                {
                    foreach (var stmt in _statements)
                    {
                        stmt.Execute();
                    }
                }
            }
        }
    }    
}
