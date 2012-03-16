using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Resources;
using NUnit.Framework;


using ComLib;
using ComLib.Lang;


namespace CommonLibrary.Tests
{

    [TestFixture]
    public class Lang_Expression_Tests
    {
        [Test]
        public void Can_Do_AssignmentExpressions()
        {
            Scope scope = new Scope();
            scope.SetValue("age", 32);
            scope.SetValue("isMale", true);
            scope.SetValue("name", "kishore");

            // Strings
            Assign(scope, "str1", "kishore1", true, "kishore1");
            Assign(scope, "str2", "name", false, "kishore");

            // Numbers
            Assign(scope, "num1", 2, true, 2);
            Assign(scope, "num3", 34.56, true, 34.56);
            Assign(scope, "num2", "age", false, 32);
            

            // bool
            Assign(scope, "b1", true, true, true);
            Assign(scope, "b2", false, true, false);
            Assign(scope, "b2", "isMale", false, true);
        }


        [Test]
        public void Can_Do_Math_Expressions_On_Constants()
        {
            Math(null, 5, 2, Operator.Multiply, 10);
            Math(null, 4, 2, Operator.Divide, 2);
            Math(null, 5, 2, Operator.Add, 7);
            Math(null, 5, 2, Operator.Subtract, 3);
            Math(null, 5, 2, Operator.Modulus, 1);
        }


        [Test]
        public void Can_Do_Math_Expressions_On_Variables()
        {
            Scope scope = new Scope();
            scope.SetValue("four", 4);
            scope.SetValue("five", 5);
            scope.SetValue("two", 2);
            Math(scope, "five", "two", Operator.Multiply, 10);
            Math(scope, "four", "two", Operator.Divide, 2);
            Math(scope, "five", "two", Operator.Add, 7);
            Math(scope, "five", "two", Operator.Subtract, 3);
            Math(scope, "five", "two", Operator.Modulus, 1);
        }


        [Test]
        public void Can_Do_Unary_Operations()
        {
            Scope scope = new Scope();
            scope.SetValue("one", 1);
            scope.SetValue("two", 2);
            scope.SetValue("three", 3);
            scope.SetValue("four", 4);
            scope.SetValue("five", 5);
            scope.SetValue("six", 6);

            Unary(scope, "one", 0, Operator.PlusPlus, 2);
            Unary(scope, "two", 2, Operator.PlusEqual, 4);
            Unary(scope, "three", 0, Operator.MinusMinus, 2);
            Unary(scope, "four", 2, Operator.MinusEqual, 2);
            Unary(scope, "five", 2, Operator.MultEqual, 10);
            Unary(scope, "six", 2, Operator.DivEqual, 3);
        }


        [Test]
        public void Can_Do_Math_Expressions_On_Constants_And_Variables()
        {
            Scope scope = new Scope();
            scope.SetValue("four", 4);
            scope.SetValue("five", 5);
            scope.SetValue("two", 2);            
            Math(scope, "five", 2, Operator.Multiply, 10);
            Math(scope, "four", 2, Operator.Divide, 2);
            Math(scope, "five", 2, Operator.Add, 7);
            Math(scope, "five", 2, Operator.Subtract, 3);
            Math(scope, "five", 2, Operator.Modulus, 1);

            Math(scope, 5, "two", Operator.Multiply, 10);
            Math(scope, 4, "two", Operator.Divide, 2);
        }


        [Test]
        public void Can_Do_Compare_Expressions_On_Constants()
        {
            // MORE THAN
            Compare(5, 4, Operator.MoreThan, true);
            Compare(5, 5, Operator.MoreThan, false);
            Compare(5, 6, Operator.MoreThan, false);

            // MORE THAN EQUAL
            Compare(5, 4, Operator.MoreThanEqual, true);
            Compare(5, 5, Operator.MoreThanEqual, true);
            Compare(5, 6, Operator.MoreThanEqual, false);

            // LESS THAN
            Compare(5, 6, Operator.LessThan, true);
            Compare(5, 5, Operator.LessThan, false);
            Compare(5, 4, Operator.LessThan, false);

            // LESS THAN EQUAL
            Compare(5, 6, Operator.LessThanEqual, true);
            Compare(5, 5, Operator.LessThanEqual, true);
            Compare(5, 4, Operator.LessThanEqual, false);

            // EQUAL
            Compare(5, 6, Operator.EqualEqual, false);
            Compare(5, 5, Operator.EqualEqual, true);
            Compare(5, 4, Operator.EqualEqual, false);

            // NOT EQUAL
            Compare(5, 6, Operator.NotEqual, true);
            Compare(5, 5, Operator.NotEqual, false);
            Compare(5, 4, Operator.NotEqual, true);
        }


        private void Compare(object left, object right, Operator op, bool expected)
        {
            // LESS THAN
            var exp = new CompareExpression(new ConstantExpression(left), op, new ConstantExpression(right));
            Assert.AreEqual(expected, exp.EvaluateAs<bool>());
        }


        private void Math(Scope scope, object left, object right, Operator op, double expected)
        {
            Expression expLeft = (left.GetType() == typeof(string))
                        ? (Expression)new VariableExpression(left.ToString(), scope)
                        : (Expression)new ConstantExpression(left);

            Expression expRight = (right.GetType() == typeof(string))
                         ? (Expression)new VariableExpression(right.ToString(), scope)
                         : (Expression)new ConstantExpression(right);

            var exp = new BinaryExpression(expLeft, op, expRight);
            Assert.AreEqual(expected, exp.EvaluateAs<double>());
        }


        private void Unary(Scope scope, string left, double inc, Operator op, double expected)
        {
            var exp = new UnaryExpression(left, inc, op, scope);
            Assert.AreEqual(expected, exp.EvaluateAs<double>());
            Assert.AreEqual(expected, scope.Get<double>(left));
        }


        private void Assign(Scope scope, string name, object val, bool isConst, object expected)
        {
            Expression expr = isConst
                            ? (Expression)new ConstantExpression(val)
                            : (Expression)new VariableExpression(val.ToString(), scope);
            var exp = new AssignmentStatement(name, expr, scope);
            exp.Execute();
            Assert.AreEqual(expected, scope.Get<object>(name));
        }
    }


    [TestFixture]
    public class Lang_Parse_Tests
    {
        [Test]
        public void Can_Do_Single_Assignment_Constant_Expressions()
        {
            var statements = new List<Tuple<string, Type, object, string>>()
            {
                new Tuple<string,Type, object, string>("name", typeof(object), null,        "var name;"),
                new Tuple<string,Type, object, string>("name", typeof(string), "kishore",   "var name = 'kishore';"),
                new Tuple<string,Type, object, string>("name", typeof(string), "kishore",   "var name = \"kishore\";"),
                new Tuple<string,Type, object, string>("age", typeof(double),   32,         "var age = 32;"),
                new Tuple<string,Type, object, string>("isActive", typeof(bool), true,      "var isActive = true;"),
                new Tuple<string,Type, object, string>("isActive", typeof(bool), false,     "var isActive = false;"),
            };
            Parse(statements);
        }


        [Test]
        public void Can_Do_Single_Assignment_Constant_Math_Expressions()
        {
            var statements = new List<Tuple<string, Type, object, string>>()
            {
                new Tuple<string,Type, object, string>("result", typeof(double), 8,  "var result = 4 * 2;"),
                new Tuple<string,Type, object, string>("result", typeof(double), 3,  "var result = 6 / 2;"),
                new Tuple<string,Type, object, string>("result", typeof(double), 6,  "var result = 4 + 2;"),
                new Tuple<string,Type, object, string>("result", typeof(double), 2,  "var result = 4 - 2;"),
                new Tuple<string,Type, object, string>("result", typeof(double), 1,  "var result = 5 % 2;"),
            };
            Parse(statements);
        }


        [Test]
        public void Can_Do_Single_Assignment_Constant_Compare_Expressions()
        {
            var statements = new List<Tuple<string, Type, object, string>>()
            {
                new Tuple<string,Type, object, string>("result", typeof(bool), true, "var result = 4 >  2;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), true, "var result = 4 >= 2;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), true, "var result = 4 <  6;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), true, "var result = 4 <= 6;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), true, "var result = 4 != 2;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), true, "var result = 4 == 4;"),
            };
            Parse(statements);
        }


        [Test]
        public void Can_Do_If_Statements_With_Constants()
        {
            var statements = new List<Tuple<string, Type, object, string>>()
            {
                new Tuple<string,Type, object, string>("result", typeof(double), 1, "var result = 1; if( 2 < 3 && 4 > 3 ){ result = 1; }"),
                new Tuple<string,Type, object, string>("result", typeof(double), 2, "var result = 1; if( 2 < 3 && 4 > 3 ){ result = 2; }"),
                new Tuple<string,Type, object, string>("result", typeof(double), 3, "var result = 1; if( 2 < 3 && 4 > 3 ){ result = 3; }"),
                new Tuple<string,Type, object, string>("result", typeof(double), 4, "var result = 1; if( 2 < 3 && 4 > 3 ){ result = 4; }"),
                new Tuple<string,Type, object, string>("result", typeof(double), 5, "var result = 1; if( 2 < 3 && 4 > 3 ){ result = 5; }"),
                new Tuple<string,Type, object, string>("result", typeof(double), 6, "var result = 1; if( 2 < 3 && 4 > 3 ){ result = 6; }"),
            };
            Parse(statements);
        }


        [Test]
        public void Can_Do_While_Statements()
        {
            var statements = new List<Tuple<string, Type, object, string>>()
            {
                new Tuple<string,Type, object, string>("result", typeof(double), 4, "var result = 1; while( result < 4 ){ result++; }"),
                new Tuple<string,Type, object, string>("result", typeof(double), 1, "var result = 4; while( result > 1 ){ result--; }")
            };
            Parse(statements);
        }


        [Test]
        public void Can_Do_For_Statements()
        {
            var statements = new List<Tuple<string, Type, object, string>>()
            {
                new Tuple<string,Type, object, string>("result", typeof(double), 4, "var result = 1; for(var ndx = 0; ndx < 5; ndx++) { result = ndx; }")
            };
            Parse(statements);
        }


        [Test]
        public void Can_Do_Single_Assignment_Constant_Logical_Expressions()
        {
            var statements = new List<Tuple<string, Type, object, string>>()
            {
                new Tuple<string,Type, object, string>("result", typeof(bool), true,  "var result = 1 >  2 || 3 < 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), true,  "var result = 1 >= 2 || 3 < 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), true,  "var result = 4 <  2 || 3 < 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), true,  "var result = 4 <= 2 || 3 < 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), true,  "var result = 2 != 2 || 3 < 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), true,  "var result = 2 == 4 || 3 < 4;"),

                new Tuple<string,Type, object, string>("result", typeof(bool), false, "var result = 1 >  2 || 3 > 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), false, "var result = 1 >= 2 || 3 > 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), false, "var result = 4 <  2 || 3 > 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), false, "var result = 4 <= 2 || 3 > 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), false, "var result = 2 != 2 || 3 > 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), false, "var result = 2 == 4 || 3 > 4;"),

                new Tuple<string,Type, object, string>("result", typeof(bool), true,  "var result = 1 <  2 && 3 < 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), true,  "var result = 1 <= 2 && 3 < 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), true,  "var result = 4 >= 2 && 3 < 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), true,  "var result = 1 <= 2 && 3 < 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), true,  "var result = 2 == 2 && 3 < 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), true,  "var result = 2 != 4 && 3 < 4;"),

                new Tuple<string,Type, object, string>("result", typeof(bool), false, "var result = 1 <  2 && 3 == 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), false, "var result = 1 <= 2 && 3 == 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), false, "var result = 4 >= 2 && 3 == 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), false, "var result = 4 <= 2 && 3 == 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), false, "var result = 2 == 2 && 3 == 4;"),
                new Tuple<string,Type, object, string>("result", typeof(bool), false, "var result = 2 <  4 && 3 == 4;")
            };
            Parse(statements);
        }


        [Test]
        public void Can_Do_Multiple_Assignment_Expressions()
        {
            var statements = new List<Tuple<string, Type, object, string>>()
            {
                new Tuple<string,Type, object, string>("result2", typeof(string), "kishore", "var result = 'kishore'; var result2 = result;"),
                new Tuple<string,Type, object, string>("result2", typeof(double), 8,         "var result = 4; var result2 = result * 2;"),
                new Tuple<string,Type, object, string>("result2", typeof(double), 3,         "var result = 6; var result2 = result / 2;"),
                new Tuple<string,Type, object, string>("result2", typeof(double), 6,         "var result = 4; var result2 = result + 2;"),
                new Tuple<string,Type, object, string>("result2", typeof(double), 2,         "var result = 4; var result2 = result - 2;"),
                new Tuple<string,Type, object, string>("result2", typeof(double), 1,         "var result = 5; var result2 = result % 2;"),
            };
            Parse(statements);
        }


        [Test]
        public void Can_Do_Unary_Expressions()
        {
            var statements = new List<Tuple<string, Type, object, string>>()
            {
                new Tuple<string,Type, object, string>("result", typeof(string), 3, "var result = 2; result++; "),
                new Tuple<string,Type, object, string>("result", typeof(double), 1, "var result = 2; result--; "),
                new Tuple<string,Type, object, string>("result", typeof(double), 4, "var result = 2; result += 2; "),
                new Tuple<string,Type, object, string>("result", typeof(double), 0, "var result = 2; result -= 2; "),
                new Tuple<string,Type, object, string>("result", typeof(double), 6, "var result = 2; result *= 3; "),
                new Tuple<string,Type, object, string>("result", typeof(double), 3, "var result = 6; result /= 2; "),
            };
            Parse(statements);
        }


        [Ignore]
        public void Can_Do_Funcion_Calls()
        {
            var statements = new List<Tuple<string, Type, object, string>>()
            {
                new Tuple<string,Type, object, string>("user.create", typeof(string), 3, "user.create();"),
                new Tuple<string,Type, object, string>("user.create", typeof(double), 1, "user.create('kishore', 123, true, 30.5);"),
                new Tuple<string,Type, object, string>("user.create", typeof(double), 4, "user.create(\"kishore\", 123, false, 30.5);")
            };
            Parse(statements);
        }


        private void Parse(List<Tuple<string, Type, object, string>> statements)
        {
            foreach (var stmt in statements)
            {

                Interpreter i = new Interpreter();
                i.Execute(stmt.Item4);

                Assert.AreEqual(i.Scope[stmt.Item1], stmt.Item3);
            }
        }
    }
}
