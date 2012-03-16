using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComLib.Lang
{
    /// <summary>
    /// Boolean datatype.
    /// </summary>
    public class LString : LObject
    {   
        /// <summary>
        /// Get string value.
        /// </summary>
        /// <returns></returns>
        public string ToStr()
        {
            return (string)_value;
        }
    }
}
