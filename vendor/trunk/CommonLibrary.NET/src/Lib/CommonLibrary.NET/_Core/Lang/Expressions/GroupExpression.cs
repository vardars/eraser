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
    public class GroupExpression : Expression
    {
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="left">Left hand expression</param>
        /// <param name="op">Operator</param>
        /// <param name="right">Right expression</param>
        public GroupExpression(Expression left, Operator op, Expression right)
        {
            Left = left;
            Right = right;
            Op = op;
        }


        /// <summary>
        /// Left hand expression
        /// </summary>
        public Expression Left;


        /// <summary>
        /// Operator * - / + % 
        /// </summary>
        public Operator Op;


        /// <summary>
        /// Right hand expression
        /// </summary>
        public Expression Right;


        /// <summary>
        /// Evaluate * / + - % 
        /// </summary>
        /// <returns></returns>
        public override object Evaluate()
        {
            object result = null;

            // binary or compare expression?
            if (Ops.IsCompare(this.Op))
            {
                var expComp = new CompareExpression(Left, Op, Right);
                result = expComp.Evaluate();
            }
            else if (Ops.IsBinary(this.Op))
            {
                var expComp = new BinaryExpression(Left, Op, Right);
                result = expComp.Evaluate();
            }
            return result;
        }
    }    
}
