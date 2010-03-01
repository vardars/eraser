
using System;

namespace alglib
{
    public class testlegendreunit
    {
        public static bool testlegendrecalculate(bool silent)
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
            double t = 0;
            bool waserrors = new bool();

            err = 0;
            sumerr = 0;
            cerr = 0;
            threshold = 1.0E-9;
            waserrors = false;
            
            //
            // Testing Legendre polynomials values
            //
            for(n=0; n<=10; n++)
            {
                legendre.legendrecoefficients(n, ref c);
                for(pass=1; pass<=10; pass++)
                {
                    x = 2*AP.Math.RandomReal()-1;
                    v = legendre.legendrecalculate(n, x);
                    t = 1;
                    for(i=0; i<=n; i++)
                    {
                        v = v-c[i]*t;
                        t = t*x;
                    }
                    err = Math.Max(err, Math.Abs(v));
                }
            }
            
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
                    v = v+legendre.legendrecalculate(n, x)*c[n];
                    sumerr = Math.Max(sumerr, Math.Abs(v-legendre.legendresum(ref c, n, x)));
                }
            }
            
            //
            // Testing coefficients
            //
            legendre.legendrecoefficients(0, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-1));
            legendre.legendrecoefficients(1, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-0));
            cerr = Math.Max(cerr, Math.Abs(c[1]-1));
            legendre.legendrecoefficients(2, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]+(double)(1)/(double)(2)));
            cerr = Math.Max(cerr, Math.Abs(c[1]-0));
            cerr = Math.Max(cerr, Math.Abs(c[2]-(double)(3)/(double)(2)));
            legendre.legendrecoefficients(3, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-0));
            cerr = Math.Max(cerr, Math.Abs(c[1]+(double)(3)/(double)(2)));
            cerr = Math.Max(cerr, Math.Abs(c[2]-0));
            cerr = Math.Max(cerr, Math.Abs(c[3]-(double)(5)/(double)(2)));
            legendre.legendrecoefficients(4, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-(double)(3)/(double)(8)));
            cerr = Math.Max(cerr, Math.Abs(c[1]-0));
            cerr = Math.Max(cerr, Math.Abs(c[2]+(double)(30)/(double)(8)));
            cerr = Math.Max(cerr, Math.Abs(c[3]-0));
            cerr = Math.Max(cerr, Math.Abs(c[4]-(double)(35)/(double)(8)));
            legendre.legendrecoefficients(9, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-0));
            cerr = Math.Max(cerr, Math.Abs(c[1]-(double)(315)/(double)(128)));
            cerr = Math.Max(cerr, Math.Abs(c[2]-0));
            cerr = Math.Max(cerr, Math.Abs(c[3]+(double)(4620)/(double)(128)));
            cerr = Math.Max(cerr, Math.Abs(c[4]-0));
            cerr = Math.Max(cerr, Math.Abs(c[5]-(double)(18018)/(double)(128)));
            cerr = Math.Max(cerr, Math.Abs(c[6]-0));
            cerr = Math.Max(cerr, Math.Abs(c[7]+(double)(25740)/(double)(128)));
            cerr = Math.Max(cerr, Math.Abs(c[8]-0));
            cerr = Math.Max(cerr, Math.Abs(c[9]-(double)(12155)/(double)(128)));
            
            //
            // Reporting
            //
            waserrors = (double)(err)>(double)(threshold) | (double)(sumerr)>(double)(threshold) | (double)(cerr)>(double)(threshold);
            if( !silent )
            {
                System.Console.Write("TESTING CALCULATION OF THE LEGENDRE POLYNOMIALS");
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
        public static bool testlegendreunit_test_silent()
        {
            bool result = new bool();

            result = testlegendrecalculate(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testlegendreunit_test()
        {
            bool result = new bool();

            result = testlegendrecalculate(false);
            return result;
        }
    }
}
