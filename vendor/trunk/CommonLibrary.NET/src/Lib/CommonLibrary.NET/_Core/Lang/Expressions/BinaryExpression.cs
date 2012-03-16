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
    public class BinaryExpression : Expression
    {
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="left">Left hand expression</param>
        /// <param name="op">Operator</param>
        /// <param name="right">Right expression</param>
        public BinaryExpression(Expression left, Operator op, Expression right)
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
            // Validate
            if (! ( Left is ValueExpression && Right is ValueExpression ))
                throw new ArgumentException("Invalid expression");

            object result = 0;
            double left = ((ValueExpression)Left).EvaluateAs<double>();
            double right = ((ValueExpression)Right).EvaluateAs<double>();

            if (Op == Operator.Multiply)
            {
                result = left * right;
            }
            else if (Op == Operator.Divide)
            {
                result = left / right;
            }
            else if (Op == Operator.Add)
            {
                result = left + right;
            }
            else if (Op == Operator.Subtract)
            {
                result = left - right;
            }
            else if (Op == Operator.Modulus)
            {
                result = left % right;
            }
            return result;
        }
    }    
}
