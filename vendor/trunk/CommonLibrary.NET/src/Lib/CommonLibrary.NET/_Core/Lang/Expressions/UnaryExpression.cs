using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;


namespace ComLib.Lang
{
    /// <summary>
    /// Variable expression data
    /// </summary>
    public class UnaryExpression : VariableExpression
    {
        private double Increment;
        private Operator Op;
        private Expression Expression;


        /// <summary>
        /// Initialize
        /// </summary>
        public UnaryExpression()
        {
        }


        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="scope">Scope containing objects</param>
        /// <param name="incValue">Value to increment</param>
        /// <param name="op">The unary operator</param>
        /// <param name="name">Variable name</param>
        public UnaryExpression(string name, double incValue, Operator op, Scope scope)
        {
            this.Name = name;
            this.Scope = scope;
            this.Op = op;
            this.Increment = incValue;
        }


        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="scope">Scope containing objects</param>
        /// <param name="exp">Expression representing value to increment by</param>
        /// <param name="op">The unary operator</param>
        /// <param name="name">Variable name</param>
        public UnaryExpression(string name, Expression exp, Operator op, Scope scope)
        {
            this.Name = name;
            this.Scope = scope;
            this.Op = op;
            this.Expression = exp;
        }
        

        /// <summary>
        /// Evaluate
        /// </summary>
        /// <returns></returns>
        public override object Evaluate()
        {
            double val = this.Scope.Get<double>(this.Name);
            this.DataType = typeof(double);
            if (this.Expression != null)
                Increment = this.Expression.EvaluateAs<double>();
            else if (Increment == 0)
                Increment = 1;

            if (Op == Operator.PlusPlus)
            {
                val++;
            }
            else if (Op == Operator.MinusMinus)
            {
                val--;
            }
            else if (Op == Operator.PlusEqual)
            {
                val = val + Increment;
            }
            else if (Op == Operator.MinusEqual)
            {
                val = val - Increment;
            }
            else if (Op == Operator.MultEqual)
            {
                val = val * Increment;
            }
            else if (Op == Operator.DivEqual)
            {
                val = val / Increment;
            }
            
            // Set the value back into scope
            this.Value = val;
            this.Scope.SetValue(this.Name, val);

            return this.Value;
        }
    }
}
