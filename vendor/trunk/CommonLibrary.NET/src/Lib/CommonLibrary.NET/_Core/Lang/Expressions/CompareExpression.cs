﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;


namespace ComLib.Lang
{
    /// <summary>
    /// Condition expression less, less than equal, more, more than equal etc.
    /// </summary>
    public class CompareExpression : Expression
    {
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="left">Left hand expression</param>
        /// <param name="op">Operator</param>
        /// <param name="right">Right expression</param>
        public CompareExpression(Expression left, Operator op, Expression right)
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
        /// Operator > >= == != less less than
        /// </summary>
        public Operator Op;


        /// <summary>
        /// Right hand expression
        /// </summary>
        public Expression Right;


        /// <summary>
        /// Evaluate > >= != == less less than
        /// </summary>
        /// <returns></returns>
        public override object Evaluate()
        {
            // Validate
            if (! ( Left is ValueExpression && Right is ValueExpression ))
                throw new ArgumentException("Invalid expression");

            bool result = false;
            double left = ((ValueExpression)Left).EvaluateAs<double>();
            double right = ((ValueExpression)Right).EvaluateAs<double>();

            if (Op == Operator.LessThan)
            {
                result = left < right;
            }
            else if (Op == Operator.LessThanEqual)
            {
                result = left <= right;
            }
            else if (Op == Operator.MoreThan)
            {
                result = left > right;
            }
            else if (Op == Operator.MoreThanEqual)
            {
                result = left >= right;
            }
            else if (Op == Operator.EqualEqual)
            {
                result = left == right;
            }
            else if (Op == Operator.NotEqual)
            {
                result = left != right;
            }
            return result;
        }
    }    
}
