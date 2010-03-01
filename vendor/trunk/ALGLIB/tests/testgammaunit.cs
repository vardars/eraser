
using System;

namespace alglib
{
    public class testgammaunit
    {
        public static bool testgamma(bool silent)
        {
            bool result = new bool();
            double threshold = 0;
            double v = 0;
            double s = 0;
            bool waserrors = new bool();
            bool gammaerrors = new bool();
            bool lngammaerrors = new bool();

            gammaerrors = false;
            lngammaerrors = false;
            waserrors = false;
            threshold = 100*AP.Math.MachineEpsilon;
            
            //
            //
            //
            gammaerrors = gammaerrors | (double)(Math.Abs(gammafunc.gamma(0.5)-Math.Sqrt(Math.PI)))>(double)(threshold);
            gammaerrors = gammaerrors | (double)(Math.Abs(gammafunc.gamma(1.5)-0.5*Math.Sqrt(Math.PI)))>(double)(threshold);
            v = gammafunc.lngamma(0.5, ref s);
            lngammaerrors = lngammaerrors | (double)(Math.Abs(v-Math.Log(Math.Sqrt(Math.PI))))>(double)(threshold) | (double)(s)!=(double)(1);
            v = gammafunc.lngamma(1.5, ref s);
            lngammaerrors = lngammaerrors | (double)(Math.Abs(v-Math.Log(0.5*Math.Sqrt(Math.PI))))>(double)(threshold) | (double)(s)!=(double)(1);
            
            //
            // report
            //
            waserrors = gammaerrors | lngammaerrors;
            if( !silent )
            {
                System.Console.Write("TESTING GAMMA FUNCTION");
                System.Console.WriteLine();
                System.Console.Write("GAMMA:                                   ");
                if( gammaerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("LN GAMMA:                                ");
                if( lngammaerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                if( waserrors )
                {
                    System.Console.Write("TEST FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("TEST PASSED");
                    System.Console.WriteLine();
                }
                System.Console.WriteLine();
                System.Console.WriteLine();
            }
            
            //
            // end
            //
            result = !waserrors;
            return result;
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testgammaunit_test_silent()
        {
            bool result = new bool();

            result = testgamma(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testgammaunit_test()
        {
            bool result = new bool();

            result = testgamma(false);
            return result;
        }
    }
}
