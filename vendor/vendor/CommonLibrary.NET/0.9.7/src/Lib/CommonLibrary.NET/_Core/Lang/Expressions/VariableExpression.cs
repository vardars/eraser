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
    public class VariableExpression : ValueExpression
    {
        /// <summary>
        /// Initialize
        /// </summary>
        public VariableExpression()
        {
        }


        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="scope">Scope containing objects</param>
        /// <param name="name">Variable name</param>
        public VariableExpression(string name, Scope scope)
        {
            this.Name = name;
            this.Scope = scope;
        }


        /// <summary>
        /// Scope containing variables
        /// </summary>
        public Scope Scope;


        /// <summary>
        /// Evaluate
        /// </summary>
        /// <returns></returns>
        public override object Evaluate()
        {
            this.Value = this.Scope.Get<object>(this.Name);
            this.DataType = this.Value.GetType();
            return this.Value;
        }


        /// <summary>
        /// Evaluate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override T EvaluateAs<T>()
        {
            object result = Evaluate();
            return (T)Convert.ChangeType(result, typeof(T));
        }
    }
}
