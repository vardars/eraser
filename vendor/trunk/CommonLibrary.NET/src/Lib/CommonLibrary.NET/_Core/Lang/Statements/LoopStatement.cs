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
    public class LoopStatement : ConditionalBlockStatement
    {

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="condition"></param>
        public LoopStatement(Expression condition)
            : base(condition, null)
        {
        }


        /// <summary>
        /// Execute
        /// </summary>
        public override void Execute()
        {
            bool continueLoop = Condition.EvaluateAs<bool>();
            while (continueLoop)
            {
                if (_statements != null && _statements.Count > 0)
                {
                    foreach (var stmt in _statements)
                    {
                        stmt.Execute();
                    }
                }
                else break;

                continueLoop = Condition.EvaluateAs<bool>();
            }
        }
    }    
}
