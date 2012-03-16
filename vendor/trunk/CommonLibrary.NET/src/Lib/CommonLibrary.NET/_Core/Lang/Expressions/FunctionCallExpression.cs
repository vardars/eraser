using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;


namespace ComLib.Lang
{        
    /// <summary>
    /// Function call expression data.
    /// </summary>
    public class FunctionCallExpression : Expression
    {
        /// <summary>
        /// Name of the function.
        /// </summary>
        public string Name;


        /// <summary>
        /// List of arguments.
        /// </summary>
        public List<object> ParamList;


        /// <summary>
        /// Arguments to the function.
        /// </summary>
        public IDictionary ParamMap;


        /// <summary>
        /// Evauate and run the function
        /// </summary>
        /// <returns></returns>
        public override object Evaluate()
        {
            throw new NotImplementedException("Function call not implemented");
        }
    }
}
