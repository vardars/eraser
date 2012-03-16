using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComLib.Lang
{
    /// <summary>
    /// Token for the language.
    /// </summary>
    public enum Operator
    {
        /* BINARY + - * / %  */
        /// <summary>
        /// +
        /// </summary>
        Add,


        /// <summary>
        /// -
        /// </summary>
        Subtract,
        
        
        /// <summary>
        /// * 
        /// </summary>
        Multiply,


        /// <summary>
        /// /
        /// </summary>
        Divide,


        /// <summary>
        /// %
        /// </summary>
        Modulus,


        /* COMPARE < <= > >= != ==  */
        /// <summary>
        /// &lt;
        /// </summary>
        LessThan,


        /// <summary>
        /// &lt;=
        /// </summary>
        LessThanEqual,


        /// <summary>
        /// >
        /// </summary>
        MoreThan,


        /// <summary>
        /// >=
        /// </summary>
        MoreThanEqual,

                
        /// <summary>
        /// =
        /// </summary>
        Equal,


        /// <summary>
        /// ==
        /// </summary>
        EqualEqual,


        /// <summary>
        /// !=
        /// </summary>
        NotEqual,


        /* CONDITION && ||  */
        /// <summary>
        /// and
        /// </summary>
        And,

        /// <summary>
        /// ||
        /// </summary>
        Or,


        /* UNARY ++ -- += -= *= /= */
        /// <summary>
        /// ++
        /// </summary>
        PlusPlus,


        /// <summary>
        /// --
        /// </summary>
        MinusMinus,


        /// <summary>
        /// += 
        /// </summary>
        PlusEqual,


        /// <summary>
        /// -=
        /// </summary>
        MinusEqual,


        /// <summary>
        /// *=
        /// </summary>
        MultEqual,


        /// <summary>
        /// /=
        /// </summary>
        DivEqual,


        /// <summary>
        /// (
        /// </summary>
        LeftParenthesis,


        /// <summary>
        /// )
        /// </summary>
        RightParenthesis,


        /// <summary>
        /// {
        /// </summary>
        LeftBrace,


        /// <summary>
        /// }
        /// </summary>
        RightBrace
    }



    /// <summary>
    /// Operator class helper methods
    /// </summary>
    public class Ops
    {
        private static IDictionary<Operator, bool> _mathOps = new Dictionary<Operator, bool>()
        {
            { Operator.Multiply, true },
            { Operator.Divide, true },
            { Operator.Add, true },
            { Operator.Subtract, true },
            { Operator.Modulus, true },
        };


        private static IDictionary<Operator, bool> _compareOps = new Dictionary<Operator, bool>()
        {
            { Operator.LessThan, true },
            { Operator.LessThanEqual, true },
            { Operator.MoreThan, true },
            { Operator.MoreThanEqual, true },
            { Operator.EqualEqual, true },
            { Operator.NotEqual, true },
        };


        private static IDictionary<Operator, bool> _incrementOps = new Dictionary<Operator, bool>()
        {
            { Operator.PlusPlus, true },
            { Operator.PlusEqual, true },
            { Operator.MinusMinus, true },
            { Operator.MinusEqual, true },
            { Operator.MultEqual, true },
            { Operator.DivEqual, true },
        };


        private static IDictionary<string, Operator> _textToOps = new Dictionary<string, Operator>()
        {
            { "*", Operator.Multiply },
            { "/", Operator.Divide }, 
            { "+", Operator.Add },
            { "-", Operator.Subtract },
            { "%", Operator.Modulus },
            { "<", Operator.LessThan },
            { "<=", Operator.LessThanEqual },
            { ">", Operator.MoreThan },
            { ">=", Operator.MoreThanEqual },
            { "==", Operator.EqualEqual },
            { "!=", Operator.NotEqual },
            { "=", Operator.Equal },
            { "&&", Operator.And },
            { "||", Operator.Or },
            { "(", Operator.LeftParenthesis },
            { ")", Operator.RightParenthesis },
            { "{", Operator.LeftBrace },
            { "}", Operator.RightBrace },
            { "++", Operator.PlusPlus },
            { "--", Operator.MinusMinus },
            { "+=", Operator.PlusEqual },
            { "-=", Operator.MinusEqual },
            { "*=", Operator.MultEqual },
            { "/=", Operator.DivEqual }
        };
             
        
        /// <summary>
        /// Checks if the operator supplied is a binary op ( * / + - % )
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static bool IsBinary(Operator op)
        {
            return _mathOps.ContainsKey(op);
        }


        /// <summary>
        /// Checks if the operator supplied is a binary op ( * / + - % )
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static bool IsCompare(Operator op)
        {
            return _compareOps.ContainsKey(op);
        }


        /// <summary>
        /// Checks if the operator supplied is a binary op ( * / + - % )
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static bool IsIncrement(Operator op)
        {
            return _incrementOps.ContainsKey(op);
        }


        /// <summary>
        /// Checks if the operator supplied is a binary op ( * / + - % )
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static bool IsLogical(Operator op)
        {
            return op == Operator.And || op == Operator.Or;
        }


        /// <summary>
        /// Gets the enum from the text operator
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static Operator ToOp(string op)
        {
            return _textToOps[op];
        }
    }
}
