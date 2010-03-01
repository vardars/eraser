
using System;

namespace alglib
{
    public class testlaguerreunit
    {
        public static bool testlaguerrecalculate(bool silent)
        {
            bool result = new bool();
            double err = 0;
            double sumerr = 0;
            double cerr = 0;
            double threshold = 0;
            int n = 0;
            int maxn = 0;
            int i = 0;
            int j = 0;
            int pass = 0;
            double[] c = new double[0];
            double x = 0;
            double v = 0;
            bool waserrors = new bool();

            err = 0;
            sumerr = 0;
            cerr = 0;
            threshold = 1.0E-9;
            waserrors = false;
            
            //
            // Testing Laguerre polynomials
            //
            n = 0;
            err = Math.Max(err, Math.Abs(laguerre.laguerrecalculate(n, 0.5)-1.0000000000));
            n = 1;
            err = Math.Max(err, Math.Abs(laguerre.laguerrecalculate(n, 0.5)-0.5000000000));
            n = 2;
            err = Math.Max(err, Math.Abs(laguerre.laguerrecalculate(n, 0.5)-0.1250000000));
            n = 3;
            err = Math.Max(err, Math.Abs(laguerre.laguerrecalculate(n, 0.5)+0.1458333333));
            n = 4;
            err = Math.Max(err, Math.Abs(laguerre.laguerrecalculate(n, 0.5)+0.3307291667));
            n = 5;
            err = Math.Max(err, Math.Abs(laguerre.laguerrecalculate(n, 0.5)+0.4455729167));
            n = 6;
            err = Math.Max(err, Math.Abs(laguerre.laguerrecalculate(n, 0.5)+0.5041449653));
            n = 7;
            err = Math.Max(err, Math.Abs(laguerre.laguerrecalculate(n, 0.5)+0.5183392237));
            n = 8;
            err = Math.Max(err, Math.Abs(laguerre.laguerrecalculate(n, 0.5)+0.4983629984));
            n = 9;
            err = Math.Max(err, Math.Abs(laguerre.laguerrecalculate(n, 0.5)+0.4529195204));
            n = 10;
            err = Math.Max(err, Math.Abs(laguerre.laguerrecalculate(n, 0.5)+0.3893744141));
            n = 11;
            err = Math.Max(err, Math.Abs(laguerre.laguerrecalculate(n, 0.5)+0.3139072988));
            n = 12;
            err = Math.Max(err, Math.Abs(laguerre.laguerrecalculate(n, 0.5)+0.2316496389));
            
            //
            // Testing Clenshaw summation
            //
            maxn = 20;
            c = new double[maxn+1];
            for(pass=1; pass<=10; pass++)
            {
                x = 2*AP.Math.RandomReal()-1;
                v = 0;
                for(n=0; n<=maxn; n++)
                {
                    c[n] = 2*AP.Math.RandomReal()-1;
                    v = v+laguerre.laguerrecalculate(n, x)*c[n];
                    sumerr = Math.Max(sumerr, Math.Abs(v-laguerre.laguerresum(ref c, n, x)));
                }
            }
            
            //
            // Testing coefficients
            //
            laguerre.laguerrecoefficients(0, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-1));
            laguerre.laguerrecoefficients(1, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-1));
            cerr = Math.Max(cerr, Math.Abs(c[1]+1));
            laguerre.laguerrecoefficients(2, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-(double)(2)/(double)(2)));
            cerr = Math.Max(cerr, Math.Abs(c[1]+(double)(4)/(double)(2)));
            cerr = Math.Max(cerr, Math.Abs(c[2]-(double)(1)/(double)(2)));
            laguerre.laguerrecoefficients(3, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-(double)(6)/(double)(6)));
            cerr = Math.Max(cerr, Math.Abs(c[1]+(double)(18)/(double)(6)));
            cerr = Math.Max(cerr, Math.Abs(c[2]-(double)(9)/(double)(6)));
            cerr = Math.Max(cerr, Math.Abs(c[3]+(double)(1)/(double)(6)));
            laguerre.laguerrecoefficients(4, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-(double)(24)/(double)(24)));
            cerr = Math.Max(cerr, Math.Abs(c[1]+(double)(96)/(double)(24)));
            cerr = Math.Max(cerr, Math.Abs(c[2]-(double)(72)/(double)(24)));
            cerr = Math.Max(cerr, Math.Abs(c[3]+(double)(16)/(double)(24)));
            cerr = Math.Max(cerr, Math.Abs(c[4]-(double)(1)/(double)(24)));
            laguerre.laguerrecoefficients(5, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-(double)(120)/(double)(120)));
            cerr = Math.Max(cerr, Math.Abs(c[1]+(double)(600)/(double)(120)));
            cerr = Math.Max(cerr, Math.Abs(c[2]-(double)(600)/(double)(120)));
            cerr = Math.Max(cerr, Math.Abs(c[3]+(double)(200)/(double)(120)));
            cerr = Math.Max(cerr, Math.Abs(c[4]-(double)(25)/(double)(120)));
            cerr = Math.Max(cerr, Math.Abs(c[5]+(double)(1)/(double)(120)));
            laguerre.laguerrecoefficients(6, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-(double)(720)/(double)(720)));
            cerr = Math.Max(cerr, Math.Abs(c[1]+(double)(4320)/(double)(720)));
            cerr = Math.Max(cerr, Math.Abs(c[2]-(double)(5400)/(double)(720)));
            cerr = Math.Max(cerr, Math.Abs(c[3]+(double)(2400)/(double)(720)));
            cerr = Math.Max(cerr, Math.Abs(c[4]-(double)(450)/(double)(720)));
            cerr = Math.Max(cerr, Math.Abs(c[5]+(double)(36)/(double)(720)));
            cerr = Math.Max(cerr, Math.Abs(c[6]-(double)(1)/(double)(720)));
            
            //
            // Reporting
            //
            waserrors = (double)(err)>(double)(threshold) | (double)(sumerr)>(double)(threshold) | (double)(cerr)>(double)(threshold);
            if( !silent )
            {
                System.Console.Write("TESTING CALCULATION OF THE LAGUERRE POLYNOMIALS");
                System.Console.WriteLine();
                System.Console.Write("Max error                                 ");
                System.Console.Write("{0,5:E2}",err);
                System.Console.WriteLine();
                System.Console.Write("Summation error                           ");
                System.Console.Write("{0,5:E2}",sumerr);
                System.Console.WriteLine();
                System.Console.Write("Coefficients error                        ");
                System.Console.Write("{0,5:E2}",cerr);
                System.Console.WriteLine();
                System.Console.Write("Threshold                                 ");
                System.Console.Write("{0,5:E2}",threshold);
                System.Console.WriteLine();
                if( !waserrors )
                {
                    System.Console.Write("TEST PASSED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("TEST FAILED");
                    System.Console.WriteLine();
                }
            }
            result = !waserrors;
            return result;
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testlaguerreunit_test_silent()
        {
            bool result = new bool();

            result = testlaguerrecalculate(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testlaguerreunit_test()
        {
            bool result = new bool();

            result = testlaguerrecalculate(false);
            return result;
        }
    }
}
