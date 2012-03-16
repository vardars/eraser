using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComLib.Lang
{
    /// <summary>
    /// Base class for statements.
    /// </summary>
    public class Statement
    {
        /// <summary>
        /// Executes the statement.
        /// </summary>
        public virtual void Execute()
        {
        }
    }
}
